using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteracaoPaginaComCamera : MonoBehaviour
{
    [Header("Interação")]
    public KeyCode teclaInteracao = KeyCode.E;
    public string mensagemPrompt = "Pressione E para examinar";
    public string mensagemEntrada = "Você está examinando uma pista...";
    public string mensagemColeta = "Você encontrou uma nova página!";
    public float duracaoMensagem = 2f;

    [Header("Página")]
    [Tooltip("Arraste um TextAsset com o conteúdo da página.")]
    public TextAsset paginaTexto;

    [Header("Câmeras")]
    public Camera cameraPrincipal;
    public Camera cameraPuzzle;

    [Header("Jogador")]
    public GameObject jogador;

    private bool playerPerto = false;
    private bool paginaColetada = false;
    private bool estaExaminando = false;

    private void Start()
    {
        if (cameraPuzzle != null)
            cameraPuzzle.enabled = false;
    }

    private void Update()
    {
        if (playerPerto && !estaExaminando && Input.GetKeyDown(teclaInteracao))
        {
            EntrarNaVisao();
        }

        if (estaExaminando && Input.GetKeyDown(KeyCode.Escape))
        {
            SairDaVisao();
        }
    }

    private void EntrarNaVisao()
    {
        estaExaminando = true;

        if (cameraPrincipal != null) cameraPrincipal.enabled = false;
        if (cameraPuzzle != null) cameraPuzzle.enabled = true;
        if (jogador != null) jogador.SetActive(false);

        HUDMensagens.instance?.MostrarMensagemPor(mensagemEntrada, duracaoMensagem);

        if (!paginaColetada && paginaTexto != null)
        {
            var dm = DiarioManager.GetOrCreate();
            dm.AdicionarPagina(paginaTexto.text);
            HUDMensagens.instance?.MostrarMensagemPor(mensagemColeta, duracaoMensagem);
            Debug.Log("[InteracaoPaginaComCamera] Página adicionada ao diário.");
            paginaColetada = true;
        }
    }

    private void SairDaVisao()
    {
        estaExaminando = false;

        if (cameraPrincipal != null) cameraPrincipal.enabled = true;
        if (cameraPuzzle != null) cameraPuzzle.enabled = false;
        if (jogador != null) jogador.SetActive(true);

        Debug.Log("[InteracaoPaginaComCamera] Saiu da visão da pista.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = true;
        if (!estaExaminando)
            HUDMensagens.instance?.MostrarMensagem(mensagemPrompt);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = false;
        if (!estaExaminando)
            HUDMensagens.instance?.LimparMensagem();
    }
}

