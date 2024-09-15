using UnityEngine;

public partial class Ch29RPG : MonoBehaviour
{
    private float nextTime = 0f;
    
    private void AttackTimeState()
    {
        nextTime += Time.deltaTime;
        if (nextTime >= 1.2f)
        {
            playerState = PlayerState.IDLE;
            Fire1Trigger = false;
            Fire2Trigger = false;
        }
    }

    private void PlayerAttack()
    {
        if (Fire1 && !Fire1Trigger)
        {
            Fire1Trigger = true;
            playerState = PlayerState.ATTACK;
            ani.SetTrigger(hashAttack);
            ani.SetFloat(hashSpeedX, 0f);
            ani.SetFloat(hashSpeedY, 0f);
            nextTime = 0f;
        }
    }

    private void PlayerShieldAttack()
    {
        if (Fire2 && !Fire2Trigger)
        {
            Fire2Trigger = true;
            playerState = PlayerState.ATTACK;
            ani.SetTrigger(hashShieldAttack);
            ani.SetFloat(hashSpeedX, 0f);
            ani.SetFloat(hashSpeedY, 0f);
            nextTime = 0f;
        }
    }
}