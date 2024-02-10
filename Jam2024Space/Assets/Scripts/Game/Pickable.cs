using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickable : Interactable
{
    private float m_PosY = 0f;

    private bool m_IsInReceptor = false;


    public void OnPicked()
    {
        m_PosY = transform.position.y;
    }

    public void OnDropped()
    {
        
    }

    public override void Interact(PlayerCharacter _Player)
    {
        
    }

    public override InteractionType GetInteractionType()
    {
        return InteractionType.Trigger;
    }

    public void SetIsInReceptor(bool _State)
    {
        m_IsInReceptor = _State;
    }

    public bool IsInReceptor()
    {
        return m_IsInReceptor;
    }
}