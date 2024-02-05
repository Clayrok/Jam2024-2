using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryReceptor : Interactable
{
    [SerializeField]
    private GameObject m_BatteryPoint  = null;

    private Battery m_Battery = null;

    private float m_PowerConsumption = 0f;


    private void Update()
    {
        if (m_Battery != null)
        {
            m_Battery.Drain(m_PowerConsumption * Time.deltaTime);
        }
    }

    public override void Interact()
    {
        
    }

    public void PlaceBattery(Battery _Battery)
    {
        m_Battery = _Battery;

        if (m_Battery.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
        }

        m_Battery.transform.position = m_BatteryPoint.transform.position;
        m_Battery.transform.rotation = m_BatteryPoint.transform.rotation;
    }

    public Battery TakeBattery()
    {
        if (!m_Battery)
        {
            return null;
        }

        if (m_Battery.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;
        }

        Battery battery = m_Battery;
        m_Battery = null;

        return battery;
    }

    public bool GetIsPowered()
    {
        return m_Battery != null && m_Battery.GetHasPower();
    }
}