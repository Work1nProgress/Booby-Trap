using UnityEngine.Events;
public interface IMenuElement
{
    public void Initialize();

    public int Index();

    public void Select();
    public void Deselect();

    public void Activate(float value);

    public UnityEvent<float> Event();
}
