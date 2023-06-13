using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image[] healthPips;
    [SerializeField] private Sprite healthPipFilled;
    [SerializeField] private Sprite healthPipEmpty;

    public void RerenderPips(int currenHealth)
    {
        for (var i = 0; i < healthPips.Length; i++)
        {
            healthPips[i].sprite = i < currenHealth - 1 
                ? healthPipFilled 
                : healthPipEmpty;
        }
    }
}
