using System.Collections;
using UnityEngine;

public class AxeMachineAnimation : MonoBehaviour
{
    [Header("Axes")]
    [SerializeField] private Animator m_axeGroup1;
    [SerializeField] private Animator m_axeGroup2;
    [SerializeField] private Animator m_axeGroup3;
    [SerializeField] private float m_axeAnimationBaseLength = 2f;
    [SerializeField] private float m_axeAnimationSpeed = 1f;

    [Header("Wheels")]
    [SerializeField] private SpinningObject m_wheel1;
    [SerializeField] private SpinningObject m_wheel2;
    [SerializeField] private float m_wheelSpinningSpeed = 80f;

    private static readonly int s_breakingHash = Animator.StringToHash("Breaking");
    private static readonly int s_idleHash = Animator.StringToHash("Idle");

    public void SetIdleAnimation()
    {
        SetWheels(false);
        StopAllCoroutines();
        SetBreakingBools(m_axeGroup1, false);
        SetBreakingBools(m_axeGroup2, false);
        SetBreakingBools(m_axeGroup3, false);
    }

    public void SetBreakAnimation()
    {
        SetWheels(false);
        StartCoroutine(StartDesyncedAxes());
    }

    public void SetMoveAnimation()
    {
        SetWheels(true);
        SetBreakingBools(m_axeGroup1, false);
        SetBreakingBools(m_axeGroup2, false);
        SetBreakingBools(m_axeGroup3, false);
    }

    private void Start()
    {
        m_axeGroup1.speed = m_axeAnimationSpeed;
        m_axeGroup2.speed = m_axeAnimationSpeed;
        m_axeGroup3.speed = m_axeAnimationSpeed;

        var speedVector = new Vector3(-m_wheelSpinningSpeed, 0f, 0f);
        m_wheel1.m_spinSpeed = speedVector;
        m_wheel2.m_spinSpeed = speedVector;
    }

    private IEnumerator StartDesyncedAxes()
    {
        SetBreakingBools(m_axeGroup1, true);
        yield return new WaitForSeconds(m_axeAnimationBaseLength / m_axeAnimationSpeed / 3);
        SetBreakingBools(m_axeGroup2, true);
        yield return new WaitForSeconds(m_axeAnimationBaseLength / 3);
        SetBreakingBools(m_axeGroup3, true);
    }

    private void SetBreakingBools(Animator animator, bool on)
    {
        animator.SetBool(s_breakingHash, on);
        animator.SetBool(s_idleHash, !on);
    }

    private void SetWheels(bool on)
    {
        m_wheel1.enabled = on;
        m_wheel2.enabled = on;
    }
}
