using UnityEngine;

public class FogFollow : MonoBehaviour
{
    [SerializeField] private Transform m_player;

    [SerializeField] private float m_speed = 10f;
    [SerializeField] private float m_triggerDistance = 30f;
    [SerializeField] private float m_activateDistance = 10f; // How far away must the player be away from 0 0 so that the follow activates
    [SerializeField] private FollowMode m_followMode;

    private float m_targetValue;

    public enum FollowMode
    {
        CompareZ_MoveX,
        CompareX_MoveZ
    }

    private void Update()
    {
        if (Vector2.Distance(new (m_player.position.x, m_player.position.z), Vector2.zero) < m_activateDistance)
            return;

        var compareDistance = 0f;

        Vector3 pos = transform.position;

        switch (m_followMode)
        {
            case FollowMode.CompareZ_MoveX:

                compareDistance =
                    m_player.position.z - transform.position.z;

                if (Mathf.Abs(compareDistance) > m_triggerDistance)
                    return;

                m_targetValue = m_player.position.x;

                pos.x = Mathf.Lerp(
                    pos.x,
                    m_targetValue,
                    m_speed * Time.deltaTime
                );

                break;

            case FollowMode.CompareX_MoveZ:

                compareDistance =
                    m_player.position.x - transform.position.x;

                if (Mathf.Abs(compareDistance) > m_triggerDistance)
                    return;

                m_targetValue = m_player.position.z;

                pos.z = Mathf.Lerp(
                    pos.z,
                    m_targetValue,
                    m_speed * Time.deltaTime
                );

                break;
        }

        transform.position = pos;
    }
}