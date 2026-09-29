using UnityEngine;

public class Atirar : MonoBehaviour
{
    public GameObject prefabProjetil;
    public Transform pontoDisparo;
    
    // Update is called once per frame
    void Update()
    {
        {
            // Verifica se o botão esquerdo do mouse ou a barra de espaço foi pressionada
            if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            {
                Disparar();
            }
        

    }
}
    void Disparar()
    {
        Instantiate(prefabProjetil, pontoDisparo.position, pontoDisparo.rotation);
    }
}
