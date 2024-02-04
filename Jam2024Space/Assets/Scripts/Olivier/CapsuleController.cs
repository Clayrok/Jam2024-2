using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleController : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float actionDuration = 2f;
    [SerializeField] private float fetchDuration = 1f;

    [SerializeField] private CharacterController c_characterController;
    [SerializeField] private Collider c_FetchRange;
    [SerializeField] private Rigidbody c_rigidbody;

    [SerializeField] private GameObject currentObject;
    [SerializeField] public GameObject fetchPosition;

    [SerializeField] private bool s_hasBattery = false;
    [SerializeField] private bool actionInRange = false;
    [SerializeField] private bool objectInRange = false;
    [SerializeField] private bool batterySlotInRange = false;
    [SerializeField] private bool hasAnObjectEquipped = false;

    [SerializeField] private bool s_isUnderGravity = false;

    [SerializeField] private bool s_IscontrollerActive = true;



    void Start()
    {
        c_characterController = GetComponent<CharacterController>();
        c_rigidbody = GetComponent<Rigidbody>();
        c_FetchRange = GetComponent<BoxCollider>();  
    }

    void Update()
    {
        if (s_IscontrollerActive == true)
        {
            MoveCharacter();
            if (Input.GetButtonDown("Fire1") == true)
            {
                UseActionButton();
            }
        }
    }

    void MoveCharacter()
    {
        
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
            movement.Normalize();

            if (s_hasBattery == true)
                c_characterController.SimpleMove(movement * speed * 0.75f);
            else
                c_characterController.SimpleMove(movement * speed);

            if (movement != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 1000f);
            }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Objet")
        {
            Debug.Log("Il y a un objet");
            objectInRange = true;
            currentObject = other.gameObject;
        }
        if (other.tag == "Activité")
        {
            actionInRange = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Objet")
        {
            Debug.Log("Il y a un objet");
            objectInRange = false;
        }
        if (other.tag == "Activité")
        {
            actionInRange = false;

        }
    }

    void UseActionButton()
    {     

            if (actionInRange == true && objectInRange == true && hasAnObjectEquipped == false)
            {
                Debug.Log("Je prend l'objet");
                FetchItem();
                hasAnObjectEquipped = true;
            }
            if (actionInRange == true && objectInRange == false && hasAnObjectEquipped == false)
            {

            }
            if (actionInRange == false && objectInRange == true && hasAnObjectEquipped == false)
            {
                Debug.Log("Je prend l'objet");
                FetchItem();
                hasAnObjectEquipped = true;
            }
            
        
    }

    void FetchItem()
    {
        float timer = 0f;
        s_IscontrollerActive = false;
        c_rigidbody.velocity = new Vector3(0, 0, 0);
        currentObject.GetComponent<BoxCollider>().enabled = false;
        currentObject.GetComponent<Rigidbody>().isKinematic = true;
        currentObject.transform.SetParent(this.transform);
        timer = timer + Time.deltaTime;
        //StartCoroutine(FetchItemCoroutine());
        Debug.Log(currentObject.transform.position+"current obj pos");
        Debug.Log(fetchPosition.transform.position + "current player pos");
        //currentObject.transform.position = Vector3.Lerp(new Vector3(currentObject.transform.position.x, currentObject.transform.position.y, currentObject.transform.position.z),new Vector3(fetchPosition.transform.position.x, fetchPosition.transform.position.y, fetchPosition.transform.position.z),Time.deltaTime);
        //currentObject.transform.rotation = Quaternion.Lerp(currentObject.transform.rotation,fetchPosition.transform.rotation,Time.deltaTime);
    }
    IEnumerator FetchItemCoroutine()
    {
        

        yield return new WaitForSeconds(fetchDuration);

        s_IscontrollerActive = true;



    }

    void DropItem()
    {
        //enlever le statut enfant à l'objet
        //activer rigidbody objet
        //petite force random
    }
}
