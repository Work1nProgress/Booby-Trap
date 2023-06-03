using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseUISlider : MonoBehaviour, IMenuElement
{
    Slider _slider;
    Image _fillImage;

    [SerializeField] int _index;
    public int Index() => _index;

    UnityEvent<float> _event;
    UnityAction<float> _onValueChangedHandler;

    public void Initialize()
    {
        _event = new UnityEvent<float>();
        _slider = GetComponent<Slider>();
        _fillImage = _slider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();
        Deselect();

        _onValueChangedHandler = new UnityAction<float>((float value) => { _event.Invoke(value); });
    }

    private void OnEnable()
    {
        if(_slider != null)
           _slider.onValueChanged.AddListener(_onValueChangedHandler);
    }

    private void OnDisable()
    {
        if (_slider != null)
            _slider.onValueChanged.RemoveListener(_onValueChangedHandler);
    }

    public void Activate(float value)
    {
        value /= 16;
        _slider.value = Mathf.Clamp(_slider.value + value, 0f, 1f);
    }

    public void Deselect()
    {
        _fillImage.color = new Color(96f/255f, 96f/255f, 96f/255f, 255f/255f);
        
    }

    public void Select()
    {
        _fillImage.color = new Color(230f/255f, 230f/255f, 230f/255f, 255f/255f);
    }

    public UnityEvent<float> Event()
    {
        return _event;
    }
}
