using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float changeCellDistance = 10f;
    public float movementSpeed = 5f;
    private float verticalRotation = -90f;
    [NonSerialized] public bool inputActive = true;
    private Vector3 startPosition = Vector3.zero;
    [NonSerialized] public Vector2Int currentHex = Vector2Int.zero;

    // tutorial checks
    [NonSerialized] public bool hasLooked = false;
    [NonSerialized] public bool hasMoved = false;

    private CharacterController controller;

    private void Awake()
    {
        startPosition = transform.position;
        currentHex = HexGridObject.WorldToHex(startPosition);
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

        // CALCULATE CLOSEST CELL

        Vector3 flatPosition = new Vector3(transform.position.x, 0, transform.position.z);
        if (Vector3.Distance(HexGridObject.HexToWorld(currentHex), flatPosition) > (changeCellDistance + 50f))
        {
            Vector2Int closestHex = currentHex;
            Vector3 closestWorldPos = HexGridObject.HexToWorld(currentHex);

            foreach (Vector2Int neighborCell in ThreeDSceneLogic.Instance.GetNeighbors(currentHex))
            {
                Vector3 cellWorldPos = HexGridObject.HexToWorld(neighborCell);
                if (Vector3.Distance(cellWorldPos, flatPosition) < Vector3.Distance(closestWorldPos, flatPosition))
                {
                    closestHex = neighborCell;
                    closestWorldPos = cellWorldPos;
                }
            }
            if (closestHex != currentHex)
            {
                currentHex = closestHex;
                ThreeDSceneLogic.Instance.HandlePlayerChangeCell(closestHex);
            }
        }

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


    public void ResetPosition(float verticalRotation = -90f)
    {
        controller.enabled = false;

        transform.position = startPosition;
        this.verticalRotation = verticalRotation;
        cam.transform.localEulerAngles = Vector3.zero;
        currentHex = HexGridObject.WorldToHex(startPosition);

        controller.enabled = true;
    }
}
