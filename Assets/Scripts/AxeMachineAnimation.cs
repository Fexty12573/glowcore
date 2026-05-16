using System.Collections;
using UnityEngine;

public class AxeMachineAnimation : MonoBehaviour
{
    [SerializeField] private Animator m_axeGroup1;
    [SerializeField] private Animator m_axeGroup2;
    [SerializeField] private Animator m_axeGroup3;
    [SerializeField] private float m_axeAnimationLength = 2f;
    [SerializeField] private SpinningObject m_wheel1;
    [SerializeField] private SpinningObject m_wheel2;

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

    private IEnumerator StartDesyncedAxes()
    {
        SetBreakingBools(m_axeGroup1, true);
        yield return new WaitForSeconds(m_axeAnimationLength / 3);
        SetBreakingBools(m_axeGroup2, true);
        yield return new WaitForSeconds(m_axeAnimationLength / 3);
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
