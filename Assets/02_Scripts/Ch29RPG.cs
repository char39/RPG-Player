using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ch29RPG : MonoBehaviour
{
    #region Vars

    public enum PlayerState { IDLE, ATTACK, UNDER_ATTACK, DEAD }
    public PlayerState playerState = PlayerState.IDLE;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    private Transform cameraTr;
    private Transform cameraPivotTr;
    private float cameraDistance = 0f;
    [SerializeField]private Vector3 mouseMove = Vector3.zero;
    private LayerMask playerLayer;

    private Transform charactorTr;
    private Animator ani;
    private CharacterController chCtrl;
    private Vector3 moveVelocity = Vector3.zero;

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

    private float GetAxis_H;
    private float GetAxis_V;
    private float GetAxisRaw_H;
    private float GetAxisRaw_V;

    #endregion

    void Start()
    {
        StartVars();
    }

    void Update()
    {
        InputGetAxis();
        FreezeXZ();
        CameraDistanceCtrl();
        PlayerStates();
    }

    void LateUpdate()
    {
        CameraCtrl();
    }

    private void StartVars()
    {
        cameraTr = Camera.main.transform;
        cameraPivotTr = cameraTr.parent;
        charactorTr = GetComponentsInChildren<Transform>()[1];
        ani = transform.GetChild(0).GetComponent<Animator>();
        chCtrl = GetComponent<CharacterController>();
        cameraDistance = 5f;
    }
    private void InputGetAxis()
    {
        GetAxis_H = Input.GetAxis("Horizontal");
        GetAxis_V = Input.GetAxis("Vertical");
        GetAxisRaw_H = Input.GetAxisRaw("Horizontal");
        GetAxisRaw_V = Input.GetAxisRaw("Vertical");
    }

    private void CameraCtrl()
    {
        float cameraHeight = 1.3f;
        cameraPivotTr.position = transform.position + (Vector3.up * cameraHeight);
        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 7f, Input.GetAxisRaw("Mouse X") * 7f, 0f);
        if (mouseMove.x < -40.0f)
            mouseMove.x = -40.0f;
        else if (mouseMove.x > 40.0f)
            mouseMove.x = 40.0f;
        cameraPivotTr.eulerAngles = mouseMove;

        RaycastHit hit;
        Vector3 dir = (cameraTr.position - cameraPivotTr.position).normalized;
        if (Physics.Raycast(cameraPivotTr.position, dir, out hit, cameraDistance, ~playerLayer))
        {
            Vector3 changeLocalPos = Vector3.back * hit.distance;
            cameraTr.localPosition = Vector3.Slerp(cameraTr.localPosition, changeLocalPos, Time.deltaTime * 15f);
        }
        else
        {
            Vector3 originLocalPos = Vector3.back * cameraDistance;
            cameraTr.localPosition = Vector3.Slerp(cameraTr.localPosition, originLocalPos, Time.deltaTime * 15f);
        }
    }
    private void CameraDistanceCtrl()
    {
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel");
        cameraDistance = Mathf.Clamp(cameraDistance, 2.2f, 10f);
    }
    private void FreezeXZ()
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }

    private void PlayerStates()
    {
        switch (playerState)
        {
            case PlayerState.IDLE:
                Player_Idle_Move();
                break;
            case PlayerState.ATTACK:
                AttackTimeState();
                break;
            case PlayerState.UNDER_ATTACK:

                break;

            case PlayerState.DEAD:

                break;
        }
    }
    private void Player_Idle_Move()
    {
        RunCheck();
        if (chCtrl.isGrounded)
        {
            if (!IsGrounded)
                IsGrounded = true;
            CalculatorInputMove();
            RaycastHit groundHit;
            if (GroundCheck(out groundHit))
                moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;
            else
                moveVelocity.y = -1f;
            PlayerAttack();
            PlayerShieldAttack();
        }
        else
        {
            if (IsGrounded)
                IsGrounded = false;
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }
        chCtrl.Move(moveVelocity * Time.deltaTime);

    }
    private void RunCheck()
    {
        if (!IsRun && Input.GetKey(KeyCode.LeftShift))
            IsRun = true;
        else if (IsRun && GetAxis_H == 0 && GetAxis_V == 0)
            IsRun = false;
    }
    private void CalculatorInputMove()
    {
        moveVelocity = new Vector3(GetAxisRaw_H, 0f, GetAxisRaw_V).normalized * (IsRun ? runSpeed : walkSpeed);
        ani.SetFloat(hashSpeedX, GetAxis_H);
        ani.SetFloat(hashSpeedY, GetAxis_V);
        moveVelocity = transform.TransformDirection(moveVelocity);
        if (moveVelocity.sqrMagnitude > 0.01f)
        {
            Quaternion cameraRot = cameraPivotTr.rotation;
            cameraRot.x = cameraRot.z = 0f;
            transform.rotation = cameraRot;
            if (IsRun)
            {
                Quaternion charRot = Quaternion.LookRotation(moveVelocity);
                charRot.x = charRot.z = 0f;
                charactorTr.rotation = Quaternion.Slerp(charactorTr.rotation, charRot, Time.deltaTime * 10f);
            }
            else
                charactorTr.rotation = Quaternion.Slerp(charactorTr.rotation, cameraRot, Time.deltaTime * 10f);
        }
    }
    private bool GroundCheck(out RaycastHit hit)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit, 0.25f);
    }

    private float nextTime = 0f;
    private void AttackTimeState()
    {
        nextTime += Time.deltaTime;
        if (nextTime >= 1.2f)
        {
            playerState = PlayerState.IDLE;
        }
    }
    private void PlayerAttack()
    {
        if (Input.GetButtonDown("Fire1"))   // Left Ctrl, Left Mouse Down
        {
            playerState = PlayerState.ATTACK;
            ani.SetTrigger(hashAttack);
            ani.SetFloat(hashSpeedX, 0f);
            ani.SetFloat(hashSpeedY, 0f);
            nextTime = 0f;
        }
    }
    private void PlayerShieldAttack()
    {
        if (Input.GetButtonDown("Fire2"))   // Left Alt, Right Mouse Down
        {
            playerState = PlayerState.ATTACK;
            ani.SetTrigger(hashShieldAttack);
            ani.SetFloat(hashSpeedX, 0f);
            ani.SetFloat(hashSpeedY, 0f);
            nextTime = 0f;
        }
    }



    private void OnDrawGizmos()
    {
        if (!cameraTr || !cameraPivotTr)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPivotTr.position, (cameraTr.position - cameraPivotTr.position).normalized * cameraDistance);   // 카메라 Pivot에서 카메라까지의 Ray
        Gizmos.color = Color.green;
        Gizmos.DrawLine(cameraTr.position, cameraPivotTr.position);                                                         // 카메라에서 카메라 Pivot까지의 벡터
    }

}