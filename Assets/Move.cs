using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] private float speed = 7f;

    private float horizontalInput;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // Hash da animação (mais rápido que usar string)
    private static readonly int RunHash = Animator.StringToHash("run");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Captura o input de movimento (A/D ou setas)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Atualiza animação de corrida
        animator.SetBool(RunHash, horizontalInput != 0);

        // Inverte o sprite conforme direção
        if (horizontalInput > 0)
            spriteRenderer.flipX = false;
        else if (horizontalInput < 0)
            spriteRenderer.flipX = true;
    }

    private void FixedUpdate()
    {
        // Unity 6+: usa linearVelocity
        rb.linearVelocity = new Vector2(horizontalInput * speed, rb.linearVelocity.y);
    }
}
