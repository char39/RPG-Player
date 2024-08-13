using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    //private Transform camTr;
    private Transform camPivotTr;
    [Range(0, 20)] public float height = 3.0f;
    [Range(0, 20)] public float distance = 3.0f;
    private float targetOffSet = 1.0f;              // 카메라가 목표로하는 지점에서 약간 높은 위치에서 바라보기 위함.
    private float moveDamping = 5.0f;               // 부드러운 움직임을 위한 값
    private float rotDamping = 10.0f;               // 부드러운 회전을 위한 값

    void Start()
    {
        camPivotTr = GetComponent<Transform>();
        //camTr = transform.GetChild(0).GetComponent<Transform>();
    }

    void LateUpdate()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        var camPos = target.position - (Vector3.forward * distance) + (Vector3.up * height);
        camPivotTr.position = Vector3.Slerp(camPivotTr.position, camPos, moveDamping * Time.deltaTime);
        camPivotTr.rotation = Quaternion.Slerp(camPivotTr.rotation, target.rotation, rotDamping * Time.deltaTime);
        camPivotTr.LookAt(target.position + (Vector3.up * targetOffSet));
    }
}
