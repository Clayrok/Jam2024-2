using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Receptor : Interactable
{
    [SerializeField]
    private GameObject m_PlacementPoint = null;

    private Pickable m_PlacedPickable = null;

    private bool m_WasPickableKinematicWhenPlaced = false;


    public bool TryPlacePickable(Pickable _Pickable)
    {
        if (!GetIsPickableCompatible(_Pickable))
        {
            return false;
        }

        m_PlacedPickable = _Pickable;

        if (_Pickable.TryGetComponent(out Rigidbody rigidbody))
        {
            m_WasPickableKinematicWhenPlaced = rigidbody.isKinematic;
            rigidbody.isKinematic = true;
        }

        _Pickable.transform.position = m_PlacementPoint.transform.position;
        _Pickable.transform.rotation = m_PlacementPoint.transform.rotation;

        _Pickable.SetIsInReceptor(true);

        return true;
    }

    public Pickable TakePickable()
    {
        if (!m_PlacedPickable)
        {
            return null;
        }

        if (m_PlacedPickable.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = m_WasPickableKinematicWhenPlaced;
        }

        Pickable pickable = m_PlacedPickable;
        m_PlacedPickable = null;
        pickable.SetIsInReceptor(false);

        return pickable;
    }

    public abstract bool GetIsPickableCompatible(Pickable _Pickable);

    public Pickable GetPlacedPickable()
    {
        return m_PlacedPickable;
    }

    public bool GetIsEmpty()
    {
        return m_PlacedPickable == null;
    }
}