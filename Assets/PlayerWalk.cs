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
    public float currentXSpeed;
    [ReadOnly]
    public float currentYSpeed;

    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
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
    void HandleWalk()
    {
        var xAxis = Input.GetAxisRaw("Horizontal");
        var yAxis = Input.GetAxisRaw("Vertical");

        var targetXSpeed = xAxis * maxSpeed;
        var targetYSpeed = yAxis * maxSpeed;
        currentXSpeed = GetNewSpeed(targetXSpeed, rb.velocity.x, acceleration, deceleration);
        currentYSpeed = GetNewSpeed(targetYSpeed, rb.velocity.y, acceleration, deceleration);
        if (xAxis > 0f)
            print(currentXSpeed);
        rb.velocity = new Vector2(currentXSpeed, currentYSpeed);

        if (xAxis != 0 || yAxis != 0)
        {
            animator.SetBool("walking", true);
        }
        else
        {
            animator.SetBool("walking", false);
        }

        if (xAxis > 0f)
        {
            SetFacingToRight(true);
        }
        else if (xAxis < 0f)
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
}
