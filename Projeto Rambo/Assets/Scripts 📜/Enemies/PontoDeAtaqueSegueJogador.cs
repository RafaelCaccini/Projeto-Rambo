using UnityEngine;

public class PontoDeAtaqueSegueJogador : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O Transform do jogador (personagem principal).")]
    public Transform jogador;

    [Tooltip("A coordenada Y fixa onde este ponto deve permanecer (altura).")]
    public float alturaFixaY = 10f; // Ajuste este valor na Unity (deve ser fora da tela, no alto)

    private void Update()
    {
        if (jogador == null)
        {
            Debug.LogError("Referência ao jogador está faltando no PontoDeAtaqueSegueJogador.");
            return;
        }

        // 1. Obtém a coordenada X do jogador
        float jogadorX = jogador.position.x;

        // 2. Mantém a coordenada Y na altura fixa (fora da tela)
        float novaY = alturaFixaY;

        // 3. Aplica a nova posição
        transform.position = new Vector3(jogadorX, novaY, transform.position.z);
    }

    // Ajuda visual no Editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        // Desenha uma linha vertical para mostrar a altura fixa
        Gizmos.DrawLine(new Vector3(transform.position.x - 5f, alturaFixaY, transform.position.z),
                        new Vector3(transform.position.x + 5f, alturaFixaY, transform.position.z));
    }
}