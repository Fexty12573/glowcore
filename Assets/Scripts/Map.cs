using UnityEngine;

public class Map : MonoBehaviour
{
    private void OnEnable()
    {
        if (MapIsInHand())
            MapUI.Instance?.SetVisible(true);
    }

    private void OnDisable()
    {
        if (MapIsInHand())
            MapUI.Instance?.SetVisible(false);
    }

    private bool MapIsInHand() => transform.parent.name == "Hand";
}
