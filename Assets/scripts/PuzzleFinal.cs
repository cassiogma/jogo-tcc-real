using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PuzzleFinal : MonoBehaviour
{
    [Header("Input")]
    public TMP_InputField campoSenha;

    [Header("Código correto")]
    public string codigoCorreto = "123";

    [Header("UI")]
    public Image telaFade; // Imagem preta para o fade (alpha inicial = 0)
    public float duracaoFade = 2f;

    [Header("Recompensa")]
    public TextAsset paginaDiario;

    [Header("Cena para carregar")]
    public string nomeCena; // Nome da cena após resolver o puzzle

    public void ValidarSenha()
    {
        string entrada = campoSenha.text.Trim(); // Remove espaços extras

        if (entrada == codigoCorreto)
        {
            HUDMensagens.instance?.MostrarMensagemPor("Puzzle resolvido!", 2f);

            if (paginaDiario != null)
            {
                var dm = DiarioManager.GetOrCreate();
                dm.AdicionarPagina(paginaDiario.text);
                HUDMensagens.instance?.MostrarMensagemPor("Nova página adicionada ao diário!", 2.5f);
                Debug.Log($"[PuzzleFinal] Página adicionada ao diário: {paginaDiario.name}");
            }

            Debug.Log("[PuzzleFinal] Puzzle resolvido.");
            StartCoroutine(FazerFadeETrocarCena());
        }
        else
        {
            HUDMensagens.instance?.MostrarMensagemPor("Código incorreto.", 2f);
            Debug.Log("[PuzzleFinal] Código incorreto.");
        }
    }

    private IEnumerator FazerFadeETrocarCena()
    {
        telaFade.gameObject.SetActive(true); // Ativa a imagem só no momento do fade
        Color cor = telaFade.color;
        float tempo = 0f;

        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0f, 1f, tempo / duracaoFade);
            telaFade.color = cor;
            yield return null;
        }

        SceneManager.LoadScene(nomeCena); // Troca de cena após o fade
    }
}