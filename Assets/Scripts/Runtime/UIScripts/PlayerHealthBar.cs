using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image[] healthPips;
    [SerializeField] private Sprite healthPipFilled;
    [SerializeField] private Sprite healthPipEmpty;

    private int _maxHealth;
    private int _currentHealth;
    
    public void setMaxHealth(int maxHealth)
    {
        this._maxHealth = maxHealth;
        _currentHealth = this._maxHealth;
        RerenderPips();
    }
    
    public void updateHealth(int amount)
    {
        _currentHealth += amount;
        RerenderPips();
    }

    private void RerenderPips()
    {
        for (int i = 0; i < healthPips.Length; i++)
        {
            if (i < _currentHealth)
            {
                healthPips[i].sprite = healthPipFilled;
            }
            else
            {
                healthPips[i].sprite = healthPipEmpty;
            }
        }
    }

    public void resetHealth()
    {
        _currentHealth = _maxHealth;
        RerenderPips();
    }
}
