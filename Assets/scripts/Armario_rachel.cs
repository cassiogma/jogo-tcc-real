using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Armario : MonoBehaviour
{
    [Header("Chave")]
    [Tooltip("Identificador da chave que será adicionada ao inventário.")]
    public string chaveDentro = "ChavePorta1";

    [Header("Interação")]
    public KeyCode teclaInteracao = KeyCode.E;

    [Header("Feedback de Mensagem")]
    [Tooltip("Texto exibido quando a chave é pega.")]
    public string mensagemPegouChave = "Você pegou a chave!";
    [Tooltip("Tempo em segundos que a mensagem ficará visível.")]
    public float duracaoMensagemPegou = 2f;

    [Header("Comportamento após pegar")]
    [Tooltip("Desativa o Collider2D para impedir interações futuras.")]
    public bool desativarColisorAoPegar = true;
    [Tooltip("Troca o sprite do próprio GameObject (exige SpriteRenderer).")]
    public bool trocarSpriteAposPegar = false;
    public Sprite spriteAposPegar;
    [Tooltip("Desativa todo o GameObject após pegar (depois de mostrar a mensagem).")]
    public bool desativarObjetoAoPegar = false;

    private bool playerPerto = false;
    private bool chavePega = false;
    private Collider2D meuColisor;
    private SpriteRenderer sr;

    private void OnValidate()
    {
        // Garante que o Collider2D seja trigger
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger) col.isTrigger = true;
    }

    private void Awake()
    {
        meuColisor = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (meuColisor == null)
            Debug.LogError($"[Armario:{name}] Não há Collider2D no mesmo GameObject.");
    }

    private void Update()
    {
        if (playerPerto && !chavePega && Input.GetKeyDown(teclaInteracao))
        {
            PegarChave();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerPerto = true;

        if (!chavePega)
        {
            HUDMensagens.instance?.MostrarMensagem("Pressione E para pegar a chave");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerPerto = false;

        // Não limpa a mensagem se a chave já foi pega (para não sumir com o feedback)
        if (!chavePega)
        {
            HUDMensagens.instance?.LimparMensagem();
        }
    }

    private void PegarChave()
    {
        if (InventarioPlayer.instance == null)
        {
            Debug.LogError($"[Armario:{name}] InventarioPlayer.instance é nulo. Coloque o InventarioPlayer na cena.");
            return;
        }

        // Adiciona a chave (sem consumir no uso da porta)
        InventarioPlayer.instance.AdicionarChave(chaveDentro);
        chavePega = true;

        // Feedback visual opcional
        if (trocarSpriteAposPegar && sr != null && spriteAposPegar != null)
            sr.sprite = spriteAposPegar;

        // Impede novas interações
        if (desativarColisorAoPegar && meuColisor != null)
            meuColisor.enabled = false;

        // Mensagem clara de confirmação
        HUDMensagens.instance?.MostrarMensagemPor(mensagemPegouChave, duracaoMensagemPegou);

        // Se desejar, desative o objeto depois (a mensagem já foi enviada ao HUD)
        if (desativarObjetoAoPegar)
            gameObject.SetActive(false);

        Debug.Log($"[Armario:{name}] Chave '{chaveDentro}' adicionada ao inventário.");
    }
}
