using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "DaddySound", menuName = "Entities/Daddy/Daddy Sound")]
public class DaddySound : ScriptableObject
{

    [Header("Common")]
    public string TeleportIn;
    public string TeleportOut;
    public string Hurt;

    [Header("Bulldozer")]
    public string Bulldozer;

    [Header("Slash")]
    public string SlashCharge;
    public string SlashAttack;

    [Header("Lightning")]
    public string LightningChannel;
    public string LightningStrike;
    public string LightningCharge;

    [Header("Walls")]
    public string WallsStart;
    public string WallsEnd;

    [Header("Mines")]
    public string MineArmed;



}
