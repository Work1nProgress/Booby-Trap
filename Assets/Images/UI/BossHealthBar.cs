using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] private Image[] healthPips;
    [SerializeField] private Sprite healthPipFilled;
    [SerializeField] private Sprite healthPipEmpty;

    
    public void RerenderPips(int health, int maxHealth)
    {
        float healthPercentage = (float)health / maxHealth;
        int filledPipsCount = Mathf.CeilToInt(healthPips.Length * healthPercentage);

        for (int i = 0; i < healthPips.Length; i++)
        {
            if (i < filledPipsCount)
            {
                healthPips[i].sprite = healthPipFilled;
            }
            else
            {
                healthPips[i].sprite = healthPipEmpty;
            }
        }
    }
}
