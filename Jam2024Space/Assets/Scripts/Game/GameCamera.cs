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
    private float m_MinCameraDistance = 1f;

    [SerializeField]
    private float m_MaxCameraDistance = 5f;

    [SerializeField]
    private float m_DistanceY = 5f;

    [SerializeField]
    private float m_DistanceZ = 5f;

    private Vector3 m_CurrentOffset = Vector3.zero;


    private void Start()
    {
        m_CurrentOffset = new Vector3(0, m_DistanceY, -m_DistanceZ);
    }

    private void LateUpdate()
    {
        transform.position = m_TargetToFollow.transform.position + m_TargetToFollow.transform.forward * m_CurrentOffset.z + m_TargetToFollow.transform.up * m_CurrentOffset.y;
        transform.LookAt(m_TargetToFollow.transform);
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        if (InputManager.ZoomIn)
        {
            m_CurrentOffset += (m_TargetToFollow.transform.position - transform.position).normalized * m_ZoomStep;
        }

        if (InputManager.ZoomOut)
        {
            m_CurrentOffset -= (m_TargetToFollow.transform.position - transform.position).normalized * m_ZoomStep;
        }
    }
}