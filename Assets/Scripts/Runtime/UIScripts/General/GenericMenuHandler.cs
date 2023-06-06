using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GenericMenuHandler : MonoBehaviour
{
    [SerializeField] private string _menuKey;
    public string key => _menuKey;
    [SerializeField] private UIDomain _domain;
    [Tooltip("NOTE: all menus will close when registered to their respective controllers and only one menu may be open at a time.")]
    [SerializeField] private bool _openOnRegister;
    [SerializeField] Selectable _initialSelectable;


    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        RegisterToController(_domain);
    }

    public virtual void OnOpen()
    {
        _initialSelectable.Select();
    }

    public virtual void OnClose()
    {

    }

    private void RegisterToController(UIDomain domain)
    {
        //registration switch
        switch (domain)
        {
            case UIDomain.MAINMENU:
                MainMenuUIController.RegisterMenu(_menuKey, this, _openOnRegister);
                return;
            case UIDomain.INGAME:
                InGameUIController.RegisterMenu(_menuKey, this, _openOnRegister);
                return;
        }
    }
}