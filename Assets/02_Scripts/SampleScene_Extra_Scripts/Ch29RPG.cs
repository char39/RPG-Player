using UnityEngine;

public partial class Ch29RPG : MonoBehaviour
{
    void Start()
    {
        SetVars();
    }

    void Update()
    {
        InputGetAxis();
        FreezeXZ();
        PlayerStates();
        // CameraDistanceCtrl(); // FreeLook Camera의 Orbit 변수가 접근권한이 없어서 못함.
    }

    private void SetVars()
    {
        cameraTr = Camera.main.transform;
        cameraPivotTr = cameraTr.parent;
        cameraDistance = 5f;    // 못씀
        playerLayer = 1 << LayerMask.NameToLayer("Player");     // 못씀
        charactorTr = GetComponentsInChildren<Transform>()[1];
        ani = transform.GetChild(0).GetComponent<Animator>();
        chCtrl = GetComponent<CharacterController>();
        inputs = GetComponent<InputSystemManage>();
    }

    private void InputGetAxis()
    {
        Move_H = inputs.Horizontal;     // 사실 이 변수들은 할당할 필요 없으나 그냥 인스펙터에서 보기 위한 용도로 할당.
        Move_V = inputs.Vertical;
        MoveRaw_H = inputs.Horizontal;
        MoveRaw_V = inputs.Vertical;
        MouseMove_H = inputs.MouseHorizontal;
        MouseMove_V = inputs.MouseVertical;
        MouseScroll = inputs.MouseScroll;
        Fire1 = inputs.Fire1;
        Fire2 = inputs.Fire2;
        RunButton = inputs.RunButton;
    }

    private void FreezeXZ() => transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);

    private void PlayerStates()
    {
        switch (playerState)
        {
            case PlayerState.IDLE:
                PlayerIdleMove();
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

    // private void CameraCtrl()
    // {
    //     float cameraHeight = 1.3f;
    //     cameraPivotTr.position = transform.position + (Vector3.up * cameraHeight);
    //     mouseMove += new Vector3(-MouseMove_V * 3f, MouseMove_H * 3f);
    //     if (mouseMove.x < -40.0f)
    //         mouseMove.x = -40.0f;
    //     else if (mouseMove.x > 40.0f)
    //         mouseMove.x = 40.0f;
    //     cameraPivotTr.eulerAngles = mouseMove;

    //     RaycastHit hit;
    //     Vector3 dir = (cameraTr.position - cameraPivotTr.position).normalized;
    //     if (Physics.Raycast(cameraPivotTr.position, dir, out hit, cameraDistance, ~playerLayer))
    //     {
    //         Vector3 changeLocalPos = Vector3.back * hit.distance;
    //         cameraTr.localPosition = Vector3.Slerp(cameraTr.localPosition, changeLocalPos, Time.deltaTime * 15f);
    //     }
    //     else
    //     {
    //         Vector3 originLocalPos = Vector3.back * cameraDistance;
    //         cameraTr.localPosition = Vector3.Slerp(cameraTr.localPosition, originLocalPos, Time.deltaTime * 15f);
    //     }
    // }

    // private void CameraDistanceCtrl()
    // {
    //     cameraDistance -= MouseScroll * 0.01f;
    //     cameraDistance = Mathf.Clamp(cameraDistance, 2.2f, 10f);
    // }
}