using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleCharacterController : MonoBehaviour
{
    // Hareket hýzý ve fare hassasiyeti gibi temel deðiþkenler
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;

    // Karakter kontrolcüsü ve dikey rotasyon deðerleri
    private CharacterController characterController;
    private float verticalRotation = 0.0f;
    private float verticalRotationLimit = 80.0f;

    // Zýplama, eðilme, yerçekimi ve dikey hýz gibi fiziksel özellikler
    private bool onAir = true;
    private bool isCrouching = false;
    private float crouchHeight = 0.3f;
    private float standingHeight = 2.0f;
    public float gravity = 10f;
    private float verticalSpeed;
    public float jumpForce = 5f;

    // Lamba sarsýntýsý için deðiþkenler
    public Transform lantern;
    private HingeJoint[] lanternHingeJoints;
    public float maxShakeRotation = 5.0f;
    private Vector3 startPosition;

    // Kazanma ve kaybetme metinleri
    public TMP_Text winText, loseText;
    private int once = 0;

    // Baþlangýçta yapýlacak iþlemler
    private void Start()
    {
        // Baþlangýç pozisyonunu kaydet
        startPosition = transform.position;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // Eðer lamba varsa onun menteþe eklemlerini al
        if (lantern != null)
        {
            lanternHingeJoints = lantern.GetComponentsInChildren<HingeJoint>();
        }
    }

    // Her frame'de yapýlacak iþlemler
    private void Update()
    {
        // Hareketi iþle
        HandleMovement();
        // Fare bakýþýný iþle
        HandleMouseLook();
        // Zýplamayý iþle
        HandleJump();
        // Eðilmeyi iþle
        HandleCrouch();
    }

    // Lamba sarsýntýsýný iþle
    private void HandleLanternShake()
    {
        if (lanternHingeJoints != null)
        {
            foreach (HingeJoint hingeJoint in lanternHingeJoints)
            {
                // Lambanýn menteþe eklemlerine rastgele döndürme uygula
                JointSpring jointSpring = hingeJoint.spring;
                jointSpring.targetPosition += Random.Range(-maxShakeRotation, maxShakeRotation);
                hingeJoint.spring = jointSpring;
            }
        }
    }

    // Hareketi iþle
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        characterController.Move(moveDirection * Time.deltaTime);

        // Lamba sarsýntýsýný iþle
        HandleLanternShake();
    }

    // Fare bakýþýný iþle
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // Zýplamayý iþle
    private void HandleJump()
    {
        bool jump = Input.GetKeyDown(KeyCode.Space);

        if (!onAir)
        {
            if (jump)
            {
                verticalSpeed = jumpForce;
                onAir = true;
                maxShakeRotation = 50;

                // Lamba sarsýntýsýný iþle
                HandleLanternShake();
            }
        }

        Vector3 verticalMovement = new Vector3(0f, verticalSpeed, 0f);
        characterController.Move(verticalMovement * Time.deltaTime);

        verticalSpeed -= gravity * Time.deltaTime;
    }

    // Eðilmeyi iþle
    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (!isCrouching)
            {
                isCrouching = true;
                characterController.height = crouchHeight;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (isCrouching)
            {
                isCrouching = false;
                characterController.height = standingHeight;
            }
        }
    }

    // Kontrolcü çarpýþmasýyla ilgili iþlemleri iþle
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Yerdeyken zýplama sýnýrlarýný sýfýrla
        if (hit.collider.CompareTag("Ground"))
        {
            if (hit.moveDirection.y < -0.3)
            {
                onAir = false;
                maxShakeRotation = 5;
            }
        }

        // Basýnç levhasý etkileþimi
        if (hit.collider.CompareTag("Plate"))
        {
            hit.gameObject.GetComponent<PressurePlate>().pressed = true;
        }

        // Ölüm alanýnda kaybetme durumu
        if (hit.collider.CompareTag("Death"))
        {
            loseText.gameObject.SetActive(true);
            if (once == 0)
            {
                once++;
                StartCoroutine(Waiter());
            }
        }
    }

    // Tetiklenme alanýnda kazanma durumu
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("END"))
        {
            winText.gameObject.SetActive(true);
            if (once == 0)
            {
                once++;
                StartCoroutine(Waiter());
            }
        }
    }

    // Oyun durumunu sýfýrla
    private void Reset()
    {
        characterController.enabled = false;
        characterController.transform.position = Vector3.zero;
        characterController.enabled = true;
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        once = 0;
    }

    // Belirli bir süre sonra oyunu sýfýrla
    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(1);
        Reset();
    }
}
