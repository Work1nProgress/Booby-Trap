using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContextInteractable : MonoBehaviour
{
    [SerializeField] UnityEvent<Transform> onInteract = new UnityEvent<Transform> ();
    public virtual void Interact(ContextInteractor interactor) { onInteract.Invoke(interactor.transform); }
}
