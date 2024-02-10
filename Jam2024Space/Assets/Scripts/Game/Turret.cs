using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField]
    private BatteryReceptor m_BatteryReceptor = null;

    [SerializeField]
    private float m_ShootDelay = 3f;

    [SerializeField]
    private float m_AutoAimRange = 5f;

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        DrawGizmosCircle(transform.position, m_AutoAimRange, 32, Color.red);
    }

    private void DrawGizmosCircle(Vector3 _Position, float _Radius, int _SegmentsCount, Color _Color)
    {
        Gizmos.color = _Color;

        float theta = 0f;
        float deltaTheta = 2f * Mathf.PI / _SegmentsCount;
        Vector3 prevPoint = Vector3.zero;

        for (int i = 0; i <= _SegmentsCount; i++)
        {
            float x = _Radius * Mathf.Cos(theta);
            float z = _Radius * Mathf.Sin(theta);

            Vector3 currentPoint = _Position + new Vector3(x, 0f, z);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, currentPoint);
            }

            prevPoint = currentPoint;
            theta += deltaTheta;
        }
    }
}