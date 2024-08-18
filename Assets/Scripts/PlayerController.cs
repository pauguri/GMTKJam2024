using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 2f;
    public float movementSpeed = 5f;
    private float verticalRotation = -70f;
    [NonSerialized] public bool inputActive = true;
    private Vector3 startPosition = Vector3.zero;

    [NonSerialized] public bool hasLooked = false;
    [NonSerialized] public bool hasMoved = false;

    private CharacterController controller;

    private void Awake()
    {
        startPosition = transform.position;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (!inputActive)
        {
            return;
        }

        // MOVEMENT

        if (hasLooked)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector2 movement = new Vector2(x, z).normalized * movementSpeed * Time.deltaTime;

            controller.Move(transform.right * movement.x + transform.forward * movement.y);
        }

        // ROTATION

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cam.transform.localEulerAngles = Vector3.right * verticalRotation;

        transform.Rotate(Vector3.up * mouseX);

        // FIRST TIME CHECKS

        if (!hasLooked)
        {
            if (mouseX != 0 || mouseY != 0)
            {
                hasLooked = true;
                print("looking done");
            }
        }
        else if (!hasMoved && Vector3.Distance(startPosition, transform.position) > 15f)
        {
            hasMoved = true;
            print("moving done");
            if (hasLooked)
            {
                ThreeDSceneLogic.Instance.EndTutorialTime();
            }
        }
    }


    public void ResetPosition()
    {
        controller.enabled = false;
        transform.position = startPosition;
        verticalRotation = -70f;
        cam.transform.localEulerAngles = Vector3.zero;

        controller.enabled = true;
    }
}
