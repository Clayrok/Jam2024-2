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

    private Vector3 m_BaseOffset = Vector3.zero;
    private Vector3 m_CurrentOffset = Vector3.zero;



    private void Start()
    {
        m_BaseOffset = transform.position - m_TargetToFollow.transform.position;
        m_CurrentOffset = m_BaseOffset;
    }

    private void LateUpdate()
    {
        transform.position = m_TargetToFollow.transform.position + m_CurrentOffset;
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        if (InputManager.ZoomIn)
        {
            m_CurrentOffset /= (1 + m_ZoomStep);
            if (m_CurrentOffset.magnitude < m_MinCameraDistance)
            {
                m_CurrentOffset = m_CurrentOffset.normalized * m_MinCameraDistance;
            }
        }
        
        if (InputManager.ZoomOut)
        {
            m_CurrentOffset = Vector3.ClampMagnitude(m_CurrentOffset * (1 + m_ZoomStep), m_MaxCameraDistance);
        }
    }
}