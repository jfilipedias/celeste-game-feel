using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    #region Atributes
    [Header("Check Collision")]
    [SerializeField] private float groundCollisionDistance;
    [SerializeField] private float wallCollisionDistance;
    [SerializeField] private float sphereGizmoSize;
    [SerializeField] private float boxShellSize;

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

    private float facingDirection = Vector2.right.x;

    private Vector2 boxColliderSize;

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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(boxColliderSize.x + boxShellSize, boxColliderSize.y + boxShellSize));

        // Foot
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

    public bool WallCollistion(float facingDirection)
    {
        Vector2 feetRayStart = (Vector2)feet.position;
        Vector2 handRayStart = (Vector2)hand.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask) ||
               Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool SpikeCollision()
    {
        bool collideOnSpike = Physics2D.OverlapBox((Vector2)this.transform.position, new Vector2(boxColliderSize.x + boxShellSize, boxColliderSize.y + boxShellSize), 0, spikeMask);

        return collideOnSpike;
    }

    public bool HandsOnWall(float facingDirection)
    {
        Vector2 handRayStart = (Vector2)hand.position;

        return Physics2D.Raycast(handRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }

    public bool FeetOnWall(float facingDirection)
    {
        Vector2 feetRayStart = (Vector2)feet.position;

        return Physics2D.Raycast(feetRayStart, new Vector2(facingDirection, 0), wallCollisionDistance, groundMask);
    }
    #endregion
}
