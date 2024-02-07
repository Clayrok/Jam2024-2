using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ship : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer m_SkyMeshRenderer = null;

    [SerializeField]
    private BatteryReceptor m_LeftThrusterBatteryReceptor = null;

    [SerializeField]
    private BatteryReceptor m_CentralThrusterBatteryReceptor = null;

    [SerializeField]
    private BatteryReceptor m_RightThrusterBatteryReceptor = null;

    [SerializeField]
    private BatteryReceptor m_OxygenRefillReceptor = null;

    [SerializeField]
    private float m_LinearThrustersStrength = 6f;

    [SerializeField]
    private float m_AgularThrustersStrength = 1f;

    [SerializeField]
    private float m_AgularThrustersLinearStrength = 3f;

    [SerializeField]
    private float m_LinearForceLimit = 9f;

    [SerializeField]
    private float m_VelocityDampening = 1f;

    [SerializeField]
    private float m_MaxVelocity = 10f;

    [SerializeField]
    private float m_OxygenDecreaseSpeed = 1f;

    [SerializeField]
    private float m_RefillOxygenSpeed = 5f;

    private Rigidbody m_Rigidbody = null;

    private bool m_IsCentralThrusterActivated = false;
    private bool m_IsLeftThrusterActivated = false;
    private bool m_IsRightThrusterActivated = false;

    private float m_OxygenRemaining = 100f;

    private Vector3? m_DestinationPosition = null;


    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.maxLinearVelocity = m_MaxVelocity;
    }

    private void Update()
    {
        UpdateThrusters();
        UpdateSky();
        ConsumeOxygen();
        RefillOxygen();
    }

    private void FixedUpdate()
    {
        UpdateLinearVelocity();
        UpdateAngularVelocity();

        DampDownVelocity();
    }

    private void UpdateThrusters()
    {
        m_IsLeftThrusterActivated = m_LeftThrusterBatteryReceptor.GetIsPowered();
        m_IsCentralThrusterActivated = m_CentralThrusterBatteryReceptor.GetIsPowered();
        m_IsRightThrusterActivated= m_RightThrusterBatteryReceptor.GetIsPowered();
    }

    private void UpdateLinearVelocity()
    {
        Vector3 linearVelocity = m_Rigidbody.velocity;
        linearVelocity += transform.forward * CalculateLinearMovementStrength() * Time.deltaTime;
        linearVelocity = Vector3.ClampMagnitude(linearVelocity, m_MaxVelocity);
        linearVelocity.y = 0;
        m_Rigidbody.velocity = linearVelocity;
    }

    private float CalculateLinearMovementStrength()
    {
        float strength = 0f;

        strength += m_IsCentralThrusterActivated ? m_LinearThrustersStrength : 0f;
        strength += m_IsLeftThrusterActivated && m_IsRightThrusterActivated ? m_LinearThrustersStrength * 2f : 0f;
        strength += m_IsLeftThrusterActivated || m_IsRightThrusterActivated ? m_AgularThrustersLinearStrength : 0f;

        strength = Mathf.Clamp(strength, 0f, m_LinearForceLimit);

        return strength;
    }

    private void UpdateAngularVelocity()
    {
        Vector3 angularVelocity = m_Rigidbody.angularVelocity;
        angularVelocity += m_IsLeftThrusterActivated && !m_IsRightThrusterActivated ? transform.up * m_AgularThrustersStrength * Time.deltaTime : Vector3.zero;
        angularVelocity += m_IsRightThrusterActivated && !m_IsLeftThrusterActivated ? -transform.up * m_AgularThrustersStrength * Time.deltaTime : Vector3.zero;
        angularVelocity = Vector3.ClampMagnitude(angularVelocity, m_MaxVelocity);
        angularVelocity.x = 0;
        angularVelocity.z = 0;
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

    private void ConsumeOxygen()
    {
        m_OxygenRemaining = Mathf.Clamp(m_OxygenRemaining - m_OxygenDecreaseSpeed * Time.deltaTime, 0f, 100f);
    }

    private void RefillOxygen()
    {
        if (m_OxygenRefillReceptor != null & m_OxygenRefillReceptor.GetIsPowered())
        {
            m_OxygenRemaining = Mathf.Clamp(m_OxygenRemaining += m_RefillOxygenSpeed * Time.deltaTime, 0f, 100f);
        }
    }

    public float GetRemainingOxygen()
    {
        return m_OxygenRemaining;
    }

    public void SetDestinationPosition(Vector3 _Position)
    {
        m_DestinationPosition = _Position;
    }

    public Vector3? GetDestinationPosition()
    {
        if (m_DestinationPosition == null)
        {
            return null;
        }

        return m_DestinationPosition.Value;
    }

    private void Start()
    {
        SetDestinationPosition(new Vector3(10, 0, 10));
    }
}