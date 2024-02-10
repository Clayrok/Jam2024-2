using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static bool Forward => Input.GetKey(KeyCode.W);

    public static bool Backward => Input.GetKey(KeyCode.S);

    public static bool Left => Input.GetKey(KeyCode.A);

    public static bool Right => Input.GetKey(KeyCode.D);

    public static bool Interact => Input.GetKeyDown(KeyCode.E);

    public static bool ZoomIn => Input.GetAxisRaw("Mouse ScrollWheel") > 0;

    public static bool ZoomOut => Input.GetAxisRaw("Mouse ScrollWheel") < 0;
}