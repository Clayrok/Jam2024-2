using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    private BatteryReceptor m_BatteryReceptor = null;

    [SerializeField]
    private float m_ShootDelay = 3f;

    private float m_LastShootTime = float.NegativeInfinity;


    private void Update()
    {
        if (m_BatteryReceptor == null || !m_BatteryReceptor.GetIsPowered())
        {
            return;
        }

        if (Time.time - m_LastShootTime > m_ShootDelay)
        {
            m_LastShootTime = Time.time;
            Shoot();
        }
    }

    private void Shoot()
    {
        Debug.Log("Shoot!");
    }
}