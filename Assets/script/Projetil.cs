using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 12f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
