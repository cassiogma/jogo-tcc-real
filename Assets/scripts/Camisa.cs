using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Camisa : MonoBehaviour
{
    [Header("Interação")]
    public KeyCode teclaInteracao = KeyCode.E;

    [Header("Mensagem")]
    public string mensagemPrompt = "Pressione E para examinar a camisa";
    public string mensagemColeta = "Você encontrou uma pista na camisa!";
    public float duracaoMensagem = 2f;

    [Header("Página da camisa")]
    [Tooltip("Arraste um TextAsset com o conteúdo da página.")]
    public TextAsset paginaTexto;

    private bool playerPerto = false;
    private bool jaInteragiu = false;
    private Collider2D meuColisor;

    private void Awake()
    {
        meuColisor = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = true;
        if (!jaInteragiu)
            HUDMensagens.instance?.MostrarMensagem(mensagemPrompt);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = false;
        if (!jaInteragiu)
            HUDMensagens.instance?.LimparMensagem();
    }

    private void Update()
    {
        if (playerPerto && !jaInteragiu && Input.GetKeyDown(teclaInteracao))
        {
            Interagir();
        }
    }

    private void Interagir()
    {
        var dm = DiarioManager.GetOrCreate();
        if (paginaTexto != null)
        {
            dm.AdicionarPagina(paginaTexto.text);
        }

        jaInteragiu = true;
        HUDMensagens.instance?.MostrarMensagemPor(mensagemColeta, duracaoMensagem);
        if (meuColisor != null) meuColisor.enabled = false;
        Debug.Log($"[Camisa] Página adicionada ao diário.");
    }
}