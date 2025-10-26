using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RealisticCarController : MonoBehaviour
{
    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftCollider;
    [SerializeField] private WheelCollider frontRightCollider;
    [SerializeField] private WheelCollider rearLeftCollider;
    [SerializeField] private WheelCollider rearRightCollider;

    [Header("Wheel Meshes (for visuals)")]
    [SerializeField] private Transform frontLeftMesh;
    [SerializeField] private Transform frontRightMesh;
    [SerializeField] private Transform rearLeftMesh;
    [SerializeField] private Transform rearRightMesh;

    [Header("Car Settings")]
    [SerializeField] private float maxMotorTorque = 1500f;   // Force applied to move the car
    [SerializeField] private float maxSteerAngle = 30f;      // Max turning angle in degrees
    [SerializeField] private float brakeForce = 3000f;       // Braking torque
    [SerializeField] private float maxSpeed = 120f;          // In km/h
    [SerializeField] private float directionChangeBrake = 5000f; // Stronger brake when reversing direction

    private Rigidbody rb;

    private float currentMotorTorque;
    private float currentSteerAngle;
    private float currentBrakeTorque;
    private float lastMoveInput = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // lower center of mass for stability
    }

    private void FixedUpdate()
    {
        HandleInput();
        ApplyMovement();
        UpdateWheelMeshes();
    }

    private void HandleInput()
    {
        // --- YOUR CUSTOM KEY MAPPING ---
        // D = Forward, A = Backward, W = Left, S = Right

        float moveInput = 0f;
        float steerInput = 0f;

        if (Input.GetKey(KeyCode.D))
            moveInput = 1f; // Forward
        else if (Input.GetKey(KeyCode.A))
            moveInput = -1f; // Backward
        else
            moveInput = 0f;

        if (Input.GetKey(KeyCode.W))
            steerInput = -1f; // Left
        else if (Input.GetKey(KeyCode.S))
            steerInput = 1f; // Right
        else
            steerInput = 0f;

        // --- Check for direction change ---
        bool isDirectionChanging = (moveInput != 0 && Mathf.Sign(moveInput) != Mathf.Sign(lastMoveInput) && rb.linearVelocity.magnitude > 1f);

        // Smooth acceleration and steering
        currentMotorTorque = moveInput * maxMotorTorque;
        currentSteerAngle = steerInput * maxSteerAngle;

        // Apply brake when:
        // 1. No move input, OR
        // 2. Direction is changing
        if (isDirectionChanging)
        {
            currentBrakeTorque = directionChangeBrake;
            currentMotorTorque = 0f; // stop motor while braking
        }
        else if (moveInput == 0)
        {
            currentBrakeTorque = brakeForce * 0.5f;
        }
        else
        {
            currentBrakeTorque = 0f;
        }

        // Limit top speed
        float speedKmh = rb.linearVelocity.magnitude * 3.6f;
        if (speedKmh > maxSpeed)
        {
            currentMotorTorque = 0f;
        }

        // Store for next frame
        lastMoveInput = moveInput;
    }

    private void ApplyMovement()
    {
        // Apply steering
        frontLeftCollider.steerAngle = currentSteerAngle;
        frontRightCollider.steerAngle = currentSteerAngle;

        // Apply motor torque to rear wheels
        rearLeftCollider.motorTorque = currentMotorTorque;
        rearRightCollider.motorTorque = currentMotorTorque;

        // Apply brakes
        ApplyBraking(currentBrakeTorque);
    }

    private void ApplyBraking(float brake)
    {
        frontLeftCollider.brakeTorque = brake;
        frontRightCollider.brakeTorque = brake;
        rearLeftCollider.brakeTorque = brake;
        rearRightCollider.brakeTorque = brake;
    }

    private void UpdateWheelMeshes()
    {
        UpdateWheelPose(frontLeftCollider, frontLeftMesh);
        UpdateWheelPose(frontRightCollider, frontRightMesh);
        UpdateWheelPose(rearLeftCollider, rearLeftMesh);
        UpdateWheelPose(rearRightCollider, rearRightMesh);
    }

    private void UpdateWheelPose(WheelCollider collider, Transform mesh)
    {
        if (collider == null || mesh == null) return;

        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
