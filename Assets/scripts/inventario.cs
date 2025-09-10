using UnityEngine;
using System.Collections.Generic;

public class InventarioPlayer : MonoBehaviour
{
    public static InventarioPlayer instance;

    private HashSet<string> chaves = new HashSet<string>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AdicionarChave(string chave)
    {
        chaves.Add(chave);
        Debug.Log("Chave coletada: " + chave);
    }

    public bool TemChave(string chave)
    {
        return chaves.Contains(chave);
    }
}
