using UnityEngine;
using TMPro;

public class DiarioUI : MonoBehaviour
{
    [Header("Referências")]
    public GameObject painel;                 // Panel (Canvas) que será aberto/fechado
    public TextMeshProUGUI textoPagina;       // TMP do conteúdo
    public TextMeshProUGUI textoIndice;       // TMP opcional "Página X/Y"

    [Header("Comportamento")]
    public bool pausarJogoAoAbrir = true;
    public KeyCode teclaAbrir = KeyCode.Q;
    public KeyCode teclaFechar = KeyCode.Q;
    public KeyCode teclaAnterior = KeyCode.A;
    public KeyCode teclaProxima = KeyCode.D;
    public KeyCode teclaAnteriorAlt = KeyCode.LeftArrow;
    public KeyCode teclaProximaAlt = KeyCode.RightArrow;

    private int paginaAtual = 0;
    private bool aberto = false;
    private float timeScaleAntes = 1f;

    private void Awake()
    {
        DiarioManager.GetOrCreate();

        if (painel == null)
        {
            Debug.LogError("[DiarioUI] 'painel' não atribuído. Arraste o Panel do Canvas.");
        }
        else if (painel == gameObject)
        {
            Debug.LogWarning("[DiarioUI] 'painel' é o MESMO GO do DiarioUI. Evite. O DiarioUI deve ficar em um GO sempre ativo (ex.: 'diario').");
        }
        else
        {
            painel.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (DiarioManager.instance != null)
            DiarioManager.instance.OnPagesChanged += HandlePagesChanged;
    }

    private void OnDisable()
    {
        if (DiarioManager.instance != null)
            DiarioManager.instance.OnPagesChanged -= HandlePagesChanged;
    }

    private void HandlePagesChanged()
    {
        if (aberto) AtualizarPagina();
    }

    private void Update()
    {

        if (Input.GetKeyDown(teclaAbrir))
        {
            var dm = DiarioManager.GetOrCreate();
            Debug.Log($"[DiarioUI] Q pressionado. TemDiario={dm.TemDiario}, painel={(painel ? painel.name : "null")}, painelAtivo={(painel ? painel.activeSelf : false)}");

            if (dm.TemDiario) Toggle();
            else HUDMensagens.instance?.MostrarMensagemPor("Você ainda não tem o diário.", 2f);
        }

        if (!aberto) return;

        if (Input.GetKeyDown(teclaFechar) || Input.GetKeyDown(KeyCode.Escape))
        {
            Fechar();
            return;
        }

        if (Input.GetKeyDown(teclaAnterior) || Input.GetKeyDown(teclaAnteriorAlt)) PaginaAnterior();
        else if (Input.GetKeyDown(teclaProxima) || Input.GetKeyDown(teclaProximaAlt)) ProximaPagina();
    }

    private void Toggle() { if (aberto) Fechar(); else Abrir(); }

    public void Abrir()
    {
        if (painel == null) { Debug.LogError("[DiarioUI] Abrir(): 'painel' nulo."); return; }
        aberto = true;

        if (pausarJogoAoAbrir) { timeScaleAntes = Time.timeScale; Time.timeScale = 0f; }
        if (!painel.activeSelf) painel.SetActive(true);

        AtualizarPagina();

        var dm = DiarioManager.instance;
        int total = dm != null ? dm.ContarPaginas() : -1;
        Debug.Log($"[DiarioUI] Abrir() → painelAtivo={painel.activeSelf}, paginas={total}, paginaAtual={paginaAtual}");
    }

    public void Fechar()
    {
        if (painel == null) return;
        aberto = false;

        if (pausarJogoAoAbrir) Time.timeScale = timeScaleAntes;
        if (painel.activeSelf) painel.SetActive(false);

        Debug.Log("[DiarioUI] Fechar() → painel desativado.");
    }

    private void AtualizarPagina()
    {
        var dm = DiarioManager.instance;
        if (dm == null)
        {
            Debug.LogError("[DiarioUI] DiarioManager.instance nulo.");
            return;
        }

        int total = dm.ContarPaginas();
        if (total == 0)
        {
            if (textoPagina != null) textoPagina.text = "(O diário está vazio)";
            if (textoIndice != null) textoIndice.text = "";
            Debug.Log("[DiarioUI] AtualizarPagina() → sem páginas.");
            return;
        }

        paginaAtual = Mathf.Clamp(paginaAtual, 0, total - 1);
        if (textoPagina != null) textoPagina.text = dm.ObterPagina(paginaAtual);
        if (textoIndice != null) textoIndice.text = $"Página {paginaAtual + 1}/{total}";
        Debug.Log($"[DiarioUI] AtualizarPagina() → paginaAtual={paginaAtual + 1}/{total}");
    }

    private void PaginaAnterior()
    {
        var dm = DiarioManager.instance;
        if (dm == null || dm.ContarPaginas() == 0) return;
        paginaAtual = (paginaAtual - 1 + dm.ContarPaginas()) % dm.ContarPaginas();
        AtualizarPagina();
        Debug.Log($"[DiarioUI] PaginaAnterior() → paginaAtual={paginaAtual + 1}/{dm.ContarPaginas()}");
    }

    private void ProximaPagina()
    {
        var dm = DiarioManager.instance;
        if (dm == null || dm.ContarPaginas() == 0) return;
        paginaAtual = (paginaAtual + 1) % dm.ContarPaginas();
        AtualizarPagina();
        Debug.Log($"[DiarioUI] ProximaPagina() → paginaAtual={paginaAtual + 1}/{dm.ContarPaginas()}");
    }

    [ContextMenu("DEBUG: Status Agora")]
    public void DebugStatus()
    {
        var dm = DiarioManager.instance;
        int total = dm ? dm.ContarPaginas() : -1;
        Debug.Log($"[DEBUG] aberto={aberto}, painel={(painel ? painel.name : "null")}, painelAtivo={(painel ? painel.activeSelf : false)}, textoPagina={(textoPagina ? textoPagina.name : "null")}, totalPaginas={total}, paginaAtual={paginaAtual}");
    }
}
