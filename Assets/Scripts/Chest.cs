using ScriptableObjects;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private Item[] m_drops;
    public void Interact()
    {
        foreach (var item in m_drops)
        {
            // test change to force linter to run
            ItemStackDrop.Spawn(new ItemStack(item, 1), transform.position + Vector3.right);
        }
    }

    public string GetActionPromptText() => "Open Chest";
}