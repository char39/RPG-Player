using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRPG : MonoBehaviour
{
    public enum PlayerState { IDLE, ATTACK, UNDER_ATTACK, DEAD }
    public PlayerState playerState = PlayerState.IDLE;

    [Tooltip("Walk Speed")] public float walkSpeed = 5f;
    [Tooltip("Run Speed")] public float runSpeed = 10f;

    [Header("Camera Vars")]
    [SerializeField] private Transform cameraTr;                // 카메라 위치
    [SerializeField] private Transform cameraPivotTr;           // 카메라 Pivot 위치
    [SerializeField] private float cameraDistance = 0f;         // 카메라와의 거리
    [SerializeField] private Vector3 mouseMove = Vector3.zero;  // 마우스 이동 벡터
    [SerializeField] private LayerMask playerLayer;             // 플레이어 레이어
    //               private int playerLayer;                   // layer index = 10

    void Start()
    {
        cameraTr = Camera.main.transform;
        cameraPivotTr = cameraTr.parent;
    }

    void Update()
    {
        
    }
}
