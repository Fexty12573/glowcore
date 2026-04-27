using UnityEngine;

public class Sign : MonoBehaviour, IInteractable
{
    [SerializeField] private string m_signText;
    [SerializeField] private bool m_showFixedText;
    private string[] m_randomTexts =
    {
        "Hi!",
        "I'm a sign!",
        "Please don't chop down my trees :(",
        "AD: Wishlist GlowCore on Steam!",
        "My name is Schildfried"
    }; // When the player places a Sign, one of those texts will be displayed 
    
    public void Interact() {}

    public string GetActionPromptText() => m_signText;

    private void Start()
    {
        if (m_showFixedText)
            return;

        m_signText = m_randomTexts[Random.Range(0, m_randomTexts.Length)];
    }
}
