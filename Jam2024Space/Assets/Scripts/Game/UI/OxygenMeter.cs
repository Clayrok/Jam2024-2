using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenMeter : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ProgressBar = null;


    private void Update()
    {
        Vector3 scale = m_ProgressBar.transform.localScale;
        scale.x = GameManager.Get().GetShip().GetRemainingOxygen() / 100f;
        m_ProgressBar.transform.localScale = scale;
    }
}