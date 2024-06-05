using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveInput = Vector2.zero;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer playerArtRenderer;
    [SerializeField] private float artOffsetOnLookingRight;
    private Vector2 startingVisualsPos;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runningBoolean = "Running";
    private int runningBooleanHash;


    private void Awake()
    {
        startingVisualsPos = playerArtRenderer.transform.localPosition;
        runningBooleanHash = Animator.StringToHash(runningBoolean);
    }
    private void Update()
    {
        VisualsFlip();
        Animations();
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = moveInput * moveSpeed;
    }
    private void VisualsFlip()
    {
        if (moveInput.x > 0)
        {
            playerArtRenderer.flipX = false;
            playerArtRenderer.transform.localPosition = new Vector2(artOffsetOnLookingRight, startingVisualsPos.y);
        }
        else if (moveInput.x < 0)
        {
            playerArtRenderer.flipX = true;
            playerArtRenderer.transform.localPosition = new Vector2(-artOffsetOnLookingRight, startingVisualsPos.y);
        }
    }
    private void Animations()
    {
        if(rb.velocity == Vector2.zero)
            animator.SetBool(runningBooleanHash, false);
        else
            animator.SetBool(runningBooleanHash, true);
    }

    #region PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }
    #endregion
}
