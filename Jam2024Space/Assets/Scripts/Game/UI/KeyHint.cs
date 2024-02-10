using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class KeyHint : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float m_VisibleOpacity = 0.6f;

    private Image m_Image = null;

    private GameObject m_GameObjectToHint = null;


    private void Awake()
    {
        m_Image = GetComponent<Image>();
    }

    private void Update()
    {
        UpdateGameObjectToHint();
    }

    private void LateUpdate()
    {
        if (m_GameObjectToHint == null)
        {
            return;
        }

        transform.position = GameManager.Get().GetGameCamera().GetUnityCamera().WorldToScreenPoint(m_GameObjectToHint.transform.position);
    }

    private void UpdateGameObjectToHint()
    {
        Interactable interactable = GameManager.Get().GetCharacter().GetInteractableInRange();
        SetKeyHintGameObject(interactable ? interactable.gameObject : null);
    }

    private void SetKeyHintGameObject(GameObject _GameObject)
    {
        if (m_GameObjectToHint != _GameObject)
        {
            Color color = m_Image.color;
            color.a = _GameObject != null ? m_VisibleOpacity : 0f;
            m_Image.color = color;

            m_GameObjectToHint = _GameObject;
        }
    }
}