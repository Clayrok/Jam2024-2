using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : Interactable
{
    private Vector3 m_InteractionStartShipPosition = Vector3.zero;

    private PlayerCharacter m_InteractionPlayer = null;


    private void Update()
    {
        if (m_InteractionPlayer == null)
        {
            return;
        }

        if (m_InteractionStartShipPosition != m_InteractionPlayer.transform.localPosition)
        {
            m_InteractionPlayer = null;
        }
        else
        {
            m_InteractionPlayer.Feed();
        }
    }

    public override void Interact(PlayerCharacter _Player)
    {
        m_InteractionStartShipPosition = _Player.transform.localPosition;
        m_InteractionPlayer = _Player;
    }
}