using NicoUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class MouseController : Singleton<MouseController>
{
    [SerializeField] private MouseControllerState currentState;
    [Header("Movement")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private string gameplayMap;
    [SerializeField] private string menusMap;
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
    [SerializeField] private MouseHelmet mouseHelmet;

    private void Update()
    {
        switch (currentState)
        {
            case MouseControllerState.Moving:
                VisualsFlip();
                Animations();
                break;
            case MouseControllerState.Shopping:
                break;
        }
    }
    private void FixedUpdate()
    {
        switch (currentState)
        {
            case MouseControllerState.Moving:
                Move();
                break;
        }
    }
    public void ChangeState(MouseControllerState newControllerState)
    {
        currentState = newControllerState;
        switch(currentState)
        {
            case MouseControllerState.Moving:
                playerInput.SwitchCurrentActionMap(gameplayMap);
                break;
            case MouseControllerState.Shopping:
                playerInput.SwitchCurrentActionMap(menusMap);
                break;
        }
    }
    public void Initialize()
    {
        startingVisualsPos = playerArtRenderer.transform.localPosition;
        runningBooleanHash = Animator.StringToHash(runningBoolean);
        ChangeState(MouseControllerState.Moving);
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
    public void EquipHelmet(HelmetTypes interactedHelmet)
    {
        mouseHelmet.SwitchHelmet(interactedHelmet);
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
    public void OnSubmit(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Started:
                //TriedInteracting();
                Debug.Log("Submiting on menu");
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
                ChangeState(MouseControllerState.Shopping);
                currentInteractable.Perform();
                break;
            case InteractionType.HelmetEquip:
                currentInteractable.Perform();
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentInteractable = collision.GetComponentInParent<IInteractable>();
        if(currentInteractable != null)
            currentInteraction = currentInteractable.InteractionType;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        currentInteractable = collision.GetComponentInParent<IInteractable>();
        if (currentInteractable != null)
            currentInteraction = InteractionType.None;
    }
    #endregion
}

public enum MouseControllerState
{
    Moving,
    Shopping,
}