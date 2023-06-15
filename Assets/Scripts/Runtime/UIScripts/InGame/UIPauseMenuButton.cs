using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIPauseMenuButton : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    GameObject _selectedIndicator;
    TextMeshProUGUI _buttonText;
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _selectedIndicator = transform.Find("SelectedIndicator").gameObject;
        if (_selectedIndicator != null) _selectedIndicator.SetActive(false);

        _buttonText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

    public void OnSelect(BaseEventData data)
    {
        if (_selectedIndicator != null) _selectedIndicator.SetActive(true);
        if (_buttonText != null) _buttonText.fontStyle = FontStyles.Underline;
    }

    public void OnDeselect(BaseEventData data)
    {
        if (_selectedIndicator != null) _selectedIndicator.SetActive(false);
        if (_buttonText != null) _buttonText.fontStyle = FontStyles.Normal;
    }

    public void OnSubmit(BaseEventData data)
    {
        //OnDeselect(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _button.Select();
    }
}