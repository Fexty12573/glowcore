using System;
using System.Linq;
using GlowCore.World;
using ScriptableObjects;
using UnityEngine;

// When savefile loads, all AxeMachines will start in the Idle State regardless of what the state was when saving.
// When the game saves during movement, the lexicographical lower tile will be saved
// This can't produce conflicts, since the machine owns both tiles during movement.
public class AxeMachine : MonoBehaviour
{
    public enum MachineState
    {
        Idle,
        Breaking,
        Moving,
        Rotating
    }

    [SerializeField] private Tool m_tool;
    [SerializeField] private float m_movementSpeed = 2f;
    [SerializeField] private float m_rotationSpeed = 40f;
    [SerializeField] private Node m_machineNode;
    [SerializeField] private Chest m_storage;
    [SerializeField] private AxeMachineAnimation m_animation;

    private MachineState m_state = MachineState.Idle;
    private Node m_targetNode;
    private Vector2Int m_position;
    private Vector2Int m_lastPosition;
    // private bool m_isRotatingRight; // false means that he rotates left //todo delete

    private void Start()
    {
        m_position = new Vector2Int(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.z));
    }

    private void OnDestroy()
    {
        if (m_state == MachineState.Breaking)
            m_targetNode.EndHold();
    }

    private void Update()
    {
        switch (m_state)
        {
            case MachineState.Idle:
                UpdateTargetNode();
                if (m_targetNode == null || m_targetNode.MarkedForDeletion)
                {
                    if (TryStartMovingForward())
                    {
                        m_state = MachineState.Moving;
                        m_animation.SetMoveAnimation();
                    }

                    return;
                }

                if (TryStartRotating())
                {
                    m_state = MachineState.Rotating;
                    m_animation.SetRotateAnimation();
                    return;
                }

                if (m_storage.HasEmptySlot() && TryStartBreakingTargetNode()) //only break node if the storage has enough space
                {
                    m_state = MachineState.Breaking;
                    m_animation.SetBreakAnimation();
                }
                break;
            case MachineState.Breaking:
                if (IsNodeBroken())
                {
                    EndBreaking();
                    m_state = MachineState.Idle;
                    m_animation.SetIdleAnimation();
                    Update(); // makes that the axe machine immediately claims the tile that it just freed
                    return;
                }
                UpdateBreakingNode();
                break;
            case MachineState.Moving:
                ContinueMovingForward();
                if (TryFinishMovingForward())
                {
                    m_state = MachineState.Idle;
                    m_animation.SetIdleAnimation();
                    Update(); // prevents the machine from stoping moving for one frame if he can drive again
                }
                break;
            case MachineState.Rotating:
                ContinueRotating();
                if (TryFinishRotating())
                {
                    m_state = MachineState.Idle;
                    m_animation.SetIdleAnimation();
                    Update();
                }
                break;
        }
    }

    private void UpdateTargetNode()
    {
        Vector2Int targetPosition = m_position + Node.RotationToDirectionVector2Int(m_machineNode.Rotation);
        m_targetNode = WorldGrid.Instance.GetNodeAt(targetPosition);
    }

    private bool TryStartBreakingTargetNode()
    {
        //Machine can't break anything if it's not in the border to prevent IndexOutOfBoundsException (shouldn't be possible anyway)
        if (!WorldGrid.Instance.IsInBounds(GetLastWorldPosition()))
            return false;

        if (m_targetNode.MarkedForDeletion)
            return false;

        if (m_targetNode.NodeData.UsableTools.All(t => t.Tool != m_tool))
            return false;

        if (m_targetNode.IsHolding) //check if someone else is already breaking this node
            return false;

        m_targetNode.StartHold(m_tool, m_storage);
        return true;
    }

    private void UpdateBreakingNode()
    {
        m_targetNode.UpdateHold(Time.deltaTime);
    }

    private bool IsNodeBroken()
    {
        return m_targetNode == null || m_targetNode.MarkedForDeletion;
    }

    private void EndBreaking()
    {
        m_targetNode = null;
    }

    private bool TryStartMovingForward()
    {
        //Machine can't move if it's not in the border to prevent IndexOutOfBoundsException (shouldn't be possible anyway)
        if (!WorldGrid.Instance.IsInBounds(GetLastWorldPosition()))
            return false;

        Vector2Int newPosition = m_position + Node.RotationToDirectionVector2Int(m_machineNode.Rotation);
        if (!WorldGrid.Instance.PlaceNodeAt(m_machineNode, newPosition.x, newPosition.y))
            return false;

        m_lastPosition = m_position;
        m_position = newPosition;
        return true;
    }

    private void ContinueMovingForward()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            new Vector3(m_position.x, 0, m_position.y),
            Time.deltaTime * m_movementSpeed);
    }

    private bool TryFinishMovingForward()
    {
        Vector3 targetPosition = new(m_position.x, 0, m_position.y);
        if (Vector3.Distance(transform.position, targetPosition) > 0.0001f)
            return false;

        transform.position = targetPosition; // snap to new target position

        if (m_machineNode.TilesUsed.Remove(GetLastWorldPosition())) // free the old tile only if it was registered
            WorldGrid.Instance.ClearNodeAt(GetLastWorldPosition());

        return true;
    }

    private bool TryStartRotating()
    {
        if (m_targetNode.SourceBlock is null)
            return false;

        switch (m_targetNode.SourceBlock.Name)
        {
            case "Rotator Right":
                m_machineNode.Rotation = (BlockRotation)(((int)m_machineNode.Rotation + 1) % 4); // set rotation 90 degrees to the right
                break;
            case "Rotator Down":
                m_machineNode.Rotation = (BlockRotation)(((int)m_machineNode.Rotation + 2) % 4); // set rotation 180 degrees to the right
                break;
            case "Rotator Left":
                m_machineNode.Rotation = (BlockRotation)(((int)m_machineNode.Rotation + 3) % 4); // set rotation 270 degrees to the right
                break;
            default:
                return false;
        }
        return true;
    }

    private void ContinueRotating()
    {
        float targetY = GetTargetYRotation();
        float currentY = transform.eulerAngles.y;

        float newY = Mathf.MoveTowardsAngle(currentY, targetY, Time.deltaTime * m_rotationSpeed);
        Vector3 newRotation = transform.eulerAngles;
        newRotation.y = newY;
        transform.eulerAngles = newRotation;
    }

    private bool TryFinishRotating()
    {
        float targetY = GetTargetYRotation();

        if (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetY)) > 0.0001f)
            return false;

        transform.eulerAngles = GetTargetRotation(); // snap
        return true;
    }

    private Vector2Int GetLastWorldPosition()
    {
        return WorldGrid.Instance.WorldToGrid(new Vector3(m_lastPosition.x, 0, m_lastPosition.y));
    }

    private float GetTargetYRotation()
    {
        return Node.BlockRotationToDegrees(m_machineNode.Rotation) + m_machineNode.SourceBlock.YRotationOffset;
    }

    private Vector3 GetTargetRotation()
    {
        return new Vector3(
            transform.eulerAngles.x,
            GetTargetYRotation(),
            transform.eulerAngles.z);
    }
}
