using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    #region Atributes
    [Header("Check Collision")]
    [SerializeField] private float groundCollisionDistance = 0.02f;
    [SerializeField] private float wallCollisionDistance = 0.52f;
    [SerializeField] private float sphereGizmoSize = 0.1f;
    [SerializeField] private float boxShellSize = 0.02f;
    [SerializeField] private Vector2 cornerBoxSize = new Vector2(0.45f, 0.15f);

    [Space]
    [Header("Colision Mask")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask spikeMask;

    [Space]
    [Header("Collision Transform")]
    [SerializeField] private Transform hand;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform rightFoot;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightHeadSide;
    [SerializeField] private Transform leftHeadSide;

    private float facingDirection = Vector2.right.x;

    private Vector2 boxColliderSize = new Vector2(1, 2);

    private PlayerController controller;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        controller = this.GetComponent<PlayerController>();
        boxColliderSize = this.GetComponent<BoxCollider2D>().size;
    }

    private void Update()
    {
        facingDirection = controller.FacingDirection;
    }

    private void OnDrawGizmos()
    {
        // Face
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(feet.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(feet.position, (feet.position + new Vector3(wallCollisionDistance * facingDirection, 0, 0)));

        // Hand
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(hand.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(hand.position, (hand.position + new Vector3(wallCollisionDistance * facingDirection, 0, 0)));

        // Collision Shell
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(boxColliderSize.x + boxShellSize, boxColliderSize.y + boxShellSize));

        // Head Sides
        Gizmos.DrawWireCube(rightHeadSide.position, cornerBoxSize);
        Gizmos.DrawWireCube(leftHeadSide.position, cornerBoxSize);

        // Foot
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(rightFoot.position, sphereGizmoSize);
        Gizmos.DrawWireSphere(leftFoot.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(rightFoot.position, (rightFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
        Gizmos.DrawLine(leftFoot.position, (leftFoot.position + new Vector3(0, groundCollisionDistance * -1, 0)));
    }
    #endregion

    #region Class Methods
    public bool GroundCollision()
    {
        Vector2 rightRayStart = (Vector2)rightFoot.position;
        Vector2 leftRayStart = (Vector2)leftFoot.position;

        bool rightFootOnGround = Physics2D.Raycast(rightRayStart, Vector2.down, groundCollisionDistance, groundMask);
        bool leftFootOnGround = Physics2D.Raycast(leftRayStart, Vector2.down, groundCollisionDistance, groundMask);

        return rightFootOnGround || leftFootOnGround;

    }

    public bool WallCollision()
    {
        Vector2 feetRayStart = (Vector2)feet.position;
        Vector2 handRayStart = (Vector2)hand.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask) ||
               Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool SpikeCollision()
    {   
        return Physics2D.OverlapBox((Vector2)this.transform.position, new Vector2(boxColliderSize.x + boxShellSize, boxColliderSize.y + boxShellSize), 0, spikeMask);
    }

    public bool HandsOnWall()
    {
        Vector2 handRayStart = (Vector2)hand.position;

        return Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool FeetOnWall()
    {
        Vector2 feetRayStart = (Vector2)feet.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool RightHeadSideCollision()
    {
        return Physics2D.OverlapBox((Vector2)rightHeadSide.position, cornerBoxSize, 0, groundMask);
    }

    public bool LeftHeadSideCollision()
    {
        return Physics2D.OverlapBox((Vector2)leftHeadSide.position, cornerBoxSize, 0, groundMask);

    }
    #endregion
}
