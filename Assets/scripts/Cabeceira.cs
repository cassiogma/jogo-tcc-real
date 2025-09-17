// Cabeceira.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Cabeceira : MonoBehaviour
{
    [Header("Interação")]
    public KeyCode teclaInteracao = KeyCode.E;

    [Header("Mensagens")]
    public string mensagemPrompt = "Pressione E para pegar o diário";
    public string mensagemColeta = "Você pegou o diário! Pressione Q para abrir";
    public float duracaoMensagemColeta = 2.5f;

    [Header("Pós-coleta (opcionais)")]
    public bool desativarColisorAoPegar = true;
    public bool trocarSpriteAposPegar = false;
    public Sprite spriteAposPegar;
    public bool desativarObjetoAoPegar = false;

    [Header("Páginas iniciais (opcional)")]
    [Tooltip("Arraste TextAssets (um por página) para entregar junto com o diário.")]
    public TextAsset[] paginasIniciais;

    private bool playerPerto = false;
    private bool diarioPego = false;
    private Collider2D meuColisor;
    private SpriteRenderer sr;

    private void OnValidate()
    {
        var col = GetComponent<Collider2D>();
        if (col != null && !col.isTrigger) col.isTrigger = true; // garante trigger
    }

    private void Awake()
    {
        meuColisor = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Se já estava pego (ex: retornou à cena), reflete visual
        var dm = DiarioManager.GetOrCreate();
        if (dm.TemDiario)
        {
            diarioPego = true;
            if (desativarColisorAoPegar && meuColisor != null) meuColisor.enabled = false;
            if (trocarSpriteAposPegar && sr != null && spriteAposPegar != null) sr.sprite = spriteAposPegar;
            if (desativarObjetoAoPegar) gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerPerto && !diarioPego && Input.GetKeyDown(teclaInteracao))
        {
            ColetarDiario();
        }
    }

    private void ColetarDiario()
    {
        var dm = DiarioManager.GetOrCreate();
        dm.ColetarDiario();

        // Entrega páginas iniciais, se houver
        if (paginasIniciais != null && paginasIniciais.Length > 0)
        {
            foreach (var ta in paginasIniciais)
                if (ta != null) dm.AdicionarPagina(ta.text);
        }

        diarioPego = true;

        // Pós-ação visual
        if (trocarSpriteAposPegar && sr != null && spriteAposPegar != null)
            sr.sprite = spriteAposPegar;

        if (desativarColisorAoPegar && meuColisor != null)
            meuColisor.enabled = false;

        HUDMensagens.instance?.MostrarMensagemPor(mensagemColeta, duracaoMensagemColeta);

        if (desativarObjetoAoPegar)
            gameObject.SetActive(false);

        Debug.Log($"[Cabeceira:{name}] Diário coletado. Páginas atuais: {dm.ContarPaginas()}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = true;

        if (!diarioPego)
            HUDMensagens.instance?.MostrarMensagem(mensagemPrompt);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerPerto = false;

        // Só limpa se não pegou ainda (para não sumir confirmação)
        if (!diarioPego)
            HUDMensagens.instance?.LimparMensagem();
    }
}
