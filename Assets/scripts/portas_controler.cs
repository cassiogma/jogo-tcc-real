using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PortaTeleporte : MonoBehaviour
{
    [Header("Teleporte")]
    public Transform destino; // Porta destino
    public bool usarFade = true;
    public float fadeOutDur = 0.35f;
    public float fadeHold = 0.05f;
    public float fadeInDur = 0.35f;
    public float cooldown = 1f; // Tempo para permitir novo teleporte
    public Vector2 deslocamento = new Vector2(0.5f, 0); // Para evitar ficar dentro do trigger

    [Header("Configuração da Porta")]
    public string chaveNecessaria = ""; // Se vazio, não precisa de chave

    [Header("Som")]
    public AudioClip somAbrir;
    public AudioClip somTrancada;

    private AudioSource audioSource;
    private GameObject player;
    private bool playerPerto = false;
    private bool emUso = false;
    private Animator anim;

    private void OnValidate()
    {
        var col = GetComponent<Collider2D>();
        if (col && !col.isTrigger) col.isTrigger = true;
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        if (player == null)
            Debug.LogError($"[{name}] Player com Tag 'Player' não encontrado.");
    }

    void Update()
    {
        if (!playerPerto || emUso) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool temChave = string.IsNullOrWhiteSpace(chaveNecessaria) ||
                            (InventarioPlayer.instance != null && InventarioPlayer.instance.TemChave(chaveNecessaria));

            if (!temChave)
            {
                HUDMensagens.instance?.MostrarMensagemPor($"A porta está trancada — precisa da {chaveNecessaria}.", 2f);
                if (somTrancada != null && audioSource != null)
                    audioSource.PlayOneShot(somTrancada);
                return;
            }

            emUso = true;
            anim?.SetTrigger("Abrir");

            if (somAbrir != null && audioSource != null)
                audioSource.PlayOneShot(somAbrir);

            HUDMensagens.instance?.LimparMensagem();

            if (usarFade)
            {
                var fader = ScreenFader.instance ?? new GameObject("ScreenFader").AddComponent<ScreenFader>();
                StartCoroutine(AbrirComFade(fader));
            }
            else
            {
                StartCoroutine(TeleportarComCooldown());
            }
        }
    }

    private IEnumerator AbrirComFade(ScreenFader fader)
    {
        yield return fader.FadeOutIn(fadeOutDur, fadeHold, fadeInDur, () =>
        {
            if (player != null && destino != null)
                player.transform.position = (Vector2)destino.position + deslocamento;
        });

        yield return new WaitForSeconds(cooldown);
        emUso = false;
    }

    private IEnumerator TeleportarComCooldown()
    {
        if (player != null && destino != null)
            player.transform.position = (Vector2)destino.position + deslocamento;

        yield return new WaitForSeconds(cooldown);
        emUso = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerPerto = true;
        HUDMensagens.instance?.MostrarMensagem("Pressione E para abrir");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerPerto = false;
        HUDMensagens.instance?.LimparMensagem();
    }
}
