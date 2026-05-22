using UnityEngine;

public class FogFollow : MonoBehaviour
{
    public Transform player;

    public float speed = 10f;
    public float triggerDistance = 30f;

    public FollowMode followMode;

    private float targetValue;

    public enum FollowMode
    {
        CompareZ_MoveX,
        CompareX_MoveZ
    }

    void Update()
    {
        float compareDistance = 0f;

        Vector3 pos = transform.position;

        switch (followMode)
        {
            case FollowMode.CompareZ_MoveX:

                compareDistance =
                    player.position.z - transform.position.z;

                if (Mathf.Abs(compareDistance) > triggerDistance)
                    return;

                targetValue = player.position.x;

                pos.x = Mathf.Lerp(
                    pos.x,
                    targetValue,
                    speed * Time.deltaTime
                );

                break;

            case FollowMode.CompareX_MoveZ:

                compareDistance =
                    player.position.x - transform.position.x;

                if (Mathf.Abs(compareDistance) > triggerDistance)
                    return;

                targetValue = player.position.z;

                pos.z = Mathf.Lerp(
                    pos.z,
                    targetValue,
                    speed * Time.deltaTime
                );

                break;
        }

        transform.position = pos;
    }
}