using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PuzzleFinal : MonoBehaviour
{
    [Header("Input")]
    public TMP_InputField campoSenha;

    [Header("Código correto")]
    public string codigoCorreto = "123";

    [Header("UI")]
    public GameObject painelPuzzle;

    [Header("Recompensa")]
    public TextAsset paginaDiario;

    public void ValidarSenha()
    {
        string entrada = campoSenha.text;

        if (!NumerosSaoUnicos(entrada))
        {
            HUDMensagens.instance?.MostrarMensagemPor("Use números diferentes!", 2f);
            return;
        }

        if (entrada == codigoCorreto)
        {
            HUDMensagens.instance?.MostrarMensagemPor("Puzzle resolvido!", 2f);
            painelPuzzle.SetActive(false);

            if (paginaDiario != null)
            {
                var dm = DiarioManager.GetOrCreate();
                dm.AdicionarPagina(paginaDiario.text);
                HUDMensagens.instance?.MostrarMensagemPor("Nova página adicionada ao diário!", 2.5f);
                Debug.Log($"[PuzzleSenhaUnica] Página adicionada ao diário: {paginaDiario.name}");
            }

            Debug.Log("[PuzzleSenhaUnica] Puzzle resolvido.");
        }
        else
        {
            HUDMensagens.instance?.MostrarMensagemPor("Código incorreto.", 2f);
            Debug.Log("[PuzzleSenhaUnica] Código incorreto.");
        }
    }

    private bool NumerosSaoUnicos(string texto)
    {
        var set = new HashSet<char>();
        foreach (char c in texto)
        {
            if (!char.IsDigit(c)) return false;
            if (!set.Add(c)) return false;
        }
        return true;
    }
}
