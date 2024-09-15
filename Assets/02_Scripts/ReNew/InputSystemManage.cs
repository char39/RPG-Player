using UnityEngine;
using UnityEngine.InputSystem;

public class InputSystemManage : MonoBehaviour
{
    public float Horizontal = 0;
    public float Vertical = 0;
    public Vector3 MoveVector = Vector3.zero;

    public float MouseHorizontal = 0;
    public float MouseVertical = 0;
    public Vector2 MouseMoveVector = Vector2.zero;

    public float MouseScroll = 0;

    public bool Fire1 = false;
    public bool Fire2 = false;

    public bool RunButton = false;

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        Horizontal = input.x;
        Vertical = input.y;
        MoveVector = new Vector3(input.x, 0, input.y);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        MouseHorizontal = input.x;
        MouseVertical = input.y;
        MouseMoveVector = input;
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        float input = context.ReadValue<Vector2>().y;
        MouseScroll = input;
    }

    public void OnFire1(InputAction.CallbackContext context)
    {
        bool input = context.ReadValueAsButton();
        Fire1 = input;
    }

    public void OnFire2(InputAction.CallbackContext context)
    {
        bool input = context.ReadValueAsButton();
        Fire2 = input;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        bool input = context.ReadValueAsButton();
        RunButton = input;
    }
}
