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
        Vector3 offset = m_CurrentOffset;
        offset = Quaternion.FromToRotation(Vector3.forward, m_TargetToFollow.transform.forward) * offset;

        transform.position = m_TargetToFollow.transform.position + offset;
        transform.forward = m_TargetToFollow.transform.position - transform.position;
    }

    private void Update()
    {
        Zoom();
    }

    private void Zoom()
    {
        if (InputManager.ZoomIn)
        {
            m_CurrentOffset /= m_ZoomStep;
            Debug.Log(m_CurrentOffset.magnitude);
        }

        if (InputManager.ZoomOut)
        {
            m_CurrentOffset *= m_ZoomStep;
            Debug.Log(m_CurrentOffset.magnitude);
        }

        if (m_CurrentOffset.magnitude < m_MinCameraDistance)
        {
            m_CurrentOffset = m_CurrentOffset.normalized * m_MinCameraDistance;
        }

        if (m_CurrentOffset.magnitude > m_MaxCameraDistance)
        {
            m_CurrentOffset = m_CurrentOffset.normalized * m_MaxCameraDistance;
        }
    }
}