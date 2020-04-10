using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    [SerializeField] private float minGroundNormalY = 0.65f;
    [SerializeField] private float gravityModifier = 1f;

    protected bool grounded;
    protected bool movingVerticaly; 
    protected Vector2 groundNormal;
    protected Vector2 velocity;
    protected Rigidbody2D rb2D;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;


    private void OnEnable()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }


    private void FixedUpdate()
    {
        CalculateGravity();
        CalculateMovement();
    }


    private void CalculateGravity()
    {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        grounded = false;
    }


    private void CalculateMovement()
    {
        movingVerticaly = true;

        Vector2 verticalMovement = CalculateVertivalMovement();
      //Vector2 horizontalMovement = CalculateHorizontalMovement();
        
        rb2D.position += verticalMovement;
    }


    private Vector2 CalculateVertivalMovement()
    {
        Vector2 deltaPosition = velocity * Time.deltaTime;
        Vector2 vMovement = Vector2.up * deltaPosition.y;

        CheckGroundCollision(vMovement);

        return vMovement;

    }


    private void CalculateHorizontalMovement()
    {

    }


    private void CheckGroundCollision(Vector2 direction)
    {
        float distance = direction.magnitude;

        if (distance > minMoveDistance)
        {
            int count = rb2D.Cast(direction, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();

            foreach (RaycastHit2D hit in hitBuffer)
            {
                hitBufferList.Add(hit);
            }

            foreach (RaycastHit2D hit in hitBufferList)
            {
                Vector2 currentNormal = hit.normal;
                
                if(currentNormal.y > minGroundNormalY)
                {
                    grounded = true;

                    if (movingVerticaly)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                   
                if(projection < 0)
                {
                    velocity -= projection * currentNormal;     
                }

                float modifiedDistance = hit.distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
    }
}
