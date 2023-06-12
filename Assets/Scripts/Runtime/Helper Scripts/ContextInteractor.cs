using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextInteractor : MonoBehaviour
{
    ContextInteractable _interactable;

    private void FixedUpdate() => FindInteractable();

    private void Awake()
    {
        ControllerInput.Instance.Interact.AddListener(() => { Interact(); });
    }

    protected virtual void FindInteractable()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, new Vector2(3f, 3f), 0f, LayerMask.GetMask("Interactable"));
        if (collider != null)
            _interactable = collider.GetComponent<ContextInteractable>();
    }

    protected virtual void Interact()
    {
        if (_interactable != null)
            _interactable.Interact(this);
    }
}
