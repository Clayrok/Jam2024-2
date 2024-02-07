using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ProgressBar = null;


    private void Update()
    {
        Vector3 scale = m_ProgressBar.transform.localScale;
        scale.y = GameManager.Get().GetCharacter().GetRemainingHealth() / 100f;
        m_ProgressBar.transform.localScale = scale;
    }

}