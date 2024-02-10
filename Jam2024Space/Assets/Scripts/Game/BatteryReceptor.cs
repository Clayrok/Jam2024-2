using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryReceptor : Receptor
{
    private float m_PowerConsumption = 0f;


    private void Update()
    {
        Battery battery = GetPlacedPickable() as Battery;
        if (battery)
        {
            battery.Drain(m_PowerConsumption * Time.deltaTime);
        }
    }

    public bool GetIsPowered()
    {
        Battery battery = GetPlacedPickable() as Battery;
        return battery && battery.GetHasPower();
    }

    public override bool GetIsPickableCompatible(Pickable _Pickable)
    {
        return _Pickable is Battery;
    }

    public override InteractionType GetInteractionType()
    {
        return InteractionType.Trigger;
    }
}