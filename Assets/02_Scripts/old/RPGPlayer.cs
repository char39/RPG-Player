using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGPlayer : MonoBehaviour
{
    public enum PlayerState { IDLE,ATTACK,UNDERATTACK,DEAD}
    public PlayerState playerState = PlayerState.IDLE;
    [SerializeField]
    [Tooltip("걷기 속도")] private float walkSpeed =5f;
    [SerializeField]
    [Tooltip("달리기 속도")] private float runSpeed =10.0f;
    [SerializeField] private Transform cameraPivotTr;
    [SerializeField] private Transform cameraTr;
    [SerializeField] private float cameraDistance = 0f;
    [SerializeField] private Vector3 mouseMove;

    [SerializeField] private Vector3 moveVelocity;
    [SerializeField] private Transform mariaModelTr;
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private bool IsGrounded = false;
    private float nextTime = 0;
    private bool isRun;
    public bool IsRun
    {
        get { return isRun; }
        set
        {
            isRun = value;
            animator.SetBool("IsRun", value);
        }
    }
    
    void Start()
    {
        cameraTr = Camera.main.transform;
        cameraPivotTr = Camera.main.transform.parent;
        cameraDistance = 5f;
        mariaModelTr = transform.GetChild(0).transform;
        animator = mariaModelTr.GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
        CameraDistnaceCtrl();
        FreezeXY();
        switch (playerState)
        {
            case PlayerState.IDLE:
                PlayerIdleAndMove();
                break;
            case PlayerState.ATTACK:
                PlayerAttack();
                break;
            case PlayerState.UNDERATTACK:

                break;
            case PlayerState.DEAD:

                break;
        }

    }
    private void LateUpdate()
    {
        if (Time.timeScale < 0.001f) return;
        float cameraHeight = 1.3f;
        cameraPivotTr.position = transform.position + (Vector3.up * cameraHeight);
         mouseMove += new Vector3(-Input.GetAxisRaw("Mouse Y") * 100f * 0.1f, Input.GetAxisRaw("Mouse X") * 100f * 0.1f, 0f);
        if (mouseMove.x < -60f)
            mouseMove.x = -60f;
        else if (60f < mouseMove.x)
            mouseMove.x = 60f;
        cameraPivotTr.localEulerAngles = mouseMove;
        RaycastHit hit;
        Vector3 Dir = (cameraTr.position- cameraPivotTr.position).normalized;
        if (Physics.Raycast(cameraPivotTr.position, Dir, out hit, cameraDistance, ~(1 << LayerMask.NameToLayer("PLAYER"))))
            cameraTr.localPosition = Vector3.back * hit.distance;
        else
            cameraTr.localPosition = Vector3.back * cameraDistance;

    }
    void PlayerIdleAndMove()
    {
        RunCheck();

        if(characterController.isGrounded)
        {
            if(IsGrounded==false) IsGrounded = true;
            animator.SetBool("IsGrounded", true);
            CalculatorInputMove();
            RaycastHit groundHit;
            if (GroundCheck(out groundHit))
                moveVelocity.y = IsRun ? -runSpeed : -walkSpeed;
            else
                moveVelocity.y = -1f;
            if(Input.GetButtonDown("Fire1"))
            {
                playerState = PlayerState.ATTACK;
                animator.SetTrigger("swordAttackTrigger");
                animator.SetFloat("SpeedX", 0f);
                animator.SetFloat("SpeedY", 0f);
                nextTime = 0;
            }
            
        }
        else
        {
            if (IsGrounded == false) IsGrounded = true;
            else 
                animator.SetBool("IsGrounded", false);
            moveVelocity += Physics.gravity * Time.deltaTime;
        }
        characterController.Move(moveVelocity * Time.deltaTime);

    }
    void RunCheck()
    {
        if(IsRun==false&& Input.GetKey(KeyCode.LeftShift))
            IsRun = true;
        else if(IsRun==true &&Input.GetAxis("Horizontal")==0&&Input.GetAxis("Vertical")==0)
            IsRun = false;
    }
    private void CalculatorInputMove()
    {
        moveVelocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized * (IsRun? runSpeed: walkSpeed);
        animator.SetFloat("SpeedX", Input.GetAxis("Horizontal"));
        animator.SetFloat("SpeedY", Input.GetAxis("Vertical"));
        moveVelocity = transform.TransformDirection(moveVelocity);
        if (IsRun)
        {
            Quaternion cameraRotation = cameraPivotTr.rotation;
            cameraRotation.x = cameraRotation.z = 0f;
            transform.rotation = cameraRotation;
            
            if (0.01f < moveVelocity.sqrMagnitude)
            {
                Quaternion characterRotation = Quaternion.LookRotation(moveVelocity);
                characterRotation.x = characterRotation.z = 0f;
                mariaModelTr.rotation = Quaternion.Slerp(mariaModelTr.rotation, characterRotation, Time.deltaTime * 10f);
            }
            else
            {
                mariaModelTr.rotation = Quaternion.Slerp(mariaModelTr.rotation, cameraRotation, Time.deltaTime * 10f);

            }
        }
        
    }
    void PlayerAttack()
    {
        nextTime += Time.deltaTime;
        if(2f<=nextTime)
        {
            playerState = PlayerState.IDLE;
        }


    }
    bool GroundCheck(out RaycastHit hit)
    {
        return Physics.Raycast(transform.position, Vector3.down, out hit ,0.2f);
    }
    void CameraDistnaceCtrl()
    {
        cameraDistance -= Input.GetAxis("Mouse ScrollWheel");
    }
    void FreezeXY()
    {
        transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);
    }
}
