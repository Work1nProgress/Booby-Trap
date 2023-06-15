using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.Serialization;

public class ControllerGame : ControllerLocal
{
    static ControllerGame m_Instance;
    public static ControllerGame Instance => m_Instance;


    public int MaxPlayerHealth;

    public Player player;
    public DaddyController Daddy;
    Vector3 m_StartingPlayerPos = new Vector3(-4.15f, -4.34f);

    [SerializeField]
    ControllerEnemies ControllerEnemies;


    ControllerRooms ControllerRooms;


    #region Damage Animation
    CinemachineVirtualCamera vCam;

    CinemachineBasicMultiChannelPerlin noiseModule;

    ChromaticAberration ChromaticAberration;

    Vignette Vignette;

    [SerializeField]
    float VignetteDuration, VignetteIntensity;

    [SerializeField]
    float AberartionDuration, ShakeDuration, AberrationIntensity, ShakeAmplitude, ShakeFrequency;

    [SerializeField]
    Ease AberrationEase;

    public int SavedRoom => ControllerSaveLoad.GetSaveData.SaveRoom;

    public bool HasSpear;

    

    #endregion

    // Use this method to initialize everyhing you need at the begging of the scene
    public override void Init()
    {
        base.Init();

        player = FindFirstObjectByType<Player>();
        if (ControllerSaveLoad.GetSaveData.Patricide)
        {
            if (ControllerSaveLoad.GetSaveData.SaveRoom == 1)
            {
                player.transform.position = m_StartingPlayerPos;
            }
            else
            {
                player.transform.position = ControllerSaveLoad.GetSaveData.SavePosition;
            }
        }
        m_Instance = this;
        HasSpear = ControllerSaveLoad.GetSaveData.HasSpear;
        Daddy = FindFirstObjectByType<DaddyController>();
        player.Init(new EntityStats
        {
            MaxHealth = MaxPlayerHealth + ControllerSaveLoad.GetSaveData.HealthPickups.Count

        });
        if (playerHealthBar)
        {
            playerHealthBar.RerenderPips(player.MaxHealth, player.MaxHealth);
        }
      

        player.OnChangeHealth.AddListener(UpdatePlayerHealth);
        player.OnDeath.AddListener(OnPlayerDeath);
        UpdatePlayerHealth(0);
        var volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out ChromaticAberration);
        volume.profile.TryGet(out ColorAdjustments adjustments);
        volume.profile.TryGet(out Vignette);
        vCam = FindObjectOfType<CinemachineVirtualCamera>();
        ControllerRooms = GetComponent<ControllerRooms>();
        ControllerRooms.Init(vCam, adjustments);

        if (vCam != null)
        {
            noiseModule = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }


