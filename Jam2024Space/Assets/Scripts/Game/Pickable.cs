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
        Vector3 position = transform.position;
        position.y = m_PosY;
        transform.position = position;
    }

    public void OnPlacedInReceptor()
    {

    }

    public override void Interact()
    {
        
    }
}