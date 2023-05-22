using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLineOfSight : MonoBehaviour
{
    [SerializeField] private int _playerLayer;
    private ILineOfSightEntity _lineOfSightEntity;

    private void Awake()
    {
        _lineOfSightEntity = GetComponentInParent<ILineOfSightEntity>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer != _playerLayer) return;
        
        _lineOfSightEntity.EnteredLOS();
    }
}
