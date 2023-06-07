using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIPauseMenuSlider : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler
{
    GameObject _selectedIndicator;
    Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _selectedIndicator = transform.Find("SelectedIndicator").gameObject;
        if (_selectedIndicator != null) _selectedIndicator.SetActive(false);
    }

    public void OnSelect(BaseEventData data)
    {
        if (_selectedIndicator != null) _selectedIndicator.SetActive(true);
    }

    public void OnDeselect(BaseEventData data)
    {
        if (_selectedIndicator != null) _selectedIndicator.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _slider.Select();
    }
}
