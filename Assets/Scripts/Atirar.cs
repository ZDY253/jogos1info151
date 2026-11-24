
using UnityEngine;
using UnityEngine.InputSystem;

public class Atirar : MonoBehaviour
{
    private PlatformController player;
    private PlayerActionMap inputActions;
    private InputAction shootAction;

    private void Awake()
    {
        player = GetComponentInParent<PlatformController>();

        inputActions = new PlayerActionMap();
        shootAction = inputActions.player.Shoot;

        if (player == null)
        {
            Debug.LogError("PlatformController não encontrado!");
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
        shootAction.performed += Disparar;
    }

    private void OnDisable()
    {
        shootAction.performed -= Disparar;
        inputActions.Disable();
    }

    private void Disparar(InputAction.CallbackContext context)
    {
        if (player != null)
        {
            player.Atirar();
        }
    }
}
