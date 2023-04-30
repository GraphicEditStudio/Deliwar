using System;
using Code;
using UnityEngine;

public enum PlayerMoveState
{
    None,
    Idle,
    Walk,
    Jump
}
public class PlayerScript : MonoBehaviour
{
    [SerializeField] private PlayerAnimator _playerAnimator;
    [SerializeField] private PlayerPackageController _packageController;
    
    [SerializeField] private PlayerMoveState _playerMoveState;
    public Rigidbody2D rb; 
    public float speed = 5f; 
    public float jumpForce = 10f; 
    public float groundCheckDistance = 1f;
    float horizontalInput;
    bool jumpInput;

    public bool isGrounded = false;

    private void Awake()
    {
        _packageController.OnPackageContainerChanged += OnPackageContainerChanged;
    }

    private void OnPackageContainerChanged()
    {
        SetAnimation();
    }

    private void OnDestroy()
    {
        _packageController.OnPackageContainerChanged -= OnPackageContainerChanged;
    }

    private void Update()
    {
        CheckGround();
        SetInputParams();
        CheckJumpAction();
        CheckAttackAction();
        CheckState();
    }

    private void CheckAttackAction()
    {
        
    }

    private void CheckState()
    {
        if (!isGrounded && _playerMoveState != PlayerMoveState.Jump)
        {
            _playerMoveState = PlayerMoveState.Jump;
            SetAnimation();
        }
        else if (horizontalInput == 0 && _playerMoveState != PlayerMoveState.Idle && isGrounded)
        {
            _playerMoveState = PlayerMoveState.Idle;
            SetAnimation();
        }
        else if (horizontalInput != 0 && _playerMoveState != PlayerMoveState.Walk && isGrounded)
        {
            _playerMoveState = PlayerMoveState.Walk;
            SetAnimation();
        }
    }

    private void SetAnimation()
    {
        switch (_playerMoveState)
        {
            case PlayerMoveState.None:
            case PlayerMoveState.Idle:
                _playerAnimator.SetIdleAnim();
                break;
            case PlayerMoveState.Walk:
                _playerAnimator.SetWalkAnim();
                break;
            case PlayerMoveState.Jump:
                _playerAnimator.SetJumpAnim();
                break;
            default:
                _playerAnimator.SetIdleAnim();
                break;
        }
    }

    private void CheckJumpAction()
    {
        if (jumpInput && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void SetInputParams()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetKeyDown(KeyCode.Space);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);
    }

    private void CheckGround()
    {
        var col = Physics2D.OverlapCircle(transform.position, groundCheckDistance,
            LayerConstants.GroundLayerMask);
        if (col != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rb.velocity; 
        position.x = horizontalInput * speed;
        rb.velocity = position; 
    }
}