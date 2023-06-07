using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControllerSingleton<T> : GenericSingleton<T> where T : UIControllerSingleton<T>
{
    //After inheriting from this class, be sure to add to the UIDomain enum at the bottom of this document.
    //Instructions on how to do so are commented in the enum itself.


    bool _isActive = true;
    public bool isActive => _isActive;
    protected static void SetActive(bool active)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"Warning: could not set active, UI controller does not exist!");
            return;
        }

        Instance._isActive = active;
        Instance.OnActivate();
    }
    public static void SetActiveUIDomain(UIDomain domain)
    {
        //deactivation list
        MainMenuUIController.CloseMenu();
        MainMenuUIController.SetActive(false);
        InGameUIController.CloseMenu();
        InGameUIController.SetActive(false);

        //activation switch
        switch (domain)
        {
            case UIDomain.MAINMENU:
                MainMenuUIController.SetActive(true);
                return;
            case UIDomain.INGAME:
                InGameUIController.SetActive(true);
                return;
        }
    }
    protected virtual void OnActivate() { }


    //these methods can be called at any time.
    private Dictionary<string, GenericMenuHandler> _menus;
    public static GenericMenuHandler Menu(string key)
    {
        if(Instance == null)
        {
            Debug.LogWarning($"Warning: menu '{key}' could not be retrieved, UI controller does not exist!");
            return null;
        }

        GenericMenuHandler menu;
        bool menuExists = Instance._menus.TryGetValue(key, out menu);
        if (!menuExists) Debug.LogWarning($"Warning: menu key '{key}' does not exist in the menu dictionary!");

        return menuExists ? menu : null;
    }
    public static void RegisterMenu(string key, GenericMenuHandler menu, bool openOnRegister = false)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"Warning: menu '{key}' could not be added, UI controller does not exist!");
            return;
        }

        Instance._menus.Add(key, menu);
        CloseMenu(key);

        if (openOnRegister) OpenMenu(key);
    }




    //these methods can only be called if _isActive == true
    private GenericMenuHandler _openMenu;
    public static void OpenMenu(string key)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"Warning: menu '{key}' could not be opened, UI controller does not exist!");
            return;
        }
        if (!Instance._isActive) return;

        if (Instance._openMenu != null)
        {
            Instance._openMenu.OnClose();
            Instance._openMenu.gameObject.SetActive(false);
        }

        Instance._openMenu = Menu(key);
        if (Instance._openMenu != null)
        {
            Instance._openMenu.gameObject.SetActive(true);
            Instance._openMenu.OnOpen();
        }
    }
    public static void CloseMenu()
    {
        if (Instance == null)
        {
            Debug.LogWarning($"Warning: menu could not be closed, UI controller does not exist!");
            return;
        }
        if (!Instance._isActive) return;

        if (Instance._openMenu != null)
        {
            Instance._openMenu.OnClose();
            Instance._openMenu.gameObject.SetActive(false);
        }
            

        Instance._openMenu = null;
    }
    public static void CloseMenu(string key)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"Warning: menu {key} could not be closed, UI controller does not exist!");
            return;
        }
        if (!Instance._isActive) return;

        GenericMenuHandler menu = Menu(key);

        if (menu != null)
        {
            menu.OnClose();
            menu.gameObject.SetActive(false);
        }
    }
    public static void ToggleMenu(string key)
    {
        if (Instance == null)
        {
            Debug.LogWarning($"Warning: menu {key} could not be toggled, UI controller does not exist!");
            return;
        }
        if (!Instance._isActive) return;

        if (Instance._openMenu != null)
        {
            if (Instance._openMenu.key == key)
                CloseMenu();
            else
            {
                CloseMenu();
                OpenMenu(key);
            }
        }
        else
            OpenMenu(key);
    }




    protected override void Awake()
    {
        base.Awake();
        _menus = new Dictionary<string, GenericMenuHandler>();
    }
}


public enum UIDomain
{
/*
(use ctrl + f to search for the terms in single quotes, make sure you're in the right document though.)
 
Checklist for adding new UIDomain:
    1. Add to the 'deactivation list' in SetActiveUIDomain, which is in UIControllerSingleton (this file).
    2. Add new case to the 'activation switch' in SetActiveUIDomain, which is in UIControllerSingleton (this file).
    3. Add new case to 'registration switch' in RegisterToController, which is in GenericMenuHandler.

 */

    MAINMENU,
    INGAME
}