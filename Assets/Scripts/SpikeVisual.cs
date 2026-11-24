using UnityEngine;

/// <summary>
/// Troca a sprite do espinho por uma das variacoes da folha de sprites
/// (espinhos-removebg-preview_0 ... _4). Assim cada espinho fica diferente.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpikeVisual : MonoBehaviour
{
    public Sprite[] variants;

    private void Awake()
    {
        if (variants == null || variants.Length == 0)
            return;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        if (renderer == null)
            return;

        renderer.sprite = variants[Random.Range(0, variants.Length)];
        renderer.flipX = Random.value < 0.5f;
    }
}
