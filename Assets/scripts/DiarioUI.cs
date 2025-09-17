using UnityEngine;
using TMPro;

public class DiarioUI : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Painel (GameObject) que será ativado/desativado ao abrir/fechar o diário.")]
    public GameObject painel;
    [Tooltip("TextMeshProUGUI que exibe o conteúdo da página.")]
    public TextMeshProUGUI textoPagina;
    [Tooltip("TextMeshProUGUI opcional para 'Página X/Y'.")]
    public TextMeshProUGUI textoIndice;

    [Header("Comportamento")]
    [Tooltip("Ao abrir o diário, pausa o jogo (Time.timeScale = 0).")]
    public bool pausarJogoAoAbrir = true;
    public KeyCode teclaAbrir = KeyCode.Q;
    public KeyCode teclaFechar = KeyCode.Q;
    public KeyCode teclaAnterior = KeyCode.A;
    public KeyCode teclaProxima = KeyCode.D;
    public KeyCode teclaAnteriorAlt = KeyCode.LeftArrow;
    public KeyCode teclaProximaAlt = KeyCode.RightArrow;

    // Estado interno
    private int paginaAtual = 0;
    private bool aberto = false;
    private float timeScaleAntes = 1f;

    private void Awake()
    {
        // Garante que exista DiarioManager (não quebra se você esquecer de colocar na cena)
        DiarioManager.GetOrCreate();

        // Proteção: não desativar o mesmo GO do script
        if (painel == null)
        {
            Debug.LogError("[DiarioUI] Campo 'painel' não atribuído no Inspector. Arraste o Panel do Canvas para cá.");
        }
        else if (painel == gameObject)
        {
            Debug.LogWarning("[DiarioUI] 'painel' é o MESMO GameObject do DiarioUI. Evite isso; o DiarioUI deve ficar em um GO sempre ativo (ex.: 'diario'). Não será desativado no Awake para não matar o Update().");
        }
        else
        {
            painel.SetActive(false); // só o painel começa desligado
        }
    }

    private void Update()
    {
        // Detecta Q para abrir/fechar
        if (Input.GetKeyDown(teclaAbrir))
        {
            var dm = DiarioManager.GetOrCreate();
            Debug.Log($"[DiarioUI] Q pressionado. TemDiario={dm.TemDiario}, painel={(painel ? painel.name : "null")}, painelAtivo={(painel ? painel.activeSelf : false)}");

            if (dm.TemDiario)
            {
                Toggle();
            }
            else
            {
                HUDMensagens.instance?.MostrarMensagemPor("Você ainda não tem o diário.", 2f);
            }
        }

        if (!aberto) return;

        // Fechar
        if (Input.GetKeyDown(teclaFechar) || Input.GetKeyDown(KeyCode.Escape))
        {
            Fechar();
            return;
        }

        // Navegação
        if (Input.GetKeyDown(teclaAnterior) || Input.GetKeyDown(teclaAnteriorAlt))
        {
            PaginaAnterior();
        }
        else if (Input.GetKeyDown(teclaProxima) || Input.GetKeyDown(teclaProximaAlt))
        {
            ProximaPagina();
        }
    }

    private void Toggle()
    {
        if (aberto) Fechar();
        else Abrir();
    }

    public void Abrir()
    {
        if (painel == null)
        {
            Debug.LogError("[DiarioUI] Não é possível abrir: 'painel' está nulo.");
            return;
        }

        aberto = true;

        if (pausarJogoAoAbrir)
        {
            timeScaleAntes = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (!painel.activeSelf) painel.SetActive(true);

        AtualizarPagina();

        // Logs
        var dm = DiarioManager.instance;
        int total = dm != null ? dm.ContarPaginas() : -1;
        Debug.Log($"[DiarioUI] Abrir() → painelAtivo={painel.activeSelf}, paginas={total}, paginaAtual={paginaAtual}");
    }

    public void Fechar()
    {
        if (painel == null) return;

        aberto = false;

        if (pausarJogoAoAbrir)
        {
            Time.timeScale = timeScaleAntes;
        }

        if (painel.activeSelf) painel.SetActive(false);

        Debug.Log("[DiarioUI] Fechar() → painel desativado.");
    }

    private void AtualizarPagina()
    {
        var dm = DiarioManager.instance;
        if (dm == null)
        {
            Debug.LogError("[DiarioUI] DiarioManager.instance está nulo durante AtualizarPagina().");
            return;
        }

        int total = dm.ContarPaginas();
        if (total == 0)
        {
            if (textoPagina != null) textoPagina.text = "(O diário está vazio)";
            if (textoIndice != null) textoIndice.text = "";
            Debug.Log("[DiarioUI] AtualizarPagina() → diário sem páginas.");
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
        Debug.Log($"[DiarioUI] PaginaAnterior() → nova paginaAtual={paginaAtual + 1}/{dm.ContarPaginas()}");
    }

    private void ProximaPagina()
    {
        var dm = DiarioManager.instance;
        if (dm == null || dm.ContarPaginas() == 0) return;

        paginaAtual = (paginaAtual + 1) % dm.ContarPaginas();
        AtualizarPagina();
        Debug.Log($"[DiarioUI] ProximaPagina() → nova paginaAtual={paginaAtual + 1}/{dm.ContarPaginas()}");
    }

    // 🔧 Atalhos úteis no Editor (menu de contexto do componente)
    [ContextMenu("ForcarAbrir")]
    public void ForcarAbrir()
    {
        Abrir();
    }

    [ContextMenu("AutoVincular (por nome)")]
    public void AutoVincular()
    {
        // Tenta achar um GO plausível para 'painel'
        if (painel == null)
        {
            var p = GameObject.Find("Panel");
            if (p == null)
            {
                var all = GameObject.FindObjectsOfType<RectTransform>(true);
                foreach (var rt in all)
                {
                    if (rt.gameObject.name.ToLower().Contains("panel"))
                    {
                        p = rt.gameObject;
                        break;
                    }
                }
            }
            painel = p;
        }

        // Tenta achar um TMP plausível para 'textoPagina'
        if (textoPagina == null && painel != null)
        {
            var tmps = painel.GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var t in tmps)
            {
                if (t.name.ToLower().Contains("texto") || t.name.ToLower().Contains("diario"))
                {
                    textoPagina = t;
                    break;
                }
            }
        }

        Debug.Log($"[DiarioUI] AutoVincular → painel={(painel ? painel.name : "null")}, textoPagina={(textoPagina ? textoPagina.name : "null")}");
    }
}
