using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : Pickable
{
    private float m_BatteryLevel = 100f;


    public void Drain(float _Consumption)
    {
        m_BatteryLevel = Mathf.Clamp(0f, 100f, m_BatteryLevel - _Consumption);
    }

    public bool GetHasPower()
    {
        return m_BatteryLevel > 0f;
    }
}