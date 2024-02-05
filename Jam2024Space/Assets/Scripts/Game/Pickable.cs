using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickable : Interactable
{
    private float m_PosY = 0f;


    public void OnPicked()
    {
        m_PosY = transform.position.y;
    }

    public void OnDropped()
    {
        
    }

    public override void Interact()
    {
        
    }
}