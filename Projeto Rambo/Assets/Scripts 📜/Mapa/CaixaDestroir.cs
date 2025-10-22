using UnityEngine;

public class CaixaTeste : MonoBehaviour
{
    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab;   // O item que pode aparecer
        [Range(0f, 100f)] public float chance; // Chance de aparecer (0 a 100%)
    }

    [Header("Itens que podem dropar da caixa")]
    public ItemDrop[] itensPossiveis; // Lista de possíveis itens

    // Método chamado quando a bala destrói a caixa
    public void DestruirCaixa()
    {
        GerarItem(); // Tenta gerar um item
        Destroy(gameObject); // Destroi a caixa
    }

    private void GerarItem()
    {
        // Soma total das chances dos itens
        float totalChance = 0f;
        foreach (ItemDrop drop in itensPossiveis)
            totalChance += drop.chance;

        if (totalChance <= 0f)
        {
            Debug.Log("Nenhum item dropado (total de chance = 0)");
            return;
        }

        // Sorteia um número aleatório
        float sorteio = Random.Range(0f, totalChance);
        float acumulado = 0f;

        for (int i = 0; i < itensPossiveis.Length; i++)
        {
            acumulado += itensPossiveis[i].chance;

            // Se o número sorteado cair aqui ou for o último item
            if (sorteio < acumulado || i == itensPossiveis.Length - 1)
            {
                if (itensPossiveis[i].itemPrefab != null)
                {
                    // Cria o item no mundo
                    Instantiate(itensPossiveis[i].itemPrefab, transform.position, Quaternion.identity);
                    Debug.Log("Item dropado: " + itensPossiveis[i].itemPrefab.name);
                }
                else
                {
                    Debug.Log("Nenhum item dropado");
                }
                return;
            }
        }
    }
}
