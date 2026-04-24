using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{
    [SerializeField] private string m_signText;

    public void Interact() {}

    public string GetActionPromptText() => m_signText;
}
