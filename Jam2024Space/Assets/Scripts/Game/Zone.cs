using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone : MonoBehaviour
{
    [SerializeField]
    private Zone m_LeftZone = null;

    [SerializeField]
    private Zone m_RightZone = null;

    [SerializeField]
    private Zone m_ForwardZone = null;

    [SerializeField]
    private Zone m_BackwardZone = null;

    [SerializeField]
    private BoxCollider m_BoxCollider = null;

    [SerializeField]
    private float m_LoadDistance = 5f;

    private Ship m_Ship = null;


    private void Update()
    {
        if (m_Ship == null)
        {
            return;
        }

        if (Vector3.Distance(m_BoxCollider.ClosestPoint(m_Ship.transform.position), m_Ship.transform.position) > m_LoadDistance)
        {

        }
    }

    private void OnTriggerExit(Collider _Other)
    {
        Ship ship = _Other.GetComponent<Ship>();
        if (ship != null)
        {
            m_Ship = ship;
        }
    }
}