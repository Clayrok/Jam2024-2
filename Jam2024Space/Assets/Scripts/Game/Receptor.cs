using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Receptor : Interactable
{
    [SerializeField]
    private GameObject m_PlacementPoint = null;

    private Interactable m_PlacedInteractable = null;

    private bool m_IsInteractedKinematicWhenPlaced = false;


    public bool TryPlaceInteractable(Interactable _Interactable)
    {
        if (!GetIsInteractableCompatible(_Interactable))
        {
            return false;
        }

        m_PlacedInteractable = _Interactable;

        if (_Interactable.TryGetComponent(out Rigidbody rigidbody))
        {
            m_IsInteractedKinematicWhenPlaced = rigidbody.isKinematic;
            rigidbody.isKinematic = true;
        }

        _Interactable.transform.position = m_PlacementPoint.transform.position;
        _Interactable.transform.rotation = m_PlacementPoint.transform.rotation;

        return true;
    }

    public Interactable TakeInteractable()
    {
        if (!m_PlacedInteractable)
        {
            return null;
        }

        if (m_PlacedInteractable.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = m_IsInteractedKinematicWhenPlaced;
        }

        Interactable interactable = m_PlacedInteractable;
        m_PlacedInteractable = null;

        return interactable;
    }

    public abstract bool GetIsInteractableCompatible(Interactable _Interactable);

    public T GetPlacedInteractable<T>() where T : Interactable
    {
        return m_PlacedInteractable as T;
    }

    public bool GetIsEmpty()
    {
        return m_PlacedInteractable == null;
    }
}