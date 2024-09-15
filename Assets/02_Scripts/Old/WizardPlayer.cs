using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardPlayer : MonoBehaviour
{
    #region ///////////////////////////  Components  ///////////////////////////

    private NavMeshAgent navAgent;
    private Animator ani;
    private Transform hitBox;

    #endregion /////////////////////////////////////////////////////////////////
    #region ///////////////////////////     Click    ///////////////////////////

    public float m_DoubleClickSecond = 0.25f;   // 더블클릭까지의 유효시간
    public bool m_IsOneClick = false;           // 클릭이 되었는가
    public double m_Timer = 0.0d;               // 정확한 시간 계산을 위하여 double을 사용
    private Ray ray;
    private RaycastHit hit;
    private Vector3 targetPos = Vector3.zero;
    private LayerMask groundLayer;
    private readonly int hashMoveSpeed = Animator.StringToHash("moveSpeed");
    private readonly int hashAttack = Animator.StringToHash("AttackTrigger");       // 공격은 Z, 움직일 때 못함
    private readonly int hashSkill = Animator.StringToHash("SkillTrigger");         // 스킬은 X, 움직일 때 못함
    public int moveState = 0;      // 0:정지, 1:걷기, 2:달리기
    public float walkSpeed = 4.0f;
    public float runSpeed = 8.0f;
    public bool isAttack = false;
    public bool isSkill = false;

    public bool isDie = false;

    #endregion /////////////////////////////////////////////////////////////////

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animator>();
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        hitBox = transform.GetChild(3).transform;
    }

    void Update()
    {
        if (isDie)
            return;
        ClickCheck();
        MoveMethod();
        AttackSkill();
    }

    private void ClickCheck()               // 클릭 관련 메서드
    {
        if (m_IsOneClick && Time.time - m_Timer > m_DoubleClickSecond)          // 클릭을 했고, 더블클릭 유효시간이 다 되었을 경우
            m_IsOneClick = false;                                                   // 클릭 입력을 없앰
        if (Input.GetMouseButtonDown(0))                                        // 클릭을 했을 때
        {
            if (!m_IsOneClick)                                                      // 클릭 입력이 없었을 경우
            {
                m_Timer = Time.time;                                                    // 더블클릭까지의 시간 계산을 위하여 지금 시간을 할당
                m_IsOneClick = true;                                                    // 클릭 입력
            }
            else if (m_IsOneClick && Time.time - m_Timer < m_DoubleClickSecond)     // 더블클릭 유효시간 전에 클릭을 했을 경우
                m_IsOneClick = false;                                                   // 클릭 입력을 없앰
        }
    }
    private void MoveMethod()               // 캐릭터 움직임 관련 메서드
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);            // 마우스의 화면 좌표에서 카메라를 통하여 광선을 발사
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.red);       // 광선을 시각적으로 확인하기 위하여 디버그 레이를 그림
        if (Input.GetMouseButtonDown(0) && !(isAttack || isSkill))          // 마우스 좌클릭을 했을 때
        {
            if (Physics.Raycast(ray, out hit, 100.0f, groundLayer))             // 광선이 지면 레이어에 닿았을 때
            {
                if (m_IsOneClick)                                                   // 클릭을 했을 때
                    moveState = 1;
                else                                                                // 더블클릭을 했을 때
                    moveState = 2;
                targetPos = hit.point;                                              // 목표 지점을 클릭한 지점으로 변경
                navAgent.SetDestination(targetPos);                                 // 목표 지점으로 이동
                navAgent.isStopped = false;                                         // 네비게이션을 멈추지 않음
            }
        }
        else
        {
            /*  도착시 Idle로 전환하는 1번째 방법
            if (navAgent.remainingDistance < 0.25f)
            {
                navAgent.speed = 0.0f;
                ani.SetFloat(hashMoveSpeed, navAgent.speed);
            }*/
            /*  도착시 Idle로 전환하는 2번째 방법
            if (Vector3.Distance(transform.position, targetPos) < 0.25f)
            {
                navAgent.speed = 0.0f;
                ani.SetFloat(hashMoveSpeed, navAgent.speed);
            }*/
            if (navAgent.remainingDistance < 0.25f && !navAgent.isStopped)
                moveState = 0;
        }
        MoveStateUpdate();
    }
    private void MoveStateUpdate()          // moveState 부드럽게 이동하는 메서드
    {
        if (moveState == 0)
        {
            if (navAgent.speed > walkSpeed / runSpeed)
                navAgent.speed = Mathf.Lerp(navAgent.speed, 0.0f, Time.deltaTime * 1f);
            else if (navAgent.speed > 0.0f)
                navAgent.speed = Mathf.Lerp(navAgent.speed, -1.0f, Time.deltaTime * 1.5f);
            else if (navAgent.speed <= 0.0f)
            {
                navAgent.speed = 0.0f;
                navAgent.isStopped = true;
            }
        }
        else if (moveState == 1)
            navAgent.speed = Mathf.Lerp(navAgent.speed, walkSpeed, Time.deltaTime * 3.0f);
        else if (moveState == 2)
            navAgent.speed = Mathf.Lerp(navAgent.speed, runSpeed, Time.deltaTime * 3.0f);
        ani.SetFloat(hashMoveSpeed, navAgent.speed / runSpeed);
    }

    private void AttackSkill()              // 공격, 스킬 결정 메서드
    {
        if (moveState == 0)
        {
            if (Input.GetKeyDown(KeyCode.Z) && !isAttack)
            {
                isAttack = true;
                ani.SetTrigger(hashAttack);
            }
            if (Input.GetKeyDown(KeyCode.X) && !isSkill)
            {
                isSkill = true;
                ani.SetTrigger(hashSkill);
            }
        }
    }
    public void SetAttackSkillEnd()         // 공격, 스킬 상태 해제 (호출용)
    {
        isAttack = false;
        isSkill = false;
    }

    public void PlayerDie()                // 플레이어 사망 메서드
    {
        isDie = true;
        moveState = 0;
        navAgent.isStopped = true;
        ani.SetFloat(hashMoveSpeed, 0.0f);
        ani.SetTrigger("DieTrigger");
    }

    public void HitBoxOn()
    {
        hitBox.gameObject.SetActive(true);
    }
    public void HitBoxOff()
    {
        hitBox.gameObject.SetActive(false);
    }
}