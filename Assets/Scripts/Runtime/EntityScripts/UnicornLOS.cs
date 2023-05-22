using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnicornLOS : MonoBehaviour
{
    [SerializeField] private int _playerLayer;
    private UnicornBot _unicornBot;

    private void Awake()
    {
        _unicornBot = GetComponentInParent<UnicornBot>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer != _playerLayer) return;
        
        _unicornBot.Enrage();
    }
}
