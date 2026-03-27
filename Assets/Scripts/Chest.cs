using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private Item[] m_drops;
    public void Interact()
    {
        foreach (var item in m_drops)
        {
            ItemStackDrop.Spawn(ItemStack.Create(item, 1), transform.position + Vector3.right);
        }
    }
}