using UnityEngine;
using System.Collections;

public class portas_controler : MonoBehaviour
{
    public Transform destination;     // destino do teleporte
    private GameObject player;
    private bool playerPerto = false;
    private Animator anim;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Jogador não encontrado! Verifique a tag 'Player'.");
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("Animator não encontrado na porta!");
        }
    }

    void Update()
    {
        if (playerPerto && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Tecla E pressionada! Abrindo porta...");

            if (anim != null)
            {
                anim.SetTrigger("Abrir"); // dispara a animação usando Trigger
            }

            // inicia coroutine para teleporte com delay de 2 segundos
            StartCoroutine(TeleportarComDelay(2f));
        }
    }

    private IEnumerator TeleportarComDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // espera 2 segundos
        player.transform.position = destination.position;
        Debug.Log("Player teleportado!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jogador perto da porta.");
            playerPerto = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Jogador saiu da porta.");
            playerPerto = false;
        }
    }
}
