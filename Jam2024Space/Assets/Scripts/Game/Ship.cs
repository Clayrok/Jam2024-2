using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_SkyMeshRenderer = null;

    [SerializeField]
    private float m_LinearThrustersStrength = 6f;

    [SerializeField]
    private float m_AgularThrustersStrength = 1f;

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
        UpdateSky();
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
        linearVelocity += m_IsCentralThrusterActivated || (m_IsLeftThrusterActivated && m_IsRightThrusterActivated) ? transform.forward * m_LinearThrustersStrength * Time.deltaTime : Vector3.zero;
        linearVelocity = Vector3.ClampMagnitude(linearVelocity, m_MaxVelocity);
        m_Rigidbody.velocity = linearVelocity;
    }

    private void UpdateAngularVelocity()
    {
        Vector3 angularVelocity = m_Rigidbody.angularVelocity;
        angularVelocity += m_IsLeftThrusterActivated && !m_IsRightThrusterActivated ? transform.up * m_AgularThrustersStrength * Time.deltaTime : Vector3.zero;
        angularVelocity += m_IsRightThrusterActivated && !m_IsLeftThrusterActivated ? -transform.up * m_AgularThrustersStrength * Time.deltaTime : Vector3.zero;
        angularVelocity = Vector3.ClampMagnitude(angularVelocity, m_MaxVelocity);
        m_Rigidbody.angularVelocity = angularVelocity;
    }

    private void DampDownVelocity()
    {
        Vector3 linearVelocity = m_Rigidbody.velocity;
        linearVelocity *= m_VelocityDampening;
        m_Rigidbody.velocity = linearVelocity;

        Vector3 angularVelocity = m_Rigidbody.angularVelocity;
        angularVelocity.y *= m_VelocityDampening;
        m_Rigidbody.angularVelocity = angularVelocity;
    }

    private void UpdateSky()
    {
        Vector3 position = transform.position;
        position.y = m_SkyMeshRenderer.transform.position.y;
        m_SkyMeshRenderer.transform.position = position;

        Vector3 linearVelocity = -m_Rigidbody.velocity * Time.deltaTime;
        Vector2 offset2D = new Vector2(linearVelocity.x, linearVelocity.z);
        offset2D = m_SkyMeshRenderer.material.GetTextureOffset("_MainTex") + offset2D;

        m_SkyMeshRenderer.material.SetTextureOffset("_MainTex", offset2D);
    }
}