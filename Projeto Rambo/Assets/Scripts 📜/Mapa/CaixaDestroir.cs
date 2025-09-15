using UnityEngine;

public class CaixaTeste : MonoBehaviour
{
    [System.Serializable]
    public class ItemDrop
    {
        public GameObject itemPrefab;   // Prefab do item (deixe null para "nenhum drop")
        [Range(0f, 100f)] public float chance; // Chance de spawn em %
    }

    [Header("Itens que podem dropar da caixa")]
    public ItemDrop[] itensPossiveis;

    // Método público que a bala chama
    public void DestruirCaixa()
    {
        GerarItem();
        Destroy(gameObject);
    }

    private void GerarItem()
    {
        // Soma total das chances
        float totalChance = 0f;
        foreach (ItemDrop drop in itensPossiveis)
            totalChance += drop.chance;

        if (totalChance <= 0f)
        {
            Debug.Log("Nenhum item dropado (total de chance = 0)");
            return;
        }

        // Sorteia um valor entre 0 e totalChance
        float sorteio = Random.Range(0f, totalChance);
        float acumulado = 0f;

        for (int i = 0; i < itensPossiveis.Length; i++)
        {
            acumulado += itensPossiveis[i].chance;

            // Se for o último item, força o drop nele
            if (sorteio < acumulado || i == itensPossiveis.Length - 1)
            {
                if (itensPossiveis[i].itemPrefab != null)
                {
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