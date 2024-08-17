using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 2f;
    public float movementSpeed = 5f;
    private float verticalRotation = 0f;

    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }


    void Update()
    {
        // MOVEMENT

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(x, z).normalized * movementSpeed * Time.deltaTime;

        controller.Move(transform.right * movement.x + transform.forward * movement.y);

        // ROTATION

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cam.transform.localEulerAngles = Vector3.right * verticalRotation;

        transform.Rotate(Vector3.up * mouseX);
    }
}
