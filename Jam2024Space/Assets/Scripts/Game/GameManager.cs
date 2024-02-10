using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_Instance = null;

    [SerializeField]
    private PlayerCharacter m_Character = null;

    [SerializeField]
    private Ship m_Ship = null;

    [SerializeField]
    private GameCamera m_GameCamera = null;

    [SerializeField]
    private GameUI m_GameUI = null;



    private List<Interactable> m_Interactables = new List<Interactable>();


    public void RegisterInteractable(Interactable _Interactable)
    {
        m_Interactables.Add(_Interactable);
    }

    public Interactable GetClosestInteractable(Vector3 _Position, Interactable _ToExclude = null)
    {
        return GetClosestInteractable<Interactable>(_Position, _ToExclude);
    }

    public T GetClosestInteractable<T>(Vector3 _Position, Interactable _ToExclude = null) where T : Interactable
    {
        T closest = null;
        float shortestDistance = float.PositiveInfinity;

        foreach (Interactable interactable in m_Interactables)
        {
            float sqrMagnitude = Vector3.SqrMagnitude(interactable.transform.position - _Position);
            if (shortestDistance > sqrMagnitude && (_ToExclude == null || _ToExclude != interactable) && interactable is T)
            {
                closest = interactable as T;
                shortestDistance = sqrMagnitude;
            }
        }

        return closest;
    }

    public List<Interactable> GetInteractablesInRange(Vector3 _Position, float _Range)
    {
        List<Interactable> interactablesInRange = new List<Interactable>();

        foreach (Interactable interactable in m_Interactables)
        {
            if (Vector3.Distance(_Position, interactable.transform.position) < _Range)
            {
                interactablesInRange.Add(interactable);
            }
        }

        return interactablesInRange;
    }

    public T GetClosestInteractableInRange<T>(Vector3 _Position, float _Range) where T : Interactable
    {
        T interactable = GetClosestInteractable<T>(_Position);

        if(Vector3.Distance(_Position, interactable.transform.position) < _Range)
        {
            return interactable;
        }

        return null;
    }

    public PlayerCharacter GetCharacter()
    {
        return m_Character;
    }

    public Ship GetShip()
    {
        return m_Ship;
    }

    public GameCamera GetGameCamera()
    {
        return m_GameCamera;
    }

    public GameUI GetGameUI()
    {
        return m_GameUI;
    }

    public static GameManager Get()
    {
        if (s_Instance == null)
        {
            s_Instance = FindObjectOfType<GameManager>();
        }

        return s_Instance;
    }
}