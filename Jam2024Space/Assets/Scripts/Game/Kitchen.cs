using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kitchen : Interactable
{
    private PlayerCharacter m_InteractionPlayer = null;


    private void Update()
    {
        if (m_InteractionPlayer == null)
        {
            return;
        }

        m_InteractionPlayer.Feed();
    }

    public override void Interact(PlayerCharacter _Player)
    {
        m_InteractionPlayer = _Player;
    }

    public override void StopInteraction(PlayerCharacter _Player)
    {
        m_InteractionPlayer = null;
    }

    public override InteractionType GetInteractionType()
    {
        return InteractionType.Stay;
    }
}