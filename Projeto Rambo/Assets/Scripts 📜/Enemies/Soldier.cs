using UnityEngine;

public class Soldier : MonoBehaviour
{
    public float velocidade;
    public float distance;

    bool isRight = true;

    public Transform groundDetector;

    void Update()
    {
        transform.Translate(Vector2.right * velocidade * Time.deltaTime);

        RaycastHit2D ground = Physics2D.Raycast(groundDetector.position, Vector2.down, distance); // Detecta o ch�o, e se tem inimigo.

        if (ground.collider == false) // verifica sde � falso para limitar o movimento do inimigo de acordo com o tamanho do ch�o em tempo real.
        {
            if(isRight == true)
            {
                transform.eulerAngles = new Vector3(0, 0, 0); 
                isRight = false; // muda a dire��o do inimigo
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 180, 0); // Rotaciona o inimigo
                isRight = true; // muda a dire��o do inimigo
            }
        }
    }

}
