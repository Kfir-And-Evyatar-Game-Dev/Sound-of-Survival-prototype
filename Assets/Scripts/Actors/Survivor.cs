using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class Survivor : MonoBehaviour
{
    [SerializeField] float speed = 2.5f;
    Rigidbody2D body;
    EchoPulse echo;
    public event Action Interact;
    public event Action Caught;

    public void Initialize(EchoPulse pulse)
    {
        echo = pulse;
        body = gameObject.AddComponent<Rigidbody2D>();
        body.gravityScale = 0;
        body.freezeRotation = true;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        gameObject.AddComponent<CircleCollider2D>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.spaceKey.wasPressedThisFrame)
            echo.Emit(body.position, keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed);
        if (keyboard.eKey.wasPressedThisFrame) Interact?.Invoke();
    }

    void FixedUpdate()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        int x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0)
              - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
        int y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0)
              - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
        body.linearVelocity = new Vector2(x, y).normalized * speed;
    }

    void OnDisable()
    {
        if (body != null) body.linearVelocity = Vector2.zero;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contactCount > 0 && collision.collider.TryGetComponent<EchoHunter>(out _))
            Caught?.Invoke();
    }
}
