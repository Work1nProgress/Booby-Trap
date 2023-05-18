using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityController : StateHandler
{

    
    private Rigidbody2D _rigidbody;
    public Rigidbody2D Rigidbody => _rigidbody;

    [Header("Entity Variables")]
    [SerializeField] private float _movementSpeed;
    public float MovementSpeed => _movementSpeed;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        Initialize(this);
    }
}
