using UnityEngine;

public class PontoDeAtaqueSegueJogador : MonoBehaviour
{
    [Header("Refer�ncias")]
    [Tooltip("O Transform do jogador (personagem principal).")]
    public Transform jogador; // Refer�ncia ao Transform do jogador na cena

    [Tooltip("A coordenada Y fixa onde este ponto deve permanecer (altura).")]
    public float alturaFixaY = 10f; // Altura fixa onde o ponto ficar� (geralmente fora da tela)

    private void Update()
    {
        // Verifica se a refer�ncia ao jogador foi atribu�da
        if (jogador == null)
        {
            Debug.LogError("Refer�ncia ao jogador est� faltando no PontoDeAtaqueSegueJogador.");
            return; // Interrompe a execu��o para evitar erros
        }

        // 1. Pega a posi��o X atual do jogador (horizontal)
        float jogadorX = jogador.position.x;

        // 2. Mant�m a coordenada Y sempre fixa (geralmente acima da tela para spawn de proj�teis)
        float novaY = alturaFixaY;

        // 3. Atualiza a posi��o deste objeto para acompanhar o jogador no eixo X
        //    mas sempre na mesma altura no eixo Y e com o mesmo valor no eixo Z
        transform.position = new Vector3(jogadorX, novaY, transform.position.z);
    }

    // Fun��o chamada no Editor da Unity para desenhar guias visuais no modo "Gizmos"
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta; // Define a cor da linha que ser� desenhada
        // Desenha uma linha horizontal indicando a altura fixa no editor
        Gizmos.DrawLine(new Vector3(transform.position.x - 5f, alturaFixaY, transform.position.z),
                        new Vector3(transform.position.x + 5f, alturaFixaY, transform.position.z));
    }
}
