using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HorrorFPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;
    public float runSpeed = 6.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 80.0f;

    [Header("Head Bobbing")]
    public bool enableHeadBob = true;
    public float bobFrequency = 5.0f;
    public float bobAmplitude = 0.05f;
    public float bobSwayAngle = 0.5f;

    [Header("Camera Tilt")]
    public bool enableCameraTilt = true;
    public float tiltAmount = 3.0f;
    public float tiltSpeed = 4.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float defaultPosY = 0;
    float timer = 0;
    float currentTilt = 0;
    
    // ตัวแปรเช็คว่าขยับได้ไหม (ไว้สั่งปิดตอนคุย)
    public bool canMove = true; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if(playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        defaultPosY = playerCamera.transform.localPosition.y;
        
        // ล็อคเมาส์ตอนเริ่ม
        LockCursor(true);
    }

    void Update()
    {
        if (!canMove) return;

        // --- Movement ---
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");
        
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded) moveDirection.y -= gravity * Time.deltaTime;
        else moveDirection.y = -0.5f;
        
        characterController.Move(moveDirection * Time.deltaTime);

        // --- Camera Rotation ---
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // --- Head Bobbing ---
        if (enableHeadBob)
        {
            if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
            {
                timer += Time.deltaTime * (isRunning ? bobFrequency * 1.5f : bobFrequency);
                float newY = defaultPosY + Mathf.Sin(timer) * bobAmplitude;
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, newY, playerCamera.transform.localPosition.z);
            }
            else
            {
                timer = 0;
                playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, new Vector3(playerCamera.transform.localPosition.x, defaultPosY, playerCamera.transform.localPosition.z), Time.deltaTime * 5f);
            }
        }

        // --- Camera Tilt ---
        if (enableCameraTilt)
        {
            float targetTilt = -Input.GetAxis("Horizontal") * tiltAmount;
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, currentTilt);
        }
        else
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }

    // ฟังก์ชันสำหรับให้ Fungus เรียกใช้
    public void SetControl(bool state)
    {
        canMove = state;
        LockCursor(state);
    }

    void LockCursor(bool state)
    {
        Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !state;
    }
}