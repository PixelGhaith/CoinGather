using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

[RequireComponent(typeof(PlayerInput), typeof(Rigidbody2D))]
public class TopDown2DMoveNet : NetworkBehaviour
{
    public float speed = 6f;

    Rigidbody2D rb;
    Vector2 move;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (!IsOwner) return;                 
        move = ctx.ReadValue<Vector2>();      
    }

    void FixedUpdate()
    {
        if (!IsOwner) return;                 
        rb.linearVelocity = move * speed;          
    }

    void OnDisable()
    {
        if (IsOwner && rb) rb.linearVelocity = Vector2.zero;
        move = Vector2.zero;
    }
}
