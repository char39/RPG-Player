using UnityEngine;

public partial class Ch29RPG : MonoBehaviour
{
    private bool GroundCheck(out RaycastHit hit) => Physics.Raycast(transform.position, Vector3.down, out hit, 0.25f);
    private void RunCheck()
    {
        if (!IsRun && RunButton)
            IsRun = true;
        else if (IsRun && Move_H == 0 && Move_V == 0)
            IsRun = false;
    }

    private void PlayerIdleMove()
    {
        RunCheck();
        if (chCtrl.isGrounded)
        {
            if (!IsGrounded)
                IsGrounded = true;
            CalculatorInputMove();
            if (GroundCheck(out RaycastHit groundHit))
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

    private void CalculatorInputMove()
    {
        moveVelocity = new Vector3(MoveRaw_H, 0f, MoveRaw_V).normalized * (IsRun ? runSpeed : walkSpeed);
        ani.SetFloat(hashSpeedX, Move_H);
        ani.SetFloat(hashSpeedY, Move_V);
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
}
