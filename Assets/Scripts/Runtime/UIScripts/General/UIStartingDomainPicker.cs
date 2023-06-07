using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStartingDomainPicker : MonoBehaviour
{
    public UIDomain _startingDomain;

    private void Start()
    {
        MainMenuUIController.SetActiveUIDomain(_startingDomain);
        Destroy(this);
    }
}
