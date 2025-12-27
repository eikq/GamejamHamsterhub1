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
    public float bobFrequency = 5.0f; // ความถี่ในการโยก
    public float bobAmplitude = 0.05f; // ความแรงในการโยก (แนวตั้ง)
    public float bobSwayAngle = 0.5f;  // การเอียงหัว (แนวหมุน)

    [Header("Camera Tilt (Lean)")]
    public bool enableCameraTilt = true;
    public float tiltAmount = 3.0f; // องศาที่เอียง
    public float tiltSpeed = 4.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float defaultPosY = 0;
    float timer = 0;
    float currentTilt = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if(playerCamera == null) playerCamera = GetComponentInChildren<Camera>();
        defaultPosY = playerCamera.transform.localPosition.y;
    }

    void Update()
    {
        // 1. Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical");
        float curSpeedY = (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal");
        
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = -0.5f; // Stick to ground
        }
        
        characterController.Move(moveDirection * Time.deltaTime);

        // 2. Camera Rotation (Mouse Look)
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        
        // หมุนตัว (Y-Axis)
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        // 3. Head Bobbing Logic
        if (enableHeadBob)
        {
            if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
            {
                // Player is moving
                timer += Time.deltaTime * (isRunning ? bobFrequency * 1.5f : bobFrequency);
                float newY = defaultPosY + Mathf.Sin(timer) * bobAmplitude;
                
                // Sway (เอียงหัวนิดๆ เวลาเดิน)
                float sway = Mathf.Cos(timer) * bobSwayAngle;
                
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, newY, playerCamera.transform.localPosition.z);
                // Note: Sway rotation will be combined with Tilt below
            }
            else
            {
                // Idle
                timer = 0;
                playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, Mathf.Lerp(playerCamera.transform.localPosition.y, defaultPosY, Time.deltaTime * 5f), playerCamera.transform.localPosition.z);
            }
        }

        // 4. Camera Tilt (Strafe) Logic
        if (enableCameraTilt)
        {
            float targetTilt = -Input.GetAxis("Horizontal") * tiltAmount;
            currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);
            
            // รวม Rotation: ก้มเงย (X) + เอียงเดิน (Z)
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, currentTilt);
        }
        else
        {
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
}