using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class PlatformController : MonoBehaviour
{
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
    private int coinCounter = 0;
    [SerializeField] private TextMeshProUGUI coinText;
    private PlayerActionMap inputActions;
    private InputAction moveAction;
    private InputAction jumpAction;
    
    private Vector2 moveInput;
    private bool isJumping;
    private bool isGrounded;
    
    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        inputActions = new PlayerActionMap();
        
        moveAction = inputActions.player.move;
        jumpAction = inputActions.player.jump;
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
        moveInput = moveAction.ReadValue<Vector2>();
        
        CheckGrounded();
        
        ApplyJumpPhysics();
    }
    
    private void FixedUpdate()
    {
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
    }
    
    private void MovePlayer()
    {
        rb.linearVelocity = new Vector2(
            moveInput.x * moveSpeed,
            rb.linearVelocity.y
        );
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
        coinCounter+=1;
        coinText.text = "coins: " + coinCounter.ToString();
    }

    public void Die(){
         Destroy(gameObject);
         ReloadCurrentScene();

    }

    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
       SceneManager.LoadSceneAsync(currentSceneName);
    }
}