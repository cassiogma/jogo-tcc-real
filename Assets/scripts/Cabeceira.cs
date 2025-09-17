// Cabeceira.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Cabeceira : MonoBehaviour
{
    [Header("Intera��o")]
    public KeyCode teclaInteracao = KeyCode.E;

    [Header("Mensagens")]
    public string mensagemPrompt = "Pressione E para pegar o di�rio";
    public string mensagemColeta = "Voc� pegou o di�rio! Pressione Q para abrir";
    public float duracaoMensagemColeta = 2.5f;

    [Header("P�s-coleta (opcionais)")]
    public bool desativarColisorAoPegar = true;
    public bool trocarSpriteAposPegar = false;
    public Sprite spriteAposPegar;
    public bool desativarObjetoAoPegar = false;

    [Header("P�ginas iniciais (opcional)")]
    [Tooltip("Arraste TextAssets (um por p�gina) para entregar junto com o di�rio.")]
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
        // Se j� estava pego (ex: retornou � cena), reflete visual
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

        // Entrega p�ginas iniciais, se houver
        if (paginasIniciais != null && paginasIniciais.Length > 0)
        {
            foreach (var ta in paginasIniciais)
                if (ta != null) dm.AdicionarPagina(ta.text);
        }

        diarioPego = true;

        // P�s-a��o visual
        if (trocarSpriteAposPegar && sr != null && spriteAposPegar != null)
            sr.sprite = spriteAposPegar;

        if (desativarColisorAoPegar && meuColisor != null)
            meuColisor.enabled = false;

        HUDMensagens.instance?.MostrarMensagemPor(mensagemColeta, duracaoMensagemColeta);

        if (desativarObjetoAoPegar)
            gameObject.SetActive(false);

        Debug.Log($"[Cabeceira:{name}] Di�rio coletado. P�ginas atuais: {dm.ContarPaginas()}");
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

        // S� limpa se n�o pegou ainda (para n�o sumir confirma��o)
        if (!diarioPego)
            HUDMensagens.instance?.LimparMensagem();
    }
}
