using UnityEngine;
using TMPro;
using System.Collections;

public class HUDMensagens : MonoBehaviour
{
    public static HUDMensagens instance;
    public TextMeshProUGUI textoUI;

    private Coroutine rotinaAutoLimpar;

    private void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }

        if (textoUI == null)
        {
            Debug.LogError("[HUD] TextMeshProUGUI 'textoUI' não atribuído no Inspector.");
        }
        else
        {
            textoUI.text = "";
        }
    }

    public void MostrarMensagem(string msg)
    {
        if (textoUI == null) return;

        if (rotinaAutoLimpar != null) { StopCoroutine(rotinaAutoLimpar); rotinaAutoLimpar = null; }
        textoUI.text = msg;
    }

    public void MostrarMensagemPor(string msg, float segundos)
    {
        MostrarMensagem(msg);
        if (segundos > 0f)
        {
            if (rotinaAutoLimpar != null) StopCoroutine(rotinaAutoLimpar);
            rotinaAutoLimpar = StartCoroutine(AutoLimpar(segundos));
        }
    }

    public void LimparMensagem()
    {
        if (textoUI != null) textoUI.text = "";
        if (rotinaAutoLimpar != null) { StopCoroutine(rotinaAutoLimpar); rotinaAutoLimpar = null; }
    }

    private IEnumerator AutoLimpar(float s)
    {
        yield return new WaitForSeconds(s);
        LimparMensagem();
    }
}
