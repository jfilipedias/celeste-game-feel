using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    #region Attributes
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;

    private SpriteRenderer playerSpriteRenderer;
    private PlayerController playerController;

    private bool canDash = true;
    private bool isFlipped = false;
    #endregion

    #region Engine Methods
    private void Awake()
    {
        playerSpriteRenderer = this.GetComponent<SpriteRenderer>();
        playerController = this.GetComponent<PlayerController>();
    }

    private void Update()
    {
        HandleSprite();
    }
    #endregion

    #region Class Methods
    private void HandleSprite()
    {
        canDash = playerController.CanDash;

        if (canDash)
            SetRedSprite();
        else
            SetBlueSprite();

        if (isFlipped != playerController.IsFlipped)
            FlipSprite();

    }

    private void SetRedSprite()
    {
        playerSpriteRenderer.sprite = redSprite;
    }

    private void SetBlueSprite()
    {
        playerSpriteRenderer.sprite = blueSprite;
    }

    private void FlipSprite()
    {
        isFlipped = playerController.IsFlipped;
        playerSpriteRenderer.flipX = isFlipped;
    }
    #endregion
}
