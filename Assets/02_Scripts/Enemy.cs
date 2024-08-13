using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Vars

    public enum EnemyState { MOVE, TRACE, ATTACK, DEAD }
    public EnemyState enemyState = EnemyState.MOVE;

    private Transform hitBox;
    private Animator ani;

    private float nextTime = 0;
    public bool isDead = false;

    public Transform Player;
    public Transform Points;
    public List<Transform> PointList;
    private int nextPoint = 0;
    private float speed = 2.5f;
    private float distance = 0;

    #endregion

    void Start()
    {
        StartVars();
    }

    void Update()
    {
        EnemyStateCheck();
        EnemyStates();
    }

    private void StartVars()
    {
        hitBox = transform.GetChild(2).transform;
        ani = GetComponent<Animator>();
        Player = GameObject.Find("Player").transform;
        if (Points != null)
        {
            foreach (Transform point in Points)
                PointList.Add(point);
        }
    }

    private void EnemyStateCheck()
    {
        if (enemyState == EnemyState.DEAD) return;
        distance = Vector3.Distance(transform.position, Player.transform.position);
        if (distance < 3.5f)
            enemyState = EnemyState.ATTACK;
        else if (distance < 10.0f)
            enemyState = EnemyState.TRACE;
        else
            enemyState = EnemyState.MOVE;
    }

    private void EnemyStates()
    {
        switch (enemyState)
        {
            case EnemyState.MOVE:
                Move();
                break;
            case EnemyState.TRACE:
                Trace();
                break;
            case EnemyState.ATTACK:
                Attack();
                break;
            case EnemyState.DEAD:
                Dead();
                break;
        }
    }

    private void Move()
    {
        if (isDead) return;
        ani.SetFloat("speedX", 0);
        ani.SetFloat("speedY", 0.5f);
        ani.SetBool("isRun", false);
        MoveApply();
    }
    private void Trace()
    {
        if (isDead) return;
        ani.SetFloat("speedX", 0);
        ani.SetFloat("speedY", 1);
        ani.SetBool("isRun", true);
        TraceApply();
    }
    private void Attack()
    {
        if (isDead) return;
        ani.SetFloat("speedX", 0);
        ani.SetFloat("speedY", 0);
        ani.SetBool("isRun", false);
        AttackApply();
    }
    private void Dead()
    {
        if (isDead) return;
        isDead = true;
        ani.SetFloat("speedX", 0);
        ani.SetFloat("speedY", 0);
        ani.SetBool("isDead", true);
        ani.SetTrigger("dieTrigger");
    }

    public void HitBoxOn()
    {
        hitBox.gameObject.SetActive(true);
    }
    public void HitBoxOff()
    {
        hitBox.gameObject.SetActive(false);
    }

    private void MoveApply()
    {
        if (Vector3.Distance(transform.position, PointList[nextPoint].position) < 3f)
        {
            nextPoint++;
            if (nextPoint >= PointList.Count)
                nextPoint = 0;
        }
        transform.position = Vector3.MoveTowards(transform.position, PointList[nextPoint].position, speed * Time.deltaTime);
        Vector3 direction = PointList[nextPoint].position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    private void TraceApply()
    {
        transform.position = Vector3.MoveTowards(transform.position, Player.position, speed  * 2f * Time.deltaTime);
        Vector3 direction = Player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
    private void AttackApply()
    {
        if (Time.time - 2.0f > nextTime)
        {
            nextTime = Time.time;
            ani.SetTrigger("swordAttackTrigger");
        }
        Vector3 direction = Player.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction);
    }
}