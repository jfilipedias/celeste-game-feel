using CelesteGameFeel.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    #region Atributes
    [Header("Check Collision")]
    [SerializeField] private float collisionDistance = 0.02f;
    [SerializeField] private float sphereGizmoSize = 0.1f;
    [SerializeField] private float shellBoxSize = 0.02f;
    [SerializeField] private float cornerDistance = 0.38f;
    [SerializeField] private Vector2 headBoxSize = new Vector2(0.54f, 0.05f);
    [SerializeField] private Vector2 cornerBoxSize = new Vector2(0.22f, 0.05f);
    [SerializeField] private Vector2 feetBoxSize = new Vector2(0.98f, 0.05f);

    [Space]
    [Header("Colision Mask")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask spikeMask;

    [Space]
    [Header("Collision Transform")]
    [SerializeField] private Transform head;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform feet;

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
        // Head
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(new Vector2(head.position.x + cornerDistance, head.position.y), cornerBoxSize);     // Right Corner
        Gizmos.DrawWireCube(new Vector2(head.position.x - cornerDistance, head.position.y), cornerBoxSize);     // Left Corner
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(head.position, headBoxSize);

        // Hand
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(hand.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(hand.position, (hand.position + new Vector3(((boxColliderSize.x / 2) + collisionDistance) * facingDirection, 0, 0)));

        // Feet
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(feet.position, sphereGizmoSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(feet.position, (feet.position + new Vector3(((boxColliderSize.x / 2) + collisionDistance) * facingDirection, 0, 0)));

        // Foot
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(feet.position, feetBoxSize);

        // Collision Shell
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(this.transform.position, new Vector2(boxColliderSize.x + shellBoxSize, boxColliderSize.y + shellBoxSize));
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
        Collider2D hitSpike = Physics2D.OverlapBox((Vector2)this.transform.position, new Vector2(boxColliderSize.x + shellBoxSize, boxColliderSize.y + shellBoxSize), 0, spikeMask);

        return hitSpike != null;
    }

    public bool WallCollision()
    {
        bool feetOnWall = HandsOnWall();
        bool handOnWall = FeetOnWall();

        return feetOnWall || handOnWall;                  
    }

    public bool RightCornerOnWall()
    {
        Vector2 rightConerPosition = new Vector2(head.position.x + cornerDistance, head.position.y);
        Collider2D hitRightCorner = Physics2D.OverlapBox(rightConerPosition, cornerBoxSize, 0, groundMask);
        
        return hitRightCorner != null;
    }

    public bool LeftCornerOnWall()
    {
        Vector2 leftConerPosition = new Vector2(head.position.x - cornerDistance, head.position.y);
        Collider2D hitLeftCorner = Physics2D.OverlapBox(leftConerPosition, cornerBoxSize, 0, groundMask);
        
        return hitLeftCorner != null;
    }

    public bool HeadOnWall()
    {
        Collider2D hitHead = Physics2D.OverlapBox((Vector2)head.position, headBoxSize, 0, groundMask);
        
        return hitHead != null;
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
    #endregion
}
