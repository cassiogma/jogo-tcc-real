// PortaControler.cs
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PortaControler : MonoBehaviour
{
    [Header("Teleporte")]
    public Transform destino;

    [Tooltip("Se usar fade, o teleporte acontece no breu. Se não, usa o delayTeleport abaixo.")]
    public bool usarFade = true;
    public float fadeOutDur = 0.35f;
    public float fadeHold = 0.05f;
    public float fadeInDur = 0.35f;

    [Tooltip("Usado APENAS se 'usarFade' = false")]
    public float delayTeleport = 1.5f;

    [Header("Configuração da Porta")]
    public string chaveNecessaria = "ChavePorta1"; // vazio = porta livre

    private GameObject player;
    private bool playerPerto = false;
    private bool emUso = false;
    private Animator anim;

    private void OnValidate()
    {
        var col = GetComponent<Collider2D>();
        if (col && !col.isTrigger) col.isTrigger = true;
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        if (player == null) Debug.LogError($"[{name}] Player com Tag 'Player' não encontrado.");
    }

    void Update()
    {
        if (!playerPerto || emUso) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool temChave = string.IsNullOrWhiteSpace(chaveNecessaria) ||
                            (InventarioPlayer.instance != null && InventarioPlayer.instance.TemChave(chaveNecessaria));

            if (!temChave)
            {
                HUDMensagens.instance?.MostrarMensagemPor($"A porta está trancada — precisa da {chaveNecessaria}.", 2f);
                return;
            }

            emUso = true;
            anim?.SetTrigger("Abrir");
            HUDMensagens.instance?.LimparMensagem();

            if (usarFade)
            {
                // Garante fader
                var fader = ScreenFader.instance ?? new GameObject("ScreenFader").AddComponent<ScreenFader>();
                StartCoroutine(AbrirComFade(fader));
            }
            else
            {
                StartCoroutine(TeleportarComDelay(delayTeleport));
            }
        }
    }

    private IEnumerator AbrirComFade(ScreenFader fader)
    {
        yield return fader.FadeOutIn(fadeOutDur, fadeHold, fadeInDur, () =>
        {
            if (player != null && destino != null)
                player.transform.position = destino.position;
        });
        emUso = false;
    }

    private IEnumerator TeleportarComDelay(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        if (player != null && destino != null)
            player.transform.position = destino.position;
        emUso = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerPerto = true;

        bool temChave = string.IsNullOrWhiteSpace(chaveNecessaria) ||
                        (InventarioPlayer.instance != null && InventarioPlayer.instance.TemChave(chaveNecessaria));

        if (HUDMensagens.instance != null)
        {
            if (temChave) HUDMensagens.instance.MostrarMensagem("Pressione E para abrir");
            else HUDMensagens.instance.MostrarMensagem($"Trancada — precisa da {chaveNecessaria}");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerPerto = false;
        HUDMensagens.instance?.LimparMensagem();
    }
}
