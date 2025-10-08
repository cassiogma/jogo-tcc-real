using UnityEngine;
using System.Collections.Generic;

public class InventarioItensEspeciais : MonoBehaviour
{
    public static InventarioItensEspeciais instance;

    private HashSet<string> itens = new HashSet<string>();

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AdicionarItem(string itemID)
    {
        itens.Add(itemID);
        Debug.Log($"Item especial coletado: {itemID}");
    }

    public bool TemItem(string itemID)
    {
        return itens.Contains(itemID);
    }
}