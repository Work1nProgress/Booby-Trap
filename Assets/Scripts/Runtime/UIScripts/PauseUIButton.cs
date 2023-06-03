using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PauseUIButton : MonoBehaviour, IMenuElement
{
    Button _button;
    Image _indicatorImage;

    [SerializeField] int _index;
    public int Index() => _index;

    UnityEvent<float> _event;
    UnityAction _onClickHandler;

    public void Initialize()
    {
        _event = new UnityEvent<float>();
        _button = GetComponent<Button>();
        _indicatorImage = transform.Find("Indicator").GetComponent<Image>();
        Deselect();

        _onClickHandler = new UnityAction(() => { _event.Invoke(0f); });
    }

    private void OnEnable()
    {
        if(_button != null)
        _button.onClick.AddListener(_onClickHandler);
    }

    private void OnDisable()
    {
        if(_button != null)
        _button.onClick.RemoveListener(_onClickHandler);
    }

    public void Activate(float value)
    {
        if(value == 0)
        _button.onClick.Invoke();
    }

    public void Deselect()
    {
        _indicatorImage.color = new Color(255f/255f, 255f/255f, 255f/255f, 0);
    }

    public void Select()
    {
        _indicatorImage.color = new Color(255f/255f, 255f/255f, 255f/255f, 255f/255f);
    }

    public UnityEvent<float> Event()
    {
        return _event;
    }
}
