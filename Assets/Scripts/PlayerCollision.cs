using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    #region Atributes
    [Header("Check Collision")]
    [SerializeField] private float groundCollisionDistance;
    [SerializeField] private float wallCollisionDistance;
    [SerializeField] private float sphereGizmoSize;
    [SerializeField] private LayerMask groundMask;

    [Header("Collision Transform")]
    [SerializeField] private Transform playerHand;
    [SerializeField] private Transform playerFeet;
    [SerializeField] private Transform playerRightFoot;
    [SerializeField] private Transform playerLeftFoot;

    private float facingDirection = Vector2.right.x;

    private PlayerController playerController;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        playerController = this.GetComponent<PlayerController>();
    }
   
    private void Update()
    {
        facingDirection = playerController.FacingDirection;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        // Face
        Gizmos.DrawWireSphere(playerFeet.position, sphereGizmoSize);
        Gizmos.DrawLine(playerFeet.position, (playerFeet.position + new Vector3(wallCollisionDistance * facingDirection, 0, 0)));

        // Hand
        Gizmos.DrawWireSphere(playerHand.position, sphereGizmoSize);
        Gizmos.DrawLine(playerHand.position, (playerHand.position + new Vector3(wallCollisionDistance * facingDirection, 0, 0)));

        // Foot
        Gizmos.DrawWireSphere(playerRightFoot.position, sphereGizmoSize);
        Gizmos.DrawWireSphere(playerLeftFoot.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerRightFoot.position, (playerRightFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
        Gizmos.DrawLine(playerLeftFoot.position, (playerLeftFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
    }
    #endregion

    #region Class Methods
    public bool CheckGroundCollision()
    {
        Vector2 rightRayStart = (Vector2)playerRightFoot.position;
        Vector2 leftRayStart = (Vector2)playerLeftFoot.position;

        bool rightFootOnGround = Physics2D.Raycast(rightRayStart, Vector2.down, groundCollisionDistance, groundMask);
        bool leftFootOnGround = Physics2D.Raycast(leftRayStart, Vector2.down, groundCollisionDistance, groundMask);

        return rightFootOnGround || leftFootOnGround ? true : false;

    }

    public bool CheckWallCollistion(float facingDirection)
    {
        Vector2 feetRayStart = (Vector2)playerFeet.position;
        Vector2 handRayStart = (Vector2)playerHand.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask) ||
               Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool CheckHandsOnWall(float facingDirection)
    {
        Vector2 handRayStart = (Vector2)playerHand.position;

        return Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }
    #endregion
}
