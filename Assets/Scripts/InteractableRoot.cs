using System.Collections.Generic;
using UnityEngine;

public class InteractableRoot : MonoBehaviour, IInteractable
{
    private void Awake()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        HashSet<GameObject> processed = new HashSet<GameObject>();

        foreach (var col in colliders)
        {
            if (!processed.Add(col.gameObject)) continue;
            if (!col.TryGetComponent(out InteractableChild child))
            {
                child = col.gameObject.AddComponent<InteractableChild>();
            }

            child.Root = this;
        }
    }

    public void Interact()
    {
    }

    public void OnHover()
    {
    }

    public void OnHoverExit()
    {
    }
}