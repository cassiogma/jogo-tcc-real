using UnityEngine;

public class UnstuckController : MonoBehaviour
{
    public Rigidbody2D rb;
    public float checkInterval = 0.3f;
    public float minSpeed = 0.05f;
    public float unstuckForce = 3f;
    private float timer;
    private Vector2 lastInput;

    void Update()
    {
        // Captura input do jogador
        lastInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            timer = 0f;

            if (rb.linearVelocity.magnitude < minSpeed && lastInput != Vector2.zero)
            {
                rb.AddForce(lastInput * unstuckForce, ForceMode2D.Impulse);
            }
        }
    }
}