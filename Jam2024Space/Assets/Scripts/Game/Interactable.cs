using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Get().RegisterInteractable(this);
    }

    public abstract void Interact();
}