        var pickups = FindObjectsOfType<Pickup>();
        for (int i = 0; i < pickups.Length; i++)
        {

            if (string.IsNullOrEmpty(pickups[i].GetPickupID) ||
                ControllerSaveLoad.GetSaveData.HealthPickups.Contains(pickups[i].GetPickupID) ||
                ControllerSaveLoad.GetSaveData.Keycards.Contains(pickups[i].GetPickupID)
                )
            {
                Destroy(pickups[i].gameObject);
            }

        }
         var spearPickup = FindObjectOfType<SpearPickup>();
        if (ControllerSaveLoad.GetSaveData.HasSpear)
        {

            Destroy(spearPickup.gameObject);
        }


    }

    //move this in some kind of spear controller script
    [HideInInspector]
    public List<Spear> Spears = new List<Spear>();

    [SerializeField] private PlayerHealthBar playerHealthBar;

    public void RemoveSpear(int index = 0)
    {
        if (index < 0 || index > Instance.Spears.Count - 1)
        {
            return;
        }
        var toRemove = Instance.Spears[index];
        Instance.Spears.RemoveAt(index);
        PoolManager.Despawn(toRemove);
    }

    public void RemoveSpear(Spear spear)
    {

        RemoveSpear(GetSpearIndex(spear));

    }

    public int GetSpearIndex(Spear spear)
    {
        return Spears.IndexOf(spear);
    }

    public void UpdatePlayerHealth(int amount)
    {
        if (amount < 0)
        {
            AnimateCameraDamage();
        }
        else if(amount > 0)
        {

            SoundManager.Instance.Play("Echo_Heal", player.transform);
            AnimateVignetter(VignetteIntensity);
            DOVirtual.Float(VignetteIntensity, 0, VignetteDuration, AnimateVignetter).OnComplete(() => AnimateVignetter(0));

        }
    }

    public void OnPlayerDeath(){

      
        ControllerRooms.OnDeathAnimation();
    }

    public void ResetPlayer()
    {
        player.Heal(player.MaxHealth);
        if (ControllerSaveLoad.GetSaveData.SaveRoom == 1)
        {
            player.transform.position = m_StartingPlayerPos;
        }
        else
        {
            player.transform.position = ControllerSaveLoad.GetSaveData.SavePosition;

        }
        if (!ControllerSaveLoad.GetSaveData.Patricide)
        {
            ControllerSaveLoad.GetSaveData.Patricide = true;
            ControllerSaveLoad.Save();
        }
        UpdatePlayerHealth(0);
        playerHealthBar.RerenderPips(player.MaxHealth, player.MaxHealth);
        
        //if (Daddy != null)
        //{

        //    SoundManager.Instance.Play(Daddy.Sound.EchoDie, transform);
        //    Daddy.ResetDadsHp();
        //    Daddy.GetComponent<DaddyMusic>().ResetMusic();
        //}
    }




    public void AddAgressiveEnemy(EntityController entity)
    {
        ControllerEnemies.AddAggresiveEnemy(entity);
        entity.isAggressive = true;

    }


    public void PickupHealth(string healthID)
    {
        if (!ControllerSaveLoad.GetSaveData.HealthPickups.Contains(healthID))
        {
            ControllerSaveLoad.GetSaveData.HealthPickups.Add(healthID);
            ControllerSaveLoad.Save();
            player.IncreaseMaxHealth(1);
            playerHealthBar.RerenderPips(player.Health, player.MaxHealth);
        }
    }

    public void PickupKeycard(string keycardID)
    {
        if (!ControllerSaveLoad.GetSaveData.Keycards.Contains(keycardID))
        {
            ControllerSaveLoad.GetSaveData.Keycards.Add(keycardID);
            ControllerSaveLoad.Save();
        }
    }

    public void PickupSpear()
    {
        ControllerSaveLoad.GetSaveData.HasSpear = true;
        HasSpear = true;
        ControllerSaveLoad.Save();

    }

    public void SaveRoom(int room)
    {
        ControllerSaveLoad.GetSaveData.SaveRoom = room;
        ControllerSaveLoad.GetSaveData.SavePosition = player.transform.position;
        ControllerSaveLoad.Save();
    }

    void AnimateCameraDamage()
    {

        noiseModule.m_AmplitudeGain = ShakeAmplitude;
        noiseModule.m_FrequencyGain = ShakeFrequency;
        DOVirtual.DelayedCall(ShakeDuration, EndSHake);
        AnimateColor(AberrationIntensity);
        DOVirtual.Float(1, 0, AberartionDuration, AnimateColor).SetEase(AberrationEase).OnComplete(() => AnimateColor(0));
    }

    void AnimateColor(float value) {

        ChromaticAberration.intensity.Override(value);
    }

    void AnimateVignetter(float value)
    {
        Vignette.intensity.Override(value);
    }

    void EndSHake() {
        noiseModule.m_AmplitudeGain = 0;
        noiseModule.m_FrequencyGain = 0;
    }

    public void RemoveAgressiveEnemy(EntityController entity)
    {
        ControllerEnemies.RemoveAggresiveEnemy(entity);
        entity.isAggressive = false;
    }


}



