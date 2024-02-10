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
    private Collider c_FetchRange;

    [SerializeField]
    private Rigidbody c_rigidbody;

    [SerializeField]
    private CapsuleCollider m_CapsuleCollider;

    [SerializeField]
    public GameObject fetchPosition;

    [SerializeField]
    private float baseSpeed = 5f;

    [SerializeField]
    private float m_RotationSpeed = 100f;

    [SerializeField]
    private AnimationCurve m_AccelerationCurve;

    [SerializeField]
    private float actionDuration = 2f;
    
    [SerializeField]
    private float fetchDuration = 1f;

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

    private bool s_hasBattery = false;
    
    private bool actionInRange = false;
    
    private bool objectInRange = false;
    
    private bool batterySlotInRange = false;
    
    private bool hasAnObjectEquipped = false;

    private bool s_isUnderGravity = false;

    private bool s_IsControllerActive = true;

    private Pickable m_PickedObject = null;

    private float m_AccelerationTime = 0f;

    private bool m_IsMoving = false;


    void Start()
    {
        c_rigidbody = GetComponent<Rigidbody>();
        c_FetchRange = GetComponent<BoxCollider>();

        m_CurrentSpeed = baseSpeed;
    }

    void Update()
    {
        if (s_IsControllerActive == true)
        {
            MoveCharacter();
            if (Input.GetKeyDown(KeyCode.E) == true)
            {
                Interact();
            }
        }

        MovePickedObject();
        ConsumeHunger();
    }

    void MoveCharacter()
    {
        float verticalInput = (InputManager.Forward ? 1 : 0) + (InputManager.Backward ? -1 : 0);
        float horizontalInput = (InputManager.Right ? 1 : 0) + (InputManager.Left ? -1 : 0);

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (movement.sqrMagnitude > 0)
        {
            m_AccelerationTime += Time.deltaTime;
        }
        else
        {
            m_AccelerationTime = 0f;
        }

        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, GameManager.Get().GetShip().transform.forward);
        movement = rotation * movement;

        float acceleration = m_AccelerationCurve.Evaluate(m_AccelerationTime);
        movement = movement * acceleration * m_CurrentSpeed * Time.deltaTime;

        ComputeSphereCastMovement(ref movement);

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * m_RotationSpeed);
        }

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
        Interactable closestPrioritaryInteractable = GetPrioritaryInteractableInRange();

        if (!closestPrioritaryInteractable)
        {
            if (m_PickedObject)
            {
                DropObject();
            }

            return;
        }

        closestPrioritaryInteractable.Interact(this);

        if (!m_PickedObject)
        {
            if (closestPrioritaryInteractable is Pickable)
            {
                PickObject(closestPrioritaryInteractable as Pickable);
            }

            if (closestPrioritaryInteractable is Receptor && !(closestPrioritaryInteractable as Receptor).GetIsEmpty())
            {
                PickObject((closestPrioritaryInteractable as Receptor).TakeInteractable() as Pickable);
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
                prioritaryInteractable = GetClosestInteractableOfType<Pickable>(interactablesInRange);
            }

            if (!prioritaryInteractable)
            {
                prioritaryInteractable = GetClosestInteractableOfType<Interactable>(interactablesInRange);
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

        m_CurrentSpeed = baseSpeed * 0.75f;
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
            if (!closestReceptor.TryPlaceInteractable(m_PickedObject))
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

        m_CurrentSpeed = baseSpeed;
    }

    private void MovePickedObject()
    {
        if (!m_PickedObject)
        {
            return;
        }

        m_PickedObject.transform.position = fetchPosition.transform.position;
        m_PickedObject.transform.rotation = fetchPosition.transform.rotation;
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

    private bool IsInteractableInRange(Interactable _Interactable)
    {
        return Vector3.Distance(_Interactable.transform.position, transform.position) < m_InteractionRange;
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