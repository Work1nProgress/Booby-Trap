using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPauseMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    GameObject _selectedIndicator;
    private void Awake()
    {
        
    }

    public void OnSelect(BaseEventData data)
    {

    }

    public void OnDeselect(BaseEventData data)
    {

    }
}
