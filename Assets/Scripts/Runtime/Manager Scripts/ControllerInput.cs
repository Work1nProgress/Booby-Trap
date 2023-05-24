using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInput : GenericSingleton<ControllerInput>
{





    [HideInInspector]
    public UnityEvent<float> Horizontal = new UnityEvent<float>();
    [HideInInspector]
    public UnityEvent<float> Vertical = new UnityEvent<float>();

    [HideInInspector]
    public UnityEvent<bool> Jump = new UnityEvent<bool>();
    [HideInInspector]
    public UnityEvent Throw = new UnityEvent();
    [HideInInspector]
    public UnityEvent Attack = new UnityEvent();


    void OnAttack(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            Attack.Invoke();
        }
    }

    void OnJump(InputValue inputValue)
    {
        Jump.Invoke(inputValue.Get<float>() > 0);
    }

    void OnThrow(InputValue inputValue)
    {
        if (inputValue.Get<float>() > 0)
        {
            Throw.Invoke();
        }
    }

    void OnHorizontal(InputValue inputValue)
    {
        var horizontalInputRaw = inputValue.Get<float>();
        Horizontal.Invoke(TristateReduction(horizontalInputRaw));

    }

    void OnVertical(InputValue inputValue)
    {
        var vertInputRaw = inputValue.Get<float>();
        Vertical.Invoke(TristateReduction(vertInputRaw));

    }
    
    private int TristateReduction(float number)
    {
        return number switch
        {
            > 0.4f => 1,
            < -0.4f => -1,
            _ => 0
        };
    }
}
