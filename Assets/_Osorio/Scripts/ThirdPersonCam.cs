using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerObject;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float rotationSpeed;
    public InputController InputActions;

    private void Awake()
    {
        InputActions = new InputController();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;
        
        Vector2 movementInput = InputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 inputDir = orientation.forward * movementInput.y + orientation.right * movementInput.x;

        if (inputDir != Vector3.zero)
        {
            playerObject.forward = Vector3.Slerp
                (playerObject.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
        
    }

    private void OnEnable()
    {
        InputActions.Player.Enable();
    }
}
