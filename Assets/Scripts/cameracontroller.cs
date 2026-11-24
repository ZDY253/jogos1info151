using UnityEngine;

/// <summary>
/// Camera que segue o jogador com suavizacao.
/// </summary>
public class cameracontroller : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 offset = new Vector3(0f, 1f, 0f);
    public float smoothTime = 0.15f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (playerTransform == null)
            return;

        Vector3 target = playerTransform.position + offset;
        target.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
}
