// TestDiarioBootstrap.cs (remova depois de testar)
using UnityEngine;

public class TESTE : MonoBehaviour
{
    void Start()
    {
        var dm = DiarioManager.GetOrCreate();
        dm.ColetarDiario();
        dm.AdicionarPagina("<b>Entrada 1</b>\nA noite está estranha...");
        dm.AdicionarPagina("Entrada 2\nOuvi passos no corredor.");
    }
}
