using UnityEngine;
using UnityEngine.InputSystem;

public class Projetil : MonoBehaviour
{
    public float velocidade = 12f;
    private Rigidbody2D rb;
    public float initialVelocity;
    private bool lookingRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (lookingRight)
        {
            rb.linearVelocity= Vector2.right * initialVelocity;
        }
        else
        {
            rb.linearVelocity = Vector2.left * initialVelocity;
        }


    }

    void Start()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.right * velocidade;
        }
        Destroy(gameObject, 3f); // Destrói a bala após 3 segundos
    }

    // 1. Testa colisão se a bala encostar em um colisor sólido
    private void OnCollisionEnter2D(Collision2D colisao)
    {
        VerificarImpacto(colisao.gameObject);
    }

    // 2. Testa colisão se a bala passar por dentro de um Trigger
    private void OnTriggerEnter2D(Collider2D colisao)
    {
        VerificarImpacto(colisao.gameObject);
    }

    // Função única que lida com a destruição
    void VerificarImpacto(GameObject objetoAtingido)
    {
        if (objetoAtingido.CompareTag("Enemy"))
        {
            Destroy(objetoAtingido); // Destrói o inimigo
            Destroy(gameObject);      // Destrói a bala
        }
    }
}
