using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContextInteractable : MonoBehaviour
{
    [SerializeField] UnityEvent onInteract = new UnityEvent ();
    [SerializeField] UnityEvent onInteractableFound = new UnityEvent();

    public virtual void Interact(ContextInteractor interactor) { onInteract.Invoke(); }

    public virtual void InteractorNear(ContextInteractor interactor) { onInteractableFound.Invoke(); }
}
