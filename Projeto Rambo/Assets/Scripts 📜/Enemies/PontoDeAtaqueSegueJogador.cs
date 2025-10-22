using UnityEngine;

public class PontoDeAtaqueSegueJogador : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O Transform do jogador (personagem principal).")]
    public Transform jogador; // Referência ao Transform do jogador na cena

    [Tooltip("A coordenada Y fixa onde este ponto deve permanecer (altura).")]
    public float alturaFixaY = 10f; // Altura fixa onde o ponto ficará (geralmente fora da tela)

    private void Update()
    {
        // Verifica se a referência ao jogador foi atribuída
        if (jogador == null)
        {
            Debug.LogError("Referência ao jogador está faltando no PontoDeAtaqueSegueJogador.");
            return; // Interrompe a execução para evitar erros
        }

        // 1. Pega a posição X atual do jogador (horizontal)
        float jogadorX = jogador.position.x;

        // 2. Mantém a coordenada Y sempre fixa (geralmente acima da tela para spawn de projéteis)
        float novaY = alturaFixaY;

        // 3. Atualiza a posição deste objeto para acompanhar o jogador no eixo X
        //    mas sempre na mesma altura no eixo Y e com o mesmo valor no eixo Z
        transform.position = new Vector3(jogadorX, novaY, transform.position.z);
    }

    // Função chamada no Editor da Unity para desenhar guias visuais no modo "Gizmos"
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta; // Define a cor da linha que será desenhada
        // Desenha uma linha horizontal indicando a altura fixa no editor
        Gizmos.DrawLine(new Vector3(transform.position.x - 5f, alturaFixaY, transform.position.z),
                        new Vector3(transform.position.x + 5f, alturaFixaY, transform.position.z));
    }
}
