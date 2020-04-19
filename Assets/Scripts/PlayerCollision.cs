﻿using System.Collections;
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

    [Space]
    [Header("Collision Transform")]
    [SerializeField] private Transform hand;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform ledgeOffset;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;

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
        Gizmos.DrawWireSphere(feet.position, sphereGizmoSize);
        Gizmos.DrawLine(feet.position, (feet.position + new Vector3(wallCollisionDistance * facingDirection, 0, 0)));

        // Hand
        Gizmos.DrawWireSphere(hand.position, sphereGizmoSize);
        Gizmos.DrawLine(hand.position, (hand.position + new Vector3(wallCollisionDistance * facingDirection, 0, 0)));

        // Foot
        Gizmos.DrawWireSphere(rightFoot.position, sphereGizmoSize);
        Gizmos.DrawWireSphere(leftFoot.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rightFoot.position, (rightFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
        Gizmos.DrawLine(leftFoot.position, (leftFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
    }
    #endregion

    #region Class Methods
    public bool CheckGroundCollision()
    {
        Vector2 rightRayStart = (Vector2)rightFoot.position;
        Vector2 leftRayStart = (Vector2)leftFoot.position;

        bool rightFootOnGround = Physics2D.Raycast(rightRayStart, Vector2.down, groundCollisionDistance, groundMask);
        bool leftFootOnGround = Physics2D.Raycast(leftRayStart, Vector2.down, groundCollisionDistance, groundMask);

        return rightFootOnGround || leftFootOnGround;

    }

    public bool CheckWallCollistion(float facingDirection)
    {
        Vector2 feetRayStart = (Vector2)feet.position;
        Vector2 handRayStart = (Vector2)hand.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask) ||
               Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool CheckHandsOnWall(float facingDirection)
    {
        Vector2 handRayStart = (Vector2)hand.position;

        return Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool CheckFeetOnWall(float facingDirection)
    {
        Vector2 feetRayStart = (Vector2)feet.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance,groundMask);
    }
    #endregion
}
