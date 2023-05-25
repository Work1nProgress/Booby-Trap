using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ControllerGame : ControllerLocal
{

    static ControllerGame m_Instance;
    public static ControllerGame Instance => m_Instance;



    public Player player;
    Vector3 m_StartingPlayerPos;

    [SerializeField]
    TMP_Text LabelHealth;

    // Use this method to initialize everyhing you need at the begging of the scene
    public override void Init()
    {

        base.Init();
        m_Instance = this;

        player = FindFirstObjectByType<Player>();
        m_StartingPlayerPos = player.transform.position;

        player.OnChangeHealth.AddListener(UpdatePlayerHealth);
        player.OnDeath.AddListener(OnPlayerDeath);
        UpdatePlayerHealth();
    }

    //move this in some kind of spear controller script
    [HideInInspector]
    public List<Spear> Spears = new List<Spear>();

    public void RemoveSpear(int index = 0)
    {
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

    public void UpdatePlayerHealth()
    {

        LabelHealth.text = $"Health: {player.Health}";
    }

    public void OnPlayerDeath(){
        player.Heal(3);
        player.transform.position = m_StartingPlayerPos;
        UpdatePlayerHealth();
     }


}



