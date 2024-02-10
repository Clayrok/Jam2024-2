using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public enum InteractionType
    {
        Trigger,
        Stay
    }

    private void Awake()
    {
        GameManager.Get().RegisterInteractable(this);
    }

    public virtual void Interact(PlayerCharacter _Player) { }

    public virtual void StopInteraction(PlayerCharacter _Player) { }

    public abstract InteractionType GetInteractionType();
}