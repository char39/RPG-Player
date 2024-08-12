using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerRPG : MonoBehaviour
{
    #region Vars

    public enum PlayerState { IDLE, ATTACK, UNDER_ATTACK, DEAD }
    public PlayerState playerState = PlayerState.IDLE;

    [Tooltip("Walk Speed")] public float walkSpeed = 5f;
    [Tooltip("Run Speed")] public float runSpeed = 10f;

    [Header("Camera Vars")]
    [SerializeField] private Transform cameraTr;                // 카메라 위치
    [SerializeField] private Transform cameraPivotTr;           // 카메라 Pivot 위치
    [SerializeField] private float cameraDistance = 0f;         // 카메라와의 거리.         스칼라
    [SerializeField] private Vector3 mouseMove = Vector3.zero;  // 마우스 이동 벡터         방향 + 스칼라
    [SerializeField] private LayerMask playerLayer;             // 플레이어 레이어
    //               private int playerLayer;                   // layer index = 10
    [Header("PlayerMove Vars")]
    [SerializeField] private Transform modelTr;                     // 플레이어의 자식 오브젝트인 모델 위치
    [SerializeField] private Animator ani;
    [SerializeField] private CharacterController chCtrl;
    [SerializeField] private Vector3 moveVelocity = Vector3.zero;   // 플레이어 이동 벡터

    [SerializeField] private bool isGrounded = false;
    public bool IsGrounded
    {
        get { return isGrounded; }
        set
        {
            isGrounded = value;
            ani.SetBool("isGrounded", value);
        }
    }
    [SerializeField] private bool isRun;
    public bool IsRun
    {
        get { return isRun; }
        set
        {
            isRun = value;
            ani.SetBool("isRun", value);
        }
    }
    [SerializeField] private bool isDead;
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
        modelTr = GetComponentsInChildren<Transform>()[1];
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

    private void CameraCtrl()                           // 카메라 컨트롤
    {
        float cameraHeight = 1.3f;
        cameraPivotTr.position = transform.position + (Vector3.up * cameraHeight);
        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 8f, Input.GetAxisRaw("Mouse X") * 8f, 0f);
        if (mouseMove.x < -40.0f)
            mouseMove.x = -40.0f;
        else if (mouseMove.x > 40.0f)
            mouseMove.x = 40.0f;
        cameraPivotTr.eulerAngles = mouseMove;

        RaycastHit hit;
        Vector3 dir = (cameraTr.position - cameraPivotTr.position).normalized;                      // 방향 벡터
        if (Physics.Raycast(cameraPivotTr.position, dir, out hit, cameraDistance, ~playerLayer))
            cameraTr.localPosition = Vector3.back * hit.distance * 0.8f;
        else
            cameraTr.localPosition = Vector3.back * cameraDistance;
    }
    private void CameraDistanceCtrl()                   // 카메라 거리 조절
    {
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel");
        cameraDistance = Mathf.Clamp(cameraDistance, 2.2f, 10f);
    }
    private void FreezeXZ()                             // X, Z축 회전값 고정
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
    
    private void PlayerStates()                         // IDLE, ATTACK, UNDER_ATTACK, DEAD
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
    private void Player_Idle_Move()                         // IDLE 상태에서 이동
    {
        RunCheck();
        if (chCtrl.isGrounded)                                  // 플레이어가 땅에 닿았으면
        {
            if (!IsGrounded)                                        // 이전 상태가 땅에 닿지 않았으면
                IsGrounded = true;                                      // 땅에 닿은 상태로 변경
            CalculatorInputMove();                                  // 이동 계산
            RaycastHit groundHit;                                   // 땅에 닿은 정보
            if (GroundCheck(out groundHit))                         // 땅에 닿았으면
                moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;        // 이동 속도 y를 설정. 달리기 상태이면 runSpeed, 걷기 상태이면 walkSpeed
            else                                                    // 땅에 닿지 않았으면
                moveVelocity.y = -1f;                                   // 이동 속도 y를 -1로 설정
            PlayerAttack();
            PlayerShieldAttack();
        }
        else                                                    // 플레이어가 땅에 닿지 않았으면
        {
            if (IsGrounded)                                         // 이전 상태가 땅에 닿았으면
                IsGrounded = false;                                     // 땅에 닿지 않은 상태로 변경
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;   // 중력 적용
        }
        chCtrl.Move(moveVelocity * Time.deltaTime);             // 캐릭터 컨트롤러 이동

    }
    private void RunCheck()                                     // 달리기 상태 체크
    {
        if (!IsRun && Input.GetKey(KeyCode.LeftShift))              // 달리기 상태가 아니고, Shift 키를 눌렀으면
            IsRun = true;                                               // 달리기 상태로 변경
        else if (IsRun && GetAxis_H == 0 && GetAxis_V == 0)         // 달리기 상태이고, 이동키를 뗐으면
            IsRun = false;                                              // 걷기 상태로 변경
    }
    private void CalculatorInputMove()                          // 이동 계산
    {
        moveVelocity = new Vector3(GetAxisRaw_H, 0f, GetAxisRaw_V).normalized * (IsRun ? runSpeed : walkSpeed);     // 이동 벡터
        ani.SetFloat(hashSpeedX, GetAxis_H);
        ani.SetFloat(hashSpeedY, GetAxis_V);
        moveVelocity = transform.TransformDirection(moveVelocity);                                      // 이동 벡터를 카메라 기준으로 변환
        if (moveVelocity.sqrMagnitude > 0.01f)                                                          // 이동 벡터가 0.01보다 크다면
        {
            Quaternion cameraRot = cameraPivotTr.rotation;                                                  // 카메라 Pivot의 회전값
            cameraRot.x = cameraRot.z = 0f;                                                                 // x, z값을 0으로 초기화
            transform.rotation = cameraRot;                                                                 // 플레이어의 회전값을 카메라 Pivot의 회전값으로 설정
            if (IsRun)                                                                                      // 달리기 상태이면
            {
                Quaternion charRot = Quaternion.LookRotation(moveVelocity);                                     // 이동 벡터를 바라보는 회전값
                charRot.x = cameraRot.z = 0f;                                                                   // x, z값을 0으로 초기화
                modelTr.rotation = Quaternion.Slerp(modelTr.rotation, charRot, Time.deltaTime * 10f);           // 모델의 회전값을 이동 벡터를 바라보는 회전값으로 설정
            }
            else                                                                                            // 걷기 상태이면
                modelTr.rotation = Quaternion.Slerp(modelTr.rotation, cameraRot, Time.deltaTime * 10f);         // 모델의 회전값을 카메라 Pivot의 회전값으로 설정
        }
    }
    private bool GroundCheck(out RaycastHit hit)                // 땅에 닿았는지 체크
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit, 0.25f);           // 플레이어의 아래쪽으로 레이캐스트를 쏴서 땅에 닿았는지 확인
    }

    private float nextTime = 0f;
    private void AttackTimeState()                              // 공격 시간 체크
    {
        nextTime += Time.deltaTime;
        if (nextTime >= 1.2f)
        {
            playerState = PlayerState.IDLE;
        }
    }
    private void PlayerAttack()                             // 공격
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
    private void PlayerShieldAttack()                       // 방패 공격
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



/*
    private void OnDrawGizmos()
    {
        if (!cameraTr || !cameraPivotTr)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawRay(cameraPivotTr.position, (cameraTr.position - cameraPivotTr.position).normalized * cameraDistance);   // 카메라 Pivot에서 카메라까지의 Ray
        Gizmos.color = Color.green;
        Gizmos.DrawLine(cameraTr.position, cameraPivotTr.position);                                                         // 카메라에서 카메라 Pivot까지의 벡터
    }
*/

}