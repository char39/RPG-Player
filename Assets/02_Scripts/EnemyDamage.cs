using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private float maxHp = 100.0f;
    public float hp;
    private Enemy enemy;

    void Start()
    {
        hp = maxHp;
        enemy = GetComponent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HitBox")
        {
            hp -= 10.0f;
            if (hp <= 0)
            {
                hp = 0;
                enemy.enemyState = Enemy.EnemyState.DEAD;
            }
        }
    }    
}
