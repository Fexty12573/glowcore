using UnityEngine;

public class FogFollow : MonoBehaviour
{
    [SerializeField] private Transform m_player;

    [SerializeField] private float m_speed = 10f;
    [SerializeField] private float m_triggerDistance = 30f;
    [SerializeField] private FollowMode m_followMode;

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

        switch (m_followMode)
        {
            case FollowMode.CompareZ_MoveX:

                compareDistance =
                    m_player.position.z - transform.position.z;

                if (Mathf.Abs(compareDistance) > m_triggerDistance)
                    return;

                targetValue = m_player.position.x;

                pos.x = Mathf.Lerp(
                    pos.x,
                    targetValue,
                    m_speed * Time.deltaTime
                );

                break;

            case FollowMode.CompareX_MoveZ:

                compareDistance =
                    m_player.position.x - transform.position.x;

                if (Mathf.Abs(compareDistance) > m_triggerDistance)
                    return;

                targetValue = m_player.position.z;

                pos.z = Mathf.Lerp(
                    pos.z,
                    targetValue,
                    m_speed * Time.deltaTime
                );

                break;
        }

        transform.position = pos;
    }
}