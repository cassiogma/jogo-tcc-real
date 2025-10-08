using System.Collections.Generic;
using UnityEngine;

public class DiarioManager : MonoBehaviour
{
    public static DiarioManager instance;

    [Header("Estado")]
    [SerializeField] private bool temDiario = false;

    [Header("Conteúdo")]
    [TextArea(3, 8)] public List<string> paginas = new List<string>();

    // Evento opcional para UI auto-atualizar quando o conteúdo muda
    public event System.Action OnPagesChanged;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(gameObject); return; }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static DiarioManager GetOrCreate()
    {
        if (instance != null) return instance;
        var go = new GameObject("DiarioManager");
        return go.AddComponent<DiarioManager>(); // Awake será chamado
    }

    public bool TemDiario => temDiario;

    public void ColetarDiario()
    {
        temDiario = true;
        OnPagesChanged?.Invoke();
    }

    public void AdicionarPagina(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return;
        paginas.Add(texto);
        OnPagesChanged?.Invoke();
    }

    public void AdicionarPaginas(IEnumerable<string> textos)
    {
        if (textos == null) return;
        paginas.AddRange(textos);
        OnPagesChanged?.Invoke();
    }

    public int ContarPaginas() => paginas.Count;

    public string ObterPagina(int index)
    {
        if (index < 0 || index >= paginas.Count) return "";
        return paginas[index];
    }
}
