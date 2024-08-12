using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCtrlRpg : MonoBehaviour
{
    public enum PlayerState { IDLE,ATTACK,UNDER_ATTACK,DEAD}
    public PlayerState state = PlayerState.IDLE;
    [Tooltip("걷기 속도")] public float walkSpeed = 5f;
    [Tooltip("달리기 속도")] public float runSpeed = 10f;
    [Header("Camera 관련 변수")]
    [SerializeField] private Transform cameraTr;
    [SerializeField] private Transform cameraPivotTr;
    [SerializeField] private float cameraDistance = 0f;
    [SerializeField] private Vector3 mouseMove = Vector3.zero;
    [SerializeField] private int playerLayer;
    [Header("Player 움직임 관련변수")]
    [SerializeField] private Transform modelTr;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Vector3 moveVelocity= Vector3.zero;
     private bool IsGrounded = false;
    private bool isRun;
    public bool IsRun
    {
        get { return isRun; }
        set 
        { 
            isRun = value;
            animator.SetBool("isRun", value);
        }
    }
    private float nextTime = 0f;
    void Start()
    {
        cameraTr = Camera.main.transform;
        cameraPivotTr = Camera.main.transform.parent;
        playerLayer = LayerMask.NameToLayer("PLAYER");
        modelTr = GetComponentsInChildren<Transform>()[1];
        animator = GetComponentInChildren<Animator>();
        characterController = GetComponentInChildren<CharacterController>();
        cameraDistance = 5f;
    }

    void Update()
    {
        FreezeXZ();

        switch (state)
        {
            case PlayerState.IDLE:
                PlayerIdleAndMove();
                break;
            case PlayerState.ATTACK:
                AttackTimeState();
                break;
            case PlayerState.UNDER_ATTACK:

                break;
            case PlayerState.DEAD:

                break;

        }
        CameraDistanceCtrl();
    }
    private void LateUpdate()
    {
        float cameraHeight = 1.3f;
        cameraPivotTr.localPosition = transform.position +(Vector3.up * cameraHeight);
        mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 100 * 0.1f, Input.GetAxisRaw("Mouse X") * 100f * 0.1f, 0f);
        if (mouseMove.x < -40f)
            mouseMove.x = -40f;
        else if (mouseMove.x > 40f)
            mouseMove.x = 40f;

        cameraPivotTr.localEulerAngles = mouseMove;

        RaycastHit hit;
        Vector3 dir = (cameraTr.position - cameraPivotTr.position).normalized;
        if (Physics.Raycast(cameraPivotTr.position, dir, out hit,cameraDistance ,~(1 << playerLayer)))
            cameraTr.localPosition = Vector3.back * hit.distance;
        else
            cameraTr.localPosition = Vector3.back * cameraDistance;
    }
    void PlayerIdleAndMove()
    {
        RunCheck();
        if(characterController.isGrounded)
        {
            if (IsGrounded == false) IsGrounded = true;
            animator.SetBool("isGrounded", true);
            CalcInputMove();
            RaycastHit groundHit;
            if (GroundCheck(out groundHit))
                moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;
            else
                moveVelocity.y = -1f;
            PlayerAttack();

        }
        else
        {
            if (IsGrounded == false) IsGrounded = true;
            else
               animator.SetBool("isGrounded", false);

            moveVelocity += Physics.gravity * Time.deltaTime;
        }
        characterController.Move(moveVelocity * Time.deltaTime);
    }

    private void PlayerAttack()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            state = PlayerState.ATTACK;
            animator.SetTrigger("swordAttackTrigger");
            animator.SetFloat("speedX", 0f);
            animator.SetFloat("speedY", 0f);
            nextTime = 0f;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            state = PlayerState.ATTACK;
            animator.SetTrigger("shieldAttackTrigger");
            animator.SetFloat("speedX", 0f);
            animator.SetFloat("speedY", 0f);
            nextTime = 0f;
        }
    }
    void AttackTimeState()
    {
        nextTime += Time.deltaTime;
        if(1f <= nextTime)
        {
            state = PlayerState.IDLE;
        }
    }

    void RunCheck()
    {
        if(IsRun==false&& Input.GetKey(KeyCode.LeftShift))
            IsRun = true;
        else if(IsRun ==true&& Input.GetAxis("Horizontal")==0 && Input.GetAxis("Vertical")==0)
            IsRun = false;

    }
    void CalcInputMove()
    {
        moveVelocity = new Vector3(Input.GetAxisRaw("Horizontal"),0f,Input.GetAxisRaw("Vertical")).normalized * (IsRun ? runSpeed : walkSpeed);
        animator.SetFloat("speedX", Input.GetAxis("Horizontal"));
        animator.SetFloat("speedY", Input.GetAxis("Vertical"));
        moveVelocity = transform.TransformDirection(moveVelocity);
        if(0.01f < moveVelocity.sqrMagnitude)
        {
            Quaternion cameraRot = cameraPivotTr.rotation;
            cameraRot.x = cameraRot.z = 0f;
            transform.rotation = cameraRot;
            if(IsRun)
            {
                Quaternion characterRot = Quaternion.LookRotation(moveVelocity);
                characterRot.x = characterRot.z = 0f;
                modelTr.rotation = Quaternion.Slerp(modelTr.rotation, characterRot, Time.deltaTime * 10f);
            }
            else
            {
                modelTr.rotation = Quaternion.Slerp(modelTr.rotation, cameraRot, Time.deltaTime * 10f);
            }
        }
        
    }
    bool GroundCheck(out RaycastHit  hit)
    {

        return Physics.Raycast(transform.position,Vector3.down,out hit,0.2f);
    }
    void PlayerSwordAttack()
    {


    }
    void PlayerShieldAttack()
    {


    }
    void CameraDistanceCtrl()
    {
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel");
    }
    void FreezeXZ()
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
}
