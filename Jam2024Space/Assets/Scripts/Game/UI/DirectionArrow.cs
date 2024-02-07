using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectionArrow : MonoBehaviour
{
    [SerializeField]
    private Image m_Arrow = null;


    private void Update()
    {
        UpdateVisibility();
        UpdateRotation();
    }

    private void UpdateVisibility()
    {
        Vector3? destinationPosition = GameManager.Get().GetShip().GetDestinationPosition();

        Color arrowColor = m_Arrow.color;
        arrowColor.a = destinationPosition == null ? 0f : 1f;
        m_Arrow.color = arrowColor;
    }

    private void UpdateRotation()
    {
        Ship ship = GameManager.Get().GetShip();
        Vector3? destinationPosition = ship.GetDestinationPosition();
        
        if (destinationPosition != null)
        {
            float angleBetweenShipAndDestination = Vector3.SignedAngle(Vector3.forward, destinationPosition.Value - ship.transform.position, ship.transform.up);
            transform.rotation = Quaternion.AngleAxis(angleBetweenShipAndDestination, ship.transform.up);
        }
    }
}