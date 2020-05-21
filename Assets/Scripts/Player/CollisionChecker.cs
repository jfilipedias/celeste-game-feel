using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    #region Atributes
    [Header("Check Collision")]
    [SerializeField] private float collisionDistance = 0.02f;
    [SerializeField] private float sphereGizmoSize = 0.1f;
    [SerializeField] private float boxShellSize = 0.02f;
    [SerializeField] private Vector2 feetBoxSize = new Vector2(0.98f, 0.05f);
    [SerializeField] private Vector2 headCornerBoxSize = new Vector2(0.28f, 0.05f);
    [SerializeField] private Vector2 headCenterBoxSize = new Vector2(0.4f, 0.05f);

    [Space]
    [Header("Colision Mask")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask spikeMask;

    [Space]
    [Header("Collision Transform")]
    [SerializeField] private Transform hand;
    [SerializeField] private Transform feet;
    [SerializeField] private Transform headRightSide;
    [SerializeField] private Transform headLeftSide;
    [SerializeField] private Transform headCenter;

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
        // Feet
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(feet.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(feet.position, (feet.position + new Vector3(((boxColliderSize.x / 2) + collisionDistance) * facingDirection, 0, 0)));

        // Hand
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(hand.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(hand.position, (hand.position + new Vector3(((boxColliderSize.x / 2) + collisionDistance) * facingDirection, 0, 0)));

        // Collision Shell
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(boxColliderSize.x + boxShellSize, boxColliderSize.y + boxShellSize));

        // Head Sides
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(headRightSide.position, headCornerBoxSize);
        Gizmos.DrawWireCube(headLeftSide.position, headCornerBoxSize);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(headCenter.position, headCenterBoxSize);

        // Foot
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(feet.position, feetBoxSize);
    }
    #endregion

    #region Collision Methods
    public bool GroundCollision()
    {
        Collider2D feetOnGround = Physics2D.OverlapBox((Vector2)feet.position, feetBoxSize, 0, groundMask);

        return feetOnGround != null;
    }

    public bool SpikeCollision()
    {
        Collider2D hitSpike = Physics2D.OverlapBox((Vector2)this.transform.position, new Vector2(boxColliderSize.x + boxShellSize, boxColliderSize.y + boxShellSize), 0, spikeMask);

        return hitSpike != null;
    }

    public bool WallCollision()
    {
        bool feetOnWall = HandsOnWall();
        bool handOnWall = FeetOnWall();

        return feetOnWall || handOnWall;                  
    }

    public bool HandsOnWall()
    {
        RaycastHit2D handOnWall = Physics2D.Raycast((Vector2)hand.position, new Vector2(facingDirection, 0), (boxColliderSize.x / 2) + collisionDistance, groundMask);

        return handOnWall.collider != null;
    }

    public bool FeetOnWall()
    {
        RaycastHit2D feetOnWall = Physics2D.Raycast((Vector2)feet.position, new Vector2(facingDirection, 0), (boxColliderSize.x / 2) + collisionDistance, groundMask);

        return feetOnWall.collider != null;    
    }

    public bool RightHeadSideCollision()
    {
        Collider2D hitRightCorner = Physics2D.OverlapBox((Vector2)headRightSide.position, headCornerBoxSize, 0, groundMask);

        return hitRightCorner != null;
    }

    public bool LeftHeadSideCollision()
    {
        Collider2D hitLeftCorner = Physics2D.OverlapBox((Vector2)headLeftSide.position, headCornerBoxSize, 0, groundMask);
        
        return hitLeftCorner != null;
    }

    public bool HeadCenterCollision()
    {
        Collider2D hitHeadCenter = Physics2D.OverlapBox((Vector2)headCenter.position, headCenterBoxSize, 0, groundMask);
        
        return hitHeadCenter != null;
    }
    #endregion
}
