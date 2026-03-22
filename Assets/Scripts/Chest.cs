using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Special interact for Chest");
    }
}