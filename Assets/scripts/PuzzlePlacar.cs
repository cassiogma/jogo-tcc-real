using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PuzzlePlacar : MonoBehaviour
{
    [Header("Inputs")]
    public TMP_InputField placarA;
    public TMP_InputField placarB;

    [Header("Código correto")]
    public string codigoA = "4";
    public string codigoB = "0";

    [Header("UI")]
    public GameObject painelPuzzle;

    public void ValidarPuzzle()
    {
        string entradaA = placarA.text;
        string entradaB = placarB.text;

        if (!NumerosSaoUnicos(entradaA) || !NumerosSaoUnicos(entradaB))
        {
            HUDMensagens.instance?.MostrarMensagemPor("Use números diferentes em cada placar!", 2f);
            return;
        }

        if (entradaA == codigoA && entradaB == codigoB)
        {
            HUDMensagens.instance?.MostrarMensagemPor("Puzzle resolvido!", 2f);
            painelPuzzle.SetActive(false);
            Debug.Log("[PuzzlePlacar] Puzzle resolvido.");
        }
        else
        {
            HUDMensagens.instance?.MostrarMensagemPor("Código incorreto.", 2f);
            Debug.Log("[PuzzlePlacar] Código incorreto.");
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