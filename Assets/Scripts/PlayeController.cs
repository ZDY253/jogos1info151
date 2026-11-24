using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformController : MonoBehaviour
{

    [Header("Shooting Settings")]
    [SerializeField] private GameObject projetilPrefab;
    [SerializeField] private Transform pontoDeDisparo;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;

    [Header("coin setting")]
    [SerializeField] private TextMeshProUGUI coinText;
    private PlayerActionMap inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;

    [Header("Animation settings")]
    [SerializeField] private Animator anim;


    private bool olhandoDireita = true;

    public bool OlhandoDireita => olhandoDireita;
    private Vector2 moveInput;
    private bool isJumping;
    private bool isGrounded;
    private bool controlEnabled = true;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        inputActions = new PlayerActionMap();

        if (anim == null)
            anim = GetComponent<Animator>();

        moveAction = inputActions.player.move;
        jumpAction = inputActions.player.jump;
    }

    /// <summary>Usado pelo PlayerHealth para congelar o personagem ao morrer.</summary>
    public void SetControlEnabled(bool value)
    {
        controlEnabled = value;

        if (value)
        {
            moveAction.Enable();
            jumpAction.Enable();
        }
        else
        {
            moveInput = Vector2.zero;
            isJumping = false;
            moveAction.Disable();
            jumpAction.Disable();

            if (rb != null)
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (anim != null)
            {
                anim.SetBool("is Moving", false);
                anim.SetBool("OnGround", false);
            }
        }
    }


    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        jumpAction.performed += OnJumpPerformed;
        jumpAction.canceled += OnJumpCanceled;
    }

    private void OnDisable()
    {
        // Unsubscribe from input events
        jumpAction.performed -= OnJumpPerformed;
        jumpAction.canceled -= OnJumpCanceled;

        moveAction.Disable();
        jumpAction.Disable();
    }

    private void Update()
    {
        if (!controlEnabled)
            return;

        moveInput = moveAction.ReadValue<Vector2>();

        CheckGrounded();

        ApplyJumpPhysics();
    }

    private void FixedUpdate()
    {
        if (!controlEnabled)
            return;

        // Apply movement
        MovePlayer();
    }

    private void CheckGrounded()
    {
        if (groundCheckPoint != null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(
                groundCheckPoint.position,
                groundCheckRadius,
                groundLayer
            );
            isGrounded = colliders.Length > 0;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                Vector2.down,
                1.1f,
                groundLayer
            );
            isGrounded = hit.collider != null;
        }

        anim.SetBool("OnGround", isGrounded);
    }

    private void MovePlayer()
    {
        rb.linearVelocity = new Vector2(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y
        );

        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f;

        if (anim != null)
            anim.SetBool("is Moving", isMoving);

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (sprite != null)
        {
            if (moveInput.x > 0.1f)
            {
                sprite.flipX = false;
                olhandoDireita = true;
            }
            else if (moveInput.x < -0.1f)
            {
                sprite.flipX = true;
                olhandoDireita = false;
            }




        }
    }

    private void ApplyJumpPhysics()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !isJumping)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            // Apply jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
        else
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, Vector2.down * 1.1f);
        }
    }
    public void ChangeTextCoin()
    {
        if (coinText == null || GameManager.Instance == null)
            return;

        coinText.text = "Moedas: " + GameManager.Instance.LevelCoins;
    }

    public void Die()
    {
        if (GetComponent<PlayerHealth>() != null)
        {
            GetComponent<PlayerHealth>().Kill("Perigo");
            return;
        }

        ReloadCurrentScene();

    }

    public void ReloadCurrentScene()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
        else
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }


    public void Atirar()
    {
        if (projetilPrefab == null || pontoDeDisparo == null)
        {
            Debug.LogError("Prefab ou ponto de disparo não configurado!");
            return;
        }

        GameObject tiro = Instantiate(
            projetilPrefab,
            pontoDeDisparo.position,
            Quaternion.identity
        );

        Projetil scriptTiro = tiro.GetComponent<Projetil>();

        if (scriptTiro != null)
        {
            scriptTiro.DefinirDirecao(olhandoDireita);
        }
    }
}

