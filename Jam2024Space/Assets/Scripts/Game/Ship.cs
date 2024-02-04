using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [SerializeField]
    private float m_ThrustersStrength = 1f;

    [SerializeField]
    private float m_VelocityDampening = 1f;

    [SerializeField]
    private float m_MaxVelocity = 10f;

    private Rigidbody m_Rigidbody = null;

    private Vector3 m_CurrentVelocity = Vector3.zero;

    private bool m_IsCentralThrusterActivated = false;
    private bool m_IsLeftThrusterActivated = false;
    private bool m_IsRightThrusterActivated = false;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.maxLinearVelocity = m_MaxVelocity;
    }

    private void Update()
    {
        UpdateInputs();
    }

    private void FixedUpdate()
    {
        UpdateLinearVelocity();
        UpdateAngularVelocity();

        DampDownVelocity();
    }

    private void UpdateInputs()
    {
        m_IsCentralThrusterActivated = InputManager.Forward;
        m_IsLeftThrusterActivated = InputManager.Left;
        m_IsRightThrusterActivated= InputManager.Right;
    }

    private void UpdateLinearVelocity()
    {
        Vector3 linearVelocity = m_Rigidbody.velocity;
        linearVelocity += m_IsCentralThrusterActivated ? transform.forward * m_ThrustersStrength * Time.deltaTime : Vector3.zero;
        linearVelocity = Vector3.ClampMagnitude(linearVelocity, m_MaxVelocity);
        m_Rigidbody.velocity = linearVelocity;
    }

    private void UpdateAngularVelocity()
    {
        Vector3 angularVelocity = m_Rigidbody.angularVelocity;
        angularVelocity += m_IsLeftThrusterActivated ? transform.up * m_ThrustersStrength * Time.deltaTime : Vector3.zero;
        angularVelocity += m_IsRightThrusterActivated ? -transform.up * m_ThrustersStrength * Time.deltaTime : Vector3.zero;
        angularVelocity = Vector3.ClampMagnitude(angularVelocity, m_MaxVelocity);
        m_Rigidbody.angularVelocity = angularVelocity;
    }

    private void DampDownVelocity()
    {
        if (!m_IsCentralThrusterActivated)
        {
            Vector3 linearVelocity = m_Rigidbody.velocity;
            linearVelocity *= m_VelocityDampening;
            m_Rigidbody.velocity = linearVelocity;
        }

        if (!m_IsLeftThrusterActivated && !m_IsRightThrusterActivated)
        {
            Vector3 angularVelocity = m_Rigidbody.angularVelocity;
            angularVelocity.y *= m_VelocityDampening;
            m_Rigidbody.angularVelocity = angularVelocity;
        }
    }
}