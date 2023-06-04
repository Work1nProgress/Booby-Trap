using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ControllerGame : ControllerLocal
{

    static ControllerGame m_Instance;
    public static ControllerGame Instance => m_Instance;



    public int MaxPlayerHealth;

    public Player player;
    Vector3 m_StartingPlayerPos;

    [SerializeField]
    TMP_Text LabelHealth;

    [SerializeField]
    ControllerEnemies ControllerEnemies;


    #region Damage Animation
    CinemachineVirtualCamera vCam;

    CinemachineBasicMultiChannelPerlin noiseModule;

    ChromaticAberration ChromaticAberration;
    Volume volume;

    [SerializeField]
    float AberartionDuration, ShakeDuration, AberrationIntensity, ShakeAmplitude, ShakeFrequency;

    [SerializeField]
    Ease AberrationEase;
    

    #endregion

    // Use this method to initialize everyhing you need at the begging of the scene
    public override void Init()
    {
        base.Init();
        m_Instance = this;

        player = FindFirstObjectByType<Player>();
        player.Init(new EntityStats
        {
            MaxHealth = MaxPlayerHealth

        });
        m_StartingPlayerPos = player.transform.position;

        player.OnChangeHealth.AddListener(UpdatePlayerHealth);
        player.OnDeath.AddListener(OnPlayerDeath);
        UpdatePlayerHealth(0);
        ControllerEnemies.Init();

        vCam = FindObjectOfType<CinemachineVirtualCamera>();
        if (vCam != null)
        {
            noiseModule = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }


        var volume = FindObjectOfType< Volume>();
        volume.profile.TryGet(out ChromaticAberration);
        
    }

    //move this in some kind of spear controller script
    [HideInInspector]
    public List<Spear> Spears = new List<Spear>();

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

        LabelHealth.text = $"Health: {player.Health}";
    }

    public void OnPlayerDeath(){
        player.Heal(MaxPlayerHealth);
        player.transform.position = m_StartingPlayerPos;
        UpdatePlayerHealth(0);
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



