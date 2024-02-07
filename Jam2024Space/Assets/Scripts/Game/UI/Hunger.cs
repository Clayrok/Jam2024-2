using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger : MonoBehaviour
{
    [SerializeField]
    private GameObject m_ProgressBar = null;


    private void Update()
    {
        Vector3 scale = m_ProgressBar.transform.localScale;
        scale.y = GameManager.Get().GetCharacter().GetRemainingHunger() / 100f;
        m_ProgressBar.transform.localScale = scale;
    }
}