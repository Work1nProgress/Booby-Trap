using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseUIHandler : MonoBehaviour
{
    PauseUISlider[] _sliders;
    PauseUIButton[] _buttons;

    List<IMenuElement> _menuElements;

    IMenuElement _selectedElement;
    int _selectedIndex;

    UnityAction<float> _navigationHandler;
    UnityAction<float> _sliderActivationHandler;
    UnityAction<bool> _buttonActivationHandler;

    private void Awake()
    {
        _sliders = FindObjectsByType<PauseUISlider>(FindObjectsSortMode.InstanceID);
        _buttons = FindObjectsByType<PauseUIButton>(FindObjectsSortMode.InstanceID);

        _menuElements = SortedMenuElements();
        foreach (IMenuElement elem in _menuElements)
            elem.Initialize();


        _navigationHandler = new UnityAction<float>((float value) => {
            if (value > 0.4f)
                SelectPreviousItem();
            if (value < -0.4f)
                SelectNextItem();
        });

        _sliderActivationHandler = new UnityAction<float>((float value) =>
        {
            if (value != 0)
                _selectedElement.Activate(value);
        });

        _buttonActivationHandler = new UnityAction<bool>((bool value) =>
        {
            _selectedElement.Activate(0);
        });

        foreach (IMenuElement elem in _menuElements)
            elem.Event().AddListener((float value) => { Debug.Log($"{elem}: Activated"); });
    }

    private void OnEnable()
    {

    }

    public void SetInactive()
    {
        ControllerInput.Instance.Vertical.RemoveListener(_navigationHandler);
        ControllerInput.Instance.Horizontal.RemoveListener(_sliderActivationHandler);
        ControllerInput.Instance.Jump.RemoveListener(_buttonActivationHandler);
    }

    public void SetActive()
    {
        _selectedIndex = 0;
        _selectedElement = _menuElements[_selectedIndex];
        _selectedElement.Select();

        ControllerInput.Instance.Vertical.AddListener(_navigationHandler);
        ControllerInput.Instance.Horizontal.AddListener(_sliderActivationHandler);
        ControllerInput.Instance.Jump.AddListener(_buttonActivationHandler);
    }

    private void SelectNextItem()
    {
        if (_selectedElement != null)
            _selectedElement.Deselect();

        _selectedIndex = Utils.Wrap(_selectedIndex + 1, 0, _menuElements.Count);

        _selectedElement = _menuElements[_selectedIndex];
        if (_selectedElement != null)
            _selectedElement.Select();
    }

    private void SelectPreviousItem()
    {
        if (_selectedElement != null)
            _selectedElement.Deselect();

        _selectedIndex = Utils.Wrap(_selectedIndex - 1, 0, _menuElements.Count);

        _selectedElement = _menuElements[_selectedIndex];
        if (_selectedElement != null)
            _selectedElement.Select();
    }

    private List<IMenuElement> SortedMenuElements()
    {
        int length = _sliders.Length + _buttons.Length;
        List<IMenuElement> menuElements = new List<IMenuElement>();
        for(int i = 0; i < _sliders.Length; i++)
        {
            menuElements.Add(_sliders[i]);
        }
        for (int i = 0; i < _buttons.Length; i++)
        {
            menuElements.Add(_buttons[i]);
        }

        return menuElements.OrderBy(element => element.Index()).ToList();
    }

}
