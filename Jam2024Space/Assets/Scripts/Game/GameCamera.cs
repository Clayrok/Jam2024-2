using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject m_TargetToFollow = null;

    [SerializeField]
    private float m_ZoomStep = 0.1f;

    [SerializeField]
    private float m_CameraAngleDegrees = 45f;

    [SerializeField]
    private float m_MinCameraDistance = 1f;

    [SerializeField]
    private float m_MaxCameraDistance = 5f;

    [SerializeField]
    private float m_CameraDistance = 10f;


    private void LateUpdate()
    {
        Quaternion cameraRotation = Quaternion.AngleAxis(m_CameraAngleDegrees, m_TargetToFollow.transform.right);
        Vector3 offset = cameraRotation * (-m_TargetToFollow.transform.forward);
        offset = offset.normalized * m_CameraDistance;
        transform.position = m_TargetToFollow.transform.position + offset;
        transform.rotation = cameraRotation * Quaternion.FromToRotation(Vector3.forward, m_TargetToFollow.transform.forward);
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        if (InputManager.ZoomIn)
        {
            m_CameraDistance /= m_ZoomStep;
        }

        if (InputManager.ZoomOut)
        {
            m_CameraDistance *= m_ZoomStep;
        }

        if (m_CameraDistance < m_MinCameraDistance)
        {
            m_CameraDistance = m_MinCameraDistance;
        }

        if (m_CameraDistance > m_MaxCameraDistance)
        {
            m_CameraDistance = m_MaxCameraDistance;
        }
    }
}