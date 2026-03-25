using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Item> m_drops = new();
    public void Interact()
    {
        foreach (var item in m_drops)
        {
            ItemStackDrop.Spawn(new ItemDrop()
            {
                Item = item,
                Min = 1,
                Max = 1
            }, transform.position + Vector3.right);
        }
    }
}