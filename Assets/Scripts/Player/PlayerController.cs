using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpForce;
    public LayerMask groundLayerMask;

    public Vector3 direction;

    private Rigidbody _rigidbody;
    public Animator _animator;

    private bool isSprint = false;
    private bool canAttack = true;
    private float attackDelay;

    public static PlayerController instance;
    private void Awake()
    {
        instance = this;
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isSprint)
        {
            if (GameManager.Instance.Player.status.Stamina > 0) GameManager.Instance.Player.status.Stamina -= 10f * Time.deltaTime;
        }
        else
        {
            if (GameManager.Instance.Player.status.Stamina < 100) GameManager.Instance.Player.status.Stamina += 5f * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        //Debug.Log(canAttack);
        Move();
        Attack();
    }

    private void Move()
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            _rigidbody.rotation = targetAngle;
        }

        _rigidbody.velocity = direction * moveSpeed + Vector3.up * _rigidbody.velocity.y;

        if (!canAttack) _rigidbody.velocity = Vector3.zero;

        _animator.SetBool("IsRun", _rigidbody.velocity != Vector3.zero);
    }

    private void Attack()
    {
        attackDelay += Time.deltaTime;
        canAttack = 1.0f < attackDelay;
    }


    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") && !Inventory.instance.IsOpen() && context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
            direction = new Vector3(curMovementInput.x, 0f, curMovementInput.y);
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
            direction = Vector3.zero;
        }
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            if (GameManager.Instance.Player.status.Stamina > 0)
            {
                moveSpeed *= 1.5f;
                isSprint = true;
                _animator.SetBool("IsSprint", true);
            }
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            moveSpeed /= 1.5f;
            isSprint = false;
            _animator.SetBool("IsSprint", false);
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        //Debug.Log("SPACE");
        //if (!Inventory.instance.IsOpen() && context.phase == InputActionPhase.Started)
        //{
        //    if (IsGrounded())
        //    {
        //        _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        //        _animator.SetTrigger("IsJump");
        //    }
        //}
    }

    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        Debug.Log("Inventory!");
        if (context.phase == InputActionPhase.Started)
        {
            //ToggleCursor(true);
            Inventory.instance.Toggle();
        }

    }
    public void OnCraftPanelInput(InputAction.CallbackContext context)
    {
        Debug.Log("Crafting!");
        if (context.phase == InputActionPhase.Started)
        {
            CraftPanelUI.instance.Toggle();
        }

    }
    public void OnAddResourcesInput(InputAction.CallbackContext context)
    {
        Debug.Log("재료가 1개씩 늘었다!");
        if (context.phase == InputActionPhase.Started)
        {
            GetComponent<InventoryPlayerTest>().OnClickAddItemsButton();
        }

    }
    public void OnAddRandomItemInput(InputAction.CallbackContext context)
    {
        Debug.Log("랜덤하게 아이템이 생성되었다!");
        if (context.phase == InputActionPhase.Started)
        {
            GetComponent<InventoryPlayerTest>().OnClickAddRandomItemButton();
        }

    }

    public void OnAttackInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (canAttack && !isSprint)
            {
                if (GameManager.Instance.Player.weapon != null)
                {
                    GameManager.Instance.Player.weapon.Use();
                }
                _animator.SetTrigger("DoAttack");
                attackDelay = 0;
            }
        }
    }

    private bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (Vector3.up * 0.01f) , Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f)+ (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (Vector3.up * 0.01f), Vector3.down),
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + (transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.forward * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (transform.right * 0.2f), Vector3.down);
        Gizmos.DrawRay(transform.position + (-transform.right * 0.2f), Vector3.down);
    }

    public void ToggleCursor(bool toggle)
    {
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        //canLook = !toggle;
    }

    private void DecreaseStamina()
    {
        if (GameManager.Instance.Player.status.Stamina > 0) GameManager.Instance.Player.status.Stamina -= 5;
    }

    private void IncraseStamina()
    {
        if (GameManager.Instance.Player.status.Stamina < 100) GameManager.Instance.Player.status.Stamina += 5;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 7) // Interactable
        {
            GameObject item = other.gameObject;
            Inventory.instance.AddItem(item.GetComponent<ItemObject>().item);
            Destroy(other.gameObject);
        }
    }
}