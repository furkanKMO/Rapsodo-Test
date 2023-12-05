using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SimpleCharacterController : MonoBehaviour
{
    // Hareket h�z� ve fare hassasiyeti gibi temel de�i�kenler
    public float speed = 5.0f;
    public float mouseSensitivity = 2.0f;

    // Karakter kontrolc�s� ve dikey rotasyon de�erleri
    private CharacterController characterController;
    private float verticalRotation = 0.0f;
    private float verticalRotationLimit = 80.0f;

    // Z�plama, e�ilme, yer�ekimi ve dikey h�z gibi fiziksel �zellikler
    private bool onAir = true;
    private bool isCrouching = false;
    private float crouchHeight = 0.3f;
    private float standingHeight = 2.0f;
    public float gravity = 10f;
    private float verticalSpeed;
    public float jumpForce = 5f;

    // Lamba sars�nt�s� i�in de�i�kenler
    public Transform lantern;
    private HingeJoint[] lanternHingeJoints;
    public float maxShakeRotation = 5.0f;
    private Vector3 startPosition;

    // Kazanma ve kaybetme metinleri
    public TMP_Text winText, loseText;
    private int once = 0;

    // Ba�lang��ta yap�lacak i�lemler
    private void Start()
    {
        // Ba�lang�� pozisyonunu kaydet
        startPosition = transform.position;
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // E�er lamba varsa onun mente�e eklemlerini al
        if (lantern != null)
        {
            lanternHingeJoints = lantern.GetComponentsInChildren<HingeJoint>();
        }
    }

    // Her frame'de yap�lacak i�lemler
    private void Update()
    {
        // Hareketi i�le
        HandleMovement();
        // Fare bak���n� i�le
        HandleMouseLook();
        // Z�plamay� i�le
        HandleJump();
        // E�ilmeyi i�le
        HandleCrouch();
    }

    // Lamba sars�nt�s�n� i�le
    private void HandleLanternShake()
    {
        if (lanternHingeJoints != null)
        {
            foreach (HingeJoint hingeJoint in lanternHingeJoints)
            {
                // Lamban�n mente�e eklemlerine rastgele d�nd�rme uygula
                JointSpring jointSpring = hingeJoint.spring;
                jointSpring.targetPosition += Random.Range(-maxShakeRotation, maxShakeRotation);
                hingeJoint.spring = jointSpring;
            }
        }
    }

    // Hareketi i�le
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0.0f, verticalInput);
        moveDirection = transform.TransformDirection(moveDirection);
        moveDirection *= speed;

        characterController.Move(moveDirection * Time.deltaTime);

        // Lamba sars�nt�s�n� i�le
        HandleLanternShake();
    }

    // Fare bak���n� i�le
    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    // Z�plamay� i�le
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

                // Lamba sars�nt�s�n� i�le
                HandleLanternShake();
            }
        }

        Vector3 verticalMovement = new Vector3(0f, verticalSpeed, 0f);
        characterController.Move(verticalMovement * Time.deltaTime);

        verticalSpeed -= gravity * Time.deltaTime;
    }

    // E�ilmeyi i�le
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

    // Kontrolc� �arp��mas�yla ilgili i�lemleri i�le
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Yerdeyken z�plama s�n�rlar�n� s�f�rla
        if (hit.collider.CompareTag("Ground"))
        {
            if (hit.moveDirection.y < -0.3)
            {
                onAir = false;
                maxShakeRotation = 5;
            }
        }

        // Bas�n� levhas� etkile�imi
        if (hit.collider.CompareTag("Plate"))
        {
            hit.gameObject.GetComponent<PressurePlate>().pressed = true;
        }

        // �l�m alan�nda kaybetme durumu
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

    // Tetiklenme alan�nda kazanma durumu
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

    // Oyun durumunu s�f�rla
    private void Reset()
    {
        characterController.enabled = false;
        characterController.transform.position = Vector3.zero;
        characterController.enabled = true;
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        once = 0;
    }

    // Belirli bir s�re sonra oyunu s�f�rla
    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(1);
        Reset();
    }
}
