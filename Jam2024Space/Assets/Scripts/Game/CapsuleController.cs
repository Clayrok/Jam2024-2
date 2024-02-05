using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    [SerializeField]
    private CharacterController c_characterController;

    [SerializeField]
    private Collider c_FetchRange;

    [SerializeField]
    private Rigidbody c_rigidbody;

    [SerializeField]
    private BoxCollider m_Collider;

    [SerializeField]
    public GameObject fetchPosition;

    [SerializeField]
    private float baseSpeed = 5f;
    
    [SerializeField]
    private float actionDuration = 2f;
    
    [SerializeField]
    private float fetchDuration = 1f;

    private float m_CurrentSpeed = 5f;

    private bool s_hasBattery = false;
    
    private bool actionInRange = false;
    
    private bool objectInRange = false;
    
    private bool batterySlotInRange = false;
    
    private bool hasAnObjectEquipped = false;

    private bool s_isUnderGravity = false;

    private bool s_IsControllerActive = true;

    private Interactable m_InteractableInRange = null;

    private Pickable m_PickedObject = null;


    void Start()
    {
        c_characterController = GetComponent<CharacterController>();
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
    }

    void MoveCharacter()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput).normalized * m_CurrentSpeed * Time.deltaTime;

        c_characterController.Move(movement);

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 1000f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            m_InteractableInRange = interactable;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_InteractableInRange != null && other.gameObject == m_InteractableInRange.gameObject)
        {
            m_InteractableInRange = null;
        }
    }

    private void Interact()
    {
        if (!m_PickedObject && m_InteractableInRange != null)
        {
            m_InteractableInRange.Interact();

            if (m_InteractableInRange is Pickable)
            {
                PickObject(m_InteractableInRange as Pickable);
            }
        }
        else
        {
            DropObject();
        }
    }

    private void PickObject(Pickable _Object)
    {
        m_PickedObject = _Object;

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

        if (m_InteractableInRange is BatteryReceptor && m_PickedObject is Battery)
        {
            ((BatteryReceptor)m_InteractableInRange).PlaceBattery(m_PickedObject as Battery);
        }

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
}
