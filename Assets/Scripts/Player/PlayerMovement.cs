using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float friction;

    Vector2 input_move;
    Vector2 move;

    public void ProcessMove(CallbackContext input) { input_move = input.ReadValue<Vector2>(); }

    private void Update()
    {
        if (input_move != Vector2.zero)
        {
            move = Vector2.ClampMagnitude(move + input_move * movementSpeed * Time.deltaTime, maxSpeed);
        }
        else
        {
            move = Vector2.Lerp(move, Vector2.zero, friction * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(move);
    }
}
