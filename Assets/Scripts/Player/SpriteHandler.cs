using CelesteGameFeel.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteHandler : MonoBehaviour
{
    #region Attributes
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;

    private SpriteRenderer playerSpriteRenderer;
    private Controller controller;

    private bool canDash = true;
    private bool isFlipped = false;
    #endregion

    #region Engine Methods
    private void Awake()
    {
        playerSpriteRenderer = this.GetComponent<SpriteRenderer>();
        controller = this.GetComponent<Controller>();
    }

    private void Update()
    {
        HandleSprite();
    }
    #endregion

    #region Class Methods
    private void HandleSprite()
    {
        canDash = controller.CanDash;

        if (canDash)
            SetRedSprite();
        else
            SetBlueSprite();

        if (isFlipped != controller.IsFlipped)
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
        isFlipped = controller.IsFlipped;
        playerSpriteRenderer.flipX = isFlipped;
    }
    #endregion
}
