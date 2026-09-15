using UnityEngine;

public class coincontroller : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other)
   {
    if (other.CompareTag("Player"))
    {
        other.gameObject.SendMessage("ChangeTextCoin");
        Destroy(gameObject);
    }
   }
}
