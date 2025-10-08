using UnityEngine;

public class InteracaoItemEspecial : MonoBehaviour
{
    public string itemID = "Item";
    public GameObject itemUI; // Ícone ou texto no canto superior direito

    private bool itemColetado = false;

    public void Interagir()
    {
        if (itemColetado) return;

        itemColetado = true;

        InventarioItensEspeciais.instance?.AdicionarItem(itemID);

        if (itemUI != null)
            itemUI.SetActive(true);

        Debug.Log($"Item especial '{itemID}' coletado.");
    }
}

