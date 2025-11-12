using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class BotaoTrocaCenaComFade : MonoBehaviour
{
    [Header("Configuração")]
    public string nomeCena; // Nome da cena para carregar
    public Image telaFade;  // Imagem UI que cobre a tela (deve ser preta e com alpha 0)
    public float duracaoFade = 3f; // Tempo do fade em segundos

    public void TrocarCenaComFade()
    {
        StartCoroutine(FazerFade());
    }

    private IEnumerator FazerFade()
    {
        if (telaFade == null)
        {
            Debug.LogError("TelaFade não atribuída no Inspector!");
            yield break;
        }

        telaFade.gameObject.SetActive(true);
        Color cor = telaFade.color;
        cor.a = 0f;
        telaFade.color = cor;

        float tempo = 0f;
        while (tempo < duracaoFade)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0f, 1f, tempo / duracaoFade);
            telaFade.color = cor;
            yield return null;
        }

        SceneManager.LoadScene(nomeCena);
    }
}