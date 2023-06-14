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
    Vector3 m_StartingPlayerPos;

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

    int savedRoom = 1;
    public int SavedRoom => savedRoom;
    

    #endregion

    // Use this method to initialize everyhing you need at the begging of the scene
    public override void Init()
    {
        base.Init();
        m_Instance = this;

        player = FindFirstObjectByType<Player>();
        Daddy = FindFirstObjectByType<DaddyController>();
        player.Init(new EntityStats
        {
            MaxHealth = MaxPlayerHealth

        });
        if (playerHealthBar)
        {
            playerHealthBar.RerenderPips(MaxPlayerHealth, MaxPlayerHealth);
        }
        
        m_StartingPlayerPos = player.transform.position;

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
        player.Heal(MaxPlayerHealth);
        player.transform.position = m_StartingPlayerPos;
        UpdatePlayerHealth(0);
        playerHealthBar.RerenderPips(MaxPlayerHealth, MaxPlayerHealth);
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



