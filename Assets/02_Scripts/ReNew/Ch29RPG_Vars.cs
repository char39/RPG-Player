using Cinemachine;
using UnityEngine;

public partial class Ch29RPG
{
    private InputSystemManage inputs;

    public enum PlayerState { IDLE, ATTACK, UNDER_ATTACK, DEAD }
    public PlayerState playerState = PlayerState.IDLE;

    internal float walkSpeed = 5f;
    internal float runSpeed = 10f;

    private Transform cameraTr;
    private Transform cameraPivotTr;
    private float cameraDistance = 0f;
    private Vector3 mouseMove = Vector3.zero;
    private LayerMask playerLayer;

    private Transform charactorTr;
    private Animator ani;
    private CharacterController chCtrl;
    private Vector3 moveVelocity = Vector3.zero;

    public CinemachineFreeLook FreeLookCam;

    private bool isGrounded = false;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            isGrounded = value;
            ani.SetBool("isGrounded", value);
        }
    }
    private bool isRun;
    public bool IsRun
    {
        get { return isRun; }
        set
        {
            isRun = value;
            ani.SetBool("isRun", value);
        }
    }
    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        set
        {
            isDead = value;
            ani.SetBool("isDead", value);
        }
    }

    private readonly int hashAttack = Animator.StringToHash("swordAttackTrigger");
    private readonly int hashShieldAttack = Animator.StringToHash("shieldAttackTrigger");
    private readonly int hashSpeedX = Animator.StringToHash("speedX");
    private readonly int hashSpeedY = Animator.StringToHash("speedY");
    private float Move_H;
    private float Move_V;
    private float MoveRaw_H;
    private float MoveRaw_V;
    private float MouseMove_H;
    private float MouseMove_V;
    private float MouseScroll;
    private bool Fire1;
    private bool Fire2;
    private bool Fire1Trigger = false;
    private bool Fire2Trigger = false;
    private bool RunButton;
}
