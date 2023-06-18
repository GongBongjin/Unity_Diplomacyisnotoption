using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 100.0f;
    public float rotateSpeed = 100.0f;
    public float zoomSpeed = 1000.0f;

    [HideInInspector] private float rotateAngle = 180.0f;
    [HideInInspector] private Quaternion initialRotation = Quaternion.Euler(45.0f, 180.0f, 0f);
    [HideInInspector] private Vector3 initialPosition = new Vector3(0, 49, 57);

    [HideInInspector] private float zoomMin = 1.0f;
    [HideInInspector] private float zoomMax = 100.0f;

    private void Start()
    {
        transform.rotation = initialRotation;
        transform.position = initialPosition;
    }


    void Update()
    {
        CamerControl();
    }

    private void CamerControl()
    {
        //Rot
        if (Input.GetKey(KeyCode.Q))
        {
            rotateAngle += rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(45.0f, rotateAngle, 0.0f);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotateAngle -= rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(45.0f, rotateAngle, 0.0f);
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float z = Input.GetAxis("Mouse ScrollWheel");

        Vector3 moveDir = new Vector3(h, 0, v).normalized;

        // Current Forward Vector
        Vector3 forward = transform.rotation * Vector3.forward;
        forward.y = 0;

        Vector3 zoom = transform.rotation * Vector3.forward;

        // Rotate Axis Vector
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        // Move
        transform.position += forward * moveDir.z * moveSpeed * Time.deltaTime;
        transform.position += right * moveDir.x * moveSpeed * Time.deltaTime;

        // Zoom In/Out
        transform.position += zoom * z * zoomSpeed * Time.deltaTime;
        if (transform.position.y < zoomMin || transform.position.y > zoomMax)
            transform.position -= zoom * z * zoomSpeed * Time.deltaTime;
    }
}