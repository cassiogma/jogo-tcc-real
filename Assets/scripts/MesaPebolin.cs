using UnityEngine;

public class MesaPebolim : MonoBehaviour
{
    [Header("Interação")]
    public KeyCode teclaInteracao = KeyCode.E;
    public string mensagemPrompt = "Pressione E para examinar a mesa";
    public string mensagemEntradaPuzzle = "Você está agora focado na mesa de pebolim...";
    public float duracaoMensagem = 2f;

    [Header("Câmeras")]
    public Camera cameraPrincipal;
    public Camera cameraPuzzle;

    [Header("Jogador")]
    public GameObject jogador; // opcional: para desativar movimento

    private bool playerPerto = false;
    private bool resolvendoPuzzle = false;

    private void Start()
    {
        if (cameraPuzzle != null)
            cameraPuzzle.enabled = false;
    }

    private void Update()
    {
        if (playerPerto && !resolvendoPuzzle && Input.GetKeyDown(teclaInteracao))
        {
            EntrarNoPuzzle();
        }

        // Exemplo: tecla para sair do puzzle
        if (resolvendoPuzzle && Input.GetKeyDown(KeyCode.Escape))
        {
            SairDoPuzzle();
        }
    }

    private void EntrarNoPuzzle()
    {
        resolvendoPuzzle = true;

        if (cameraPrincipal != null) cameraPrincipal.enabled = false;
        if (cameraPuzzle != null) cameraPuzzle.enabled = true;

        if (jogador != null) jogador.SetActive(false); // opcional

        HUDMensagens.instance?.MostrarMensagemPor(mensagemEntradaPuzzle, duracaoMensagem);
        Debug.Log("[MesaPebolim] Entrou no modo puzzle.");
    }

    private void SairDoPuzzle()
    {
        resolvendoPuzzle = false;

        if (cameraPrincipal != null) cameraPrincipal.enabled = true;
        if (cameraPuzzle != null) cameraPuzzle.enabled = false;

        if (jogador != null) jogador.SetActive(true); // opcional

        Debug.Log("[MesaPebolim] Saiu do modo puzzle.");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = true;
        HUDMensagens.instance?.MostrarMensagem(mensagemPrompt);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = false;
        HUDMensagens.instance?.LimparMensagem();
    }
}
