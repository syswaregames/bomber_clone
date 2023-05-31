using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{

    [Header("Walk")]
    public float maxSpeed = 10;
    public float acceleration = 10;
    public float deceleration = 10;
    [ReadOnly]
    public bool facingRight = true;
    [ReadOnly]
    public WalkDirection walkDirection;
    public enum WalkDirection { down, up, left, right }
    [ReadOnly]
    public float currentXSpeed;
    [ReadOnly]
    public float currentYSpeed;
    [ReadOnly]
    public float targetXSpeed;
    [ReadOnly]
    public float targetYSpeed;
    public LayerMask wallLayermask;
    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {


    }

    void FixedUpdate()
    {

        HandleWalk();
        setRbSpeedForAnimator();
    }

    void setRbSpeedForAnimator()
    {
        animator.SetFloat("xVelocity", rb.velocity.x);
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    Vector2 deviateDirection(Vector2 testDirection)
    {

        var result = Physics2D.Raycast(collider.bounds.center, testDirection, 0.55f, wallLayermask);
        Debug.DrawRay(collider.bounds.center, testDirection * 0.55f, Color.red);
        if (result.collider is null)
        {
            var perpendicular = Vector2.Perpendicular(testDirection);
            result = Physics2D.Raycast((Vector2)collider.bounds.center + (perpendicular * 0.2f), testDirection, 0.55f, wallLayermask);
            Debug.DrawRay((Vector2)collider.bounds.center + (perpendicular * 0.2f), testDirection * 0.55f, Color.red);

            if (result.collider is null)
            {
                perpendicular = perpendicular * -1;
                result = Physics2D.Raycast((Vector2)collider.bounds.center + (perpendicular * 0.2f), testDirection, 0.55f, wallLayermask);
                Debug.DrawRay((Vector2)collider.bounds.center + (perpendicular * 0.2f), testDirection * 0.55f, Color.red);
            }
        }
        if (result.collider is not null)
        {
            if (result.normal * -1 == testDirection && (result.normal.x==0f || result.normal.y==0f))
            {
                return testDirection;
            }

            var perpendicular = Vector2.Perpendicular(result.normal);
            print("Eita" + result.normal);
            print("Perp" + Vector2.Perpendicular(result.normal));
            Vector3 direcaoDesvio = Vector3.ProjectOnPlane(testDirection, result.normal).normalized;
            print("Perp" + direcaoDesvio);
            return direcaoDesvio;

        }
        else
        {

        }
        return testDirection;
    }
    void HandleWalk()
    {
        calculateWalkDirection();
        var axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        axis = deviateDirection(axis);

        targetXSpeed = axis.x * maxSpeed;
        targetYSpeed = axis.y * maxSpeed;



        currentXSpeed = targetXSpeed;//GetNewSpeed(targetXSpeed, rb.velocity.x, acceleration, deceleration);
        currentYSpeed = targetYSpeed; // GetNewSpeed(targetYSpeed, rb.velocity.y, acceleration, deceleration);

        rb.velocity = new Vector2(currentXSpeed, currentYSpeed);

        if (targetXSpeed != 0 || targetYSpeed != 0)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }


        //calculateWalkDirection();
    }
    
    void calculateWalkDirection()
    {

        var x = rb.velocity.x;
        var y = rb.velocity.y;
        if (x == 0f && y == 0f)
        {
            return;
        }
        if (Mathf.Abs(x) == Mathf.Abs(y))
        {
            return;
        }



        WalkDirection newWalkDirection = walkDirection;
        var isHorizontalGreater = Mathf.Abs(x) > Mathf.Abs(y);

        if (isHorizontalGreater)
        {
            if (x > 0f)
            {
                newWalkDirection = WalkDirection.right;
            }
            else if (x < 0f)
            {
                newWalkDirection = WalkDirection.left;
            }
        }
        else
        {
            if (y > 0f)
            {
                newWalkDirection = WalkDirection.up;
            }
            else if (y < 0f)
            {
                newWalkDirection = WalkDirection.down;
            }
        }

        if (newWalkDirection != walkDirection)
        {
            animator.SetInteger("walkDirection", (int)newWalkDirection);
        }
        walkDirection = newWalkDirection;

        if (x > 0f)
        {
            SetFacingToRight(true);
        }
        else if (x < 0f)
        {
            SetFacingToRight(false);
        }
    }
    void SetFacingToRight(bool toRight)
    {
        spriteRenderer.flipX = !toRight;
        facingRight = toRight;
    }

    private float GetNewSpeed(float targetSpeed, float currentSpeed, float acceleration, float deceleration)
    {
        if (targetSpeed != 0f) /* Acceleration */
        {
            currentSpeed += (targetSpeed * acceleration * Time.fixedDeltaTime);
            if (currentSpeed > maxSpeed || currentSpeed < -maxSpeed)
            {
                currentSpeed = targetSpeed;
            }
        }
        else /* Deceleration */
        {
            if (currentSpeed == 0f)
            {
                /* do nothing */
            }
            else if (currentSpeed > 0f)
            {
                currentSpeed -= deceleration * Time.fixedDeltaTime;
                if (currentSpeed < 0f)
                {
                    currentSpeed = 0f;
                }
            }
            else
            {
                currentSpeed += deceleration * Time.fixedDeltaTime;
                if (currentSpeed > 0f)
                {
                    currentSpeed = 0f;
                }
            }
        }
        return currentSpeed;
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        // print(rb.velocity);
        // teste(col);

    }
    public void OnCollisionStay2D(Collision2D col)
    {
        // teste(col);
        //Contornate(col);

    }


}
