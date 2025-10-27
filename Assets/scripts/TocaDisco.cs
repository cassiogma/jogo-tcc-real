using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TocaDisco : MonoBehaviour
{
    [Header("Interação")]
    public KeyCode teclaInteracao = KeyCode.E;
    public string mensagemPrompt = "Pressione E para ligar o toca-discos";
    public string mensagemTocando = "A música começou a tocar...";
    public string mensagemPagina = "Você encontrou uma anotação junto ao toca-discos!";
    public float duracaoMensagem = 2f;
    public float cooldown = 15f;

    [Header("Áudio")]
    public AudioClip musica;
    private AudioSource audioSource;

    [Header("Página do Diário")]
    public TextAsset paginaTexto;

    private bool playerPerto = false;
    private bool emCooldown = false;
    private bool paginaColetada = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = true;
        if (!emCooldown)
            HUDMensagens.instance?.MostrarMensagem(mensagemPrompt);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = false;
        HUDMensagens.instance?.LimparMensagem();
    }

    private void Update()
    {
        if (playerPerto && !emCooldown && Input.GetKeyDown(teclaInteracao))
        {
            Interagir();
        }
    }

    private void Interagir()
    {
        emCooldown = true;

        // Tocar música
        if (musica != null && audioSource != null)
        {
            audioSource.clip = musica;
            audioSource.loop = false;
            audioSource.Play();
        }

        HUDMensagens.instance?.MostrarMensagemPor(mensagemTocando, duracaoMensagem);
        Debug.Log("[TocaDisco] Música iniciada.");

        // Coletar página (apenas uma vez)
        if (!paginaColetada && paginaTexto != null)
        {
            var dm = DiarioManager.GetOrCreate();
            dm.AdicionarPagina(paginaTexto.text);
            HUDMensagens.instance?.MostrarMensagemPor(mensagemPagina, duracaoMensagem);
            Debug.Log("[TocaDisco] Página adicionada ao diário.");
            paginaColetada = true;
        }

        // Inicia cooldown
        Invoke(nameof(ResetCooldown), cooldown);
    }

    private void ResetCooldown()
    {
        emCooldown = false;
        if (playerPerto)
            HUDMensagens.instance?.MostrarMensagem(mensagemPrompt);
    }
}