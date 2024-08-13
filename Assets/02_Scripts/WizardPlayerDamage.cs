using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardPlayerDamage : MonoBehaviour
{
    private float maxHp = 100.0f;
    public float hp;
    private WizardPlayer wizardPlayer;

    void Start()
    {
        hp = maxHp;
        wizardPlayer = GetComponent<WizardPlayer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "HitBox_E")
        {
            hp -= 10.0f;
            if (hp <= 0)
            {
                hp = 0;
                if (!wizardPlayer.isDie)
                    wizardPlayer.PlayerDie();
            }
        }
    }
}
