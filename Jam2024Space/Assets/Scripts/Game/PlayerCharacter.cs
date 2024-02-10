using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class PlayerCharacter : MonoBehaviour
{
    [SerializeField]
    private CapsuleCollider m_CapsuleCollider;

    [SerializeField]
    public GameObject m_FetchPosition;

    [SerializeField]
    private float m_BaseSpeed = 5f;

    [SerializeField]
    private float m_RotationSpeed = 100f;

    [SerializeField]
    private AnimationCurve m_AccelerationCurve;

    [SerializeField]
    private float m_ActionDuration = 2f;
    
    [SerializeField]
    private float m_FetchDuration = 1f;

    [SerializeField]
    private float m_InteractionRange = 1f;

    [SerializeField]
    private float m_Health = 100f;

    [SerializeField]
    private float m_Hunger = 100f;

    [SerializeField]
    private float m_HungerConsumption = 1f;

    [SerializeField]
    private float m_HungerRefillSpeed = 3f;

    private float m_CurrentSpeed = 5f;

    private bool m_CanMove = true;

    private Pickable m_PickedObject = null;

    private float m_AccelerationTime = 0f;

    private bool m_IsMoving = false;

    private Interactable m_InteractableInRange = null;

    private Interactable m_CurrentStayInteractable = null;


    private void Start()
    {
        m_CurrentSpeed = m_BaseSpeed;
    }

    private void Update()
    {
        if (m_CanMove)
        {
            Move();

            if (!m_CurrentStayInteractable && InputManager.Interact)
            {
                Interact();
            }
        }

        MovePickedObject();
        ConsumeHunger();
        UpdateInteractable();
    }

    private void UpdateInteractable()
    {
        if (m_CurrentStayInteractable)
        {
            m_InteractableInRange = null;

            if (m_IsMoving)
            {
                m_CurrentStayInteractable.StopInteraction(this);
                m_CurrentStayInteractable = null;
            }
        }
        else
        {
            m_InteractableInRange = GetPrioritaryInteractableInRange();
        }
    }

    private void Move()
    {
        float verticalInput = (InputManager.Forward ? 1 : 0) + (InputManager.Backward ? -1 : 0);
        float horizontalInput = (InputManager.Right ? 1 : 0) + (InputManager.Left ? -1 : 0);

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, GameManager.Get().GetShip().transform.forward);
        movement = rotation * movement;

        if (movement.sqrMagnitude > 0)
        {
            m_AccelerationTime += Time.deltaTime;

            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * m_RotationSpeed);
        }
        else
        {
            m_AccelerationTime = 0f;
        }

        float acceleration = m_AccelerationCurve.Evaluate(m_AccelerationTime);
        movement = movement * acceleration * m_CurrentSpeed * Time.deltaTime;

        ComputeSphereCastMovement(ref movement);

        transform.Translate(movement, Space.World);

        m_IsMoving = movement.sqrMagnitude > 0;
    }

    private void ComputeSphereCastMovement(ref Vector3 _Movement)
    {
        if (_Movement.sqrMagnitude == 0)
        {
            return;
        }

        float capsuleColliderRadius = m_CapsuleCollider.radius;
        Vector3 capsuleBottomSpherePos = transform.position - transform.up * (m_CapsuleCollider.height / 2f - capsuleColliderRadius);
        Vector3 capsuleTopSpherePos = transform.position + transform.up * (m_CapsuleCollider.height / 2f - capsuleColliderRadius);

        if (Physics.CapsuleCast(capsuleBottomSpherePos, capsuleTopSpherePos, capsuleColliderRadius, _Movement.normalized, out RaycastHit hitInfo, _Movement.magnitude, ~(LayerMask.GetMask("Player"))))
        {
            _Movement += hitInfo.normal * _Movement.magnitude;
        }
    }

    private void Interact()
    {
        if (!m_InteractableInRange)
        {
            if (m_PickedObject)
            {
                DropObject();
            }

            return;
        }

        m_InteractableInRange.Interact(this);
        if (m_InteractableInRange.GetInteractionType() == Interactable.InteractionType.Stay)
        {
            m_CurrentStayInteractable = m_InteractableInRange;
        }

        if (!m_PickedObject)
        {
            if (m_InteractableInRange is Pickable)
            {
                PickObject(m_InteractableInRange as Pickable);
            }

            if (m_InteractableInRange is Receptor && !(m_InteractableInRange as Receptor).GetIsEmpty())
            {
                PickObject((m_InteractableInRange as Receptor).TakePickable() as Pickable);
            }
        }
        else
        {
            DropObject();
        }
    }

    private Interactable GetPrioritaryInteractableInRange()
    {
        Interactable prioritaryInteractable = null;
        List<Interactable> interactablesInRange = GameManager.Get().GetInteractablesInRange(transform.position, m_InteractionRange);

        if (m_PickedObject)
        {
            prioritaryInteractable = GetClosestInteractableOfType<Receptor>(interactablesInRange);
        }
        else
        {
            prioritaryInteractable = GetClosestInteractableOfType<Receptor>(interactablesInRange, (Receptor _Receptor) =>
            {
                return !_Receptor.GetIsEmpty();
            });

            if (!prioritaryInteractable)
            {
                prioritaryInteractable = GetClosestInteractableOfType<Interactable>(interactablesInRange, (Interactable _Interactable) =>
                {
                    bool isReceptor = _Interactable is Receptor;
                    bool isPickable = _Interactable is Pickable;

                    return !isReceptor && !isPickable;
                });
            }

            if (!prioritaryInteractable)
            {
                prioritaryInteractable = GetClosestInteractableOfType<Pickable>(interactablesInRange, (Pickable _Pickable) =>
                {
                    return !_Pickable.IsInReceptor();
                });
            }
        }

        return prioritaryInteractable;
    }

    private T GetClosestInteractableOfType<T>(List<Interactable> _Interactables, System.Func<T, bool> _Condition = null) where T : Interactable
    {
        T closestInteractable = null;
        float shortestSqrMagnitude = float.PositiveInfinity;

        foreach (Interactable interactable in _Interactables)
        {
            float sqrMagnitude = (interactable.transform.position - transform.position).sqrMagnitude;
            if (interactable is T && sqrMagnitude < shortestSqrMagnitude && (_Condition == null || _Condition.Invoke(interactable as T)))
            {
                closestInteractable = interactable as T;
                shortestSqrMagnitude = sqrMagnitude;
            }
        }

        return closestInteractable;
    }

    private void PickObject(Pickable _Object)
    {
        m_PickedObject = _Object;
        _Object.OnPicked();

        if (m_PickedObject.TryGetComponent(out Collider collider))
        {
            collider.isTrigger = true;
        }

        m_CurrentSpeed = m_BaseSpeed * 0.75f;
    }

    private void DropObject()
    {
        if (m_PickedObject == null)
        {
            return;
        }

        if (m_PickedObject.TryGetComponent(out Collider collider))
        {
            collider.isTrigger = false;
        }

        Receptor closestReceptor = GameManager.Get().GetClosestInteractableInRange<Receptor>(transform.position, m_InteractionRange);
        if (closestReceptor)
        {
            if (!closestReceptor.TryPlacePickable(m_PickedObject))
            {
                PlacePickableOnFloor(m_PickedObject);
            }
        }
        else
        {
            PlacePickableOnFloor(m_PickedObject);
        }

        m_PickedObject.OnDropped();
        m_PickedObject = null;

        m_CurrentSpeed = m_BaseSpeed;
    }

    private void MovePickedObject()
    {
        if (!m_PickedObject)
        {
            return;
        }

        m_PickedObject.transform.position = m_FetchPosition.transform.position;
        m_PickedObject.transform.rotation = m_FetchPosition.transform.rotation;
    }

    private void PlacePickableOnFloor(Pickable _Pickable)
    {
        MeshRenderer playerMeshRenderer = GetComponent<MeshRenderer>();
        MeshRenderer pickableMeshRenderer = _Pickable.GetComponent<MeshRenderer>();

        Vector3 feetPosition = transform.position + Vector3.down * (playerMeshRenderer.bounds.extents.y);
        float pickableHalfHeight = pickableMeshRenderer.bounds.extents.y;

        _Pickable.transform.position = feetPosition + Vector3.up * pickableHalfHeight;
    }

    private void ConsumeHunger()
    {
        m_Hunger = Mathf.Clamp(m_Hunger - m_HungerConsumption * Time.deltaTime, 0f, 100f);
    }

    public void Feed()
    {
        m_Hunger = Mathf.Clamp(m_Hunger + m_HungerRefillSpeed * Time.deltaTime, 0f, 100f);
    }

    public Interactable GetInteractableInRange()
    {
        return m_InteractableInRange;
    }

    public void Damage(float _Damage)
    {
        m_Health = Mathf.Clamp(m_Health - _Damage, 0f, 100f);
    }

    public float GetRemainingHealth()
    {
        return m_Health;
    }

    public float GetRemainingHunger()
    {
        return m_Hunger;
    }

    public bool GetIsMoving()
    {
        return m_IsMoving;
    }
}