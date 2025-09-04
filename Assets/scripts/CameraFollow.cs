using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;          // Refer�ncia ao jogador
    public float smoothSpeed = 0.1f;  // Velocidade de suaviza��o do movimento
    public Vector3 offset;            // Deslocamento para manter a c�mera atr�s ou acima do jogador

    private void LateUpdate()
    {
        if (player == null) return;

        Vector3 posicaoDesejada = player.position + offset;
        Vector3 posicaoSuave = Vector3.Lerp(transform.position, posicaoDesejada, smoothSpeed);
        transform.position = posicaoSuave;
    }
}
