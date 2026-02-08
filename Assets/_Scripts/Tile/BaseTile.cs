using UnityEngine;
using UnityEngine.InputSystem;  // Required for new Input System

public class BaseTile : MonoBehaviour
{
    protected float speed;
    protected float despawnY = -6f; // off-screen Y

    public void Initialize(float fallSpeed)
    {
        speed = fallSpeed;
    }

    void Update()
    {
        // Move tile down
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Despawn if offscreen
        if (transform.position.y < despawnY)
        {
            DespawnTile();
        }

        // Check input using new Input System
        DetectInput();
    }

    void DetectInput()
    {
        // ----- MOUSE CLICK -----
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 mouse2D = new Vector2(mousePos.x, mousePos.y);

            Collider2D hit = Physics2D.OverlapPoint(mouse2D);
            if (hit != null && hit.transform == this.transform)
            {
                DespawnTile();
            }
        }

        // ----- TOUCH INPUT -----
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.isPressed && touch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                    Vector2 touch2D = new Vector2(touchPos.x, touchPos.y);

                    Collider2D hit = Physics2D.OverlapPoint(touch2D);
                    if (hit != null && hit.transform == this.transform)
                    {
                        DespawnTile();
                    }
                }
            }
        }
    }

    public void DespawnTile()
    {
        ObjectPooler.Instance.Despawn(transform);
    }
}
