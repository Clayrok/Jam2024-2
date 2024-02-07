using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryReceptor : Receptor
{
    private float m_PowerConsumption = 0f;


    private void Update()
    {
        Battery battery = GetPlacedInteractable<Battery>();
        if (battery)
        {
            battery.Drain(m_PowerConsumption * Time.deltaTime);
        }
    }

    public override void Interact(PlayerCharacter _Player)
    {
        
    }

    public bool GetIsPowered()
    {
        Battery battery = GetPlacedInteractable<Battery>();
        return battery && battery.GetHasPower();
    }

    public override bool GetIsInteractableCompatible(Interactable _Interactable)
    {
        return _Interactable is Battery;
    }
}