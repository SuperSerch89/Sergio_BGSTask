using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MouseController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveInput = Vector2.zero;

    [Header("Visuals")]
    [SerializeField] private SpriteRenderer playerArtRenderer;
    [SerializeField] private float artOffsetOnLookingRight;
    [SerializeField] private Transform clothesTransform;

    private Vector2 startingVisualsPos;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runningBoolean = "Running";
    private int runningBooleanHash;

    [Header("Interactions")]
    [SerializeField] private InteractionType currentInteraction = InteractionType.None;
    private IInteractable currentInteractable;

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
            clothesTransform.localScale = Vector2.one;
        }
        else if (moveInput.x < 0)
        {
            playerArtRenderer.flipX = true;
            playerArtRenderer.transform.localPosition = new Vector2(-artOffsetOnLookingRight, startingVisualsPos.y);
            clothesTransform.localScale = new Vector2(-1, 1);
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
    public void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                TriedInteracting();
                break;
        }
    }
    #endregion

    #region Interactions
    private void TriedInteracting()
    {
        switch (currentInteraction)
        {
            case InteractionType.Shop:
                Debug.Log("interacting with shop");
                currentInteractable.Perform();
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentInteractable = collision.GetComponentInParent<IInteractable>();
        switch (currentInteractable.InteractionType)
        {
            case InteractionType.Shop:
                currentInteraction = InteractionType.Shop;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentInteractable = collision.GetComponentInParent<IInteractable>();
        if (currentInteractable != null)
            currentInteraction = InteractionType.None;
    }
    #endregion
}
