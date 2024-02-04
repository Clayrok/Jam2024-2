using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputManager
{
    public static bool Forward => Input.GetKey(KeyCode.UpArrow);

    public static bool Left => Input.GetKey(KeyCode.LeftArrow);

    public static bool Right => Input.GetKey(KeyCode.RightArrow);

    public static bool ZoomIn => Input.GetAxisRaw("Mouse ScrollWheel") > 0;

    public static bool ZoomOut => Input.GetAxisRaw("Mouse ScrollWheel") < 0;
}