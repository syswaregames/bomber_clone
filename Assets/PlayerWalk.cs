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
    [Range(0f, 1f)]
    public float toleranceToAvoidStuckness = 0.7f;
    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public CircleCollider2D collider;

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
    Vector2 deviateDirectionToAvoidStuck3(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return Vector2.zero;
        }
        if (direction.y == 0f)
        {
            var origin = (Vector2)collider.bounds.center;
            var centerHit = Physics2DExtended.Raycast(origin, direction, 0.55f, wallLayermask);
            var upHit = Physics2DExtended.Raycast(origin + (Vector2.up * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);
            var downHit = Physics2DExtended.Raycast(origin + (Vector2.down * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);

            if (upHit.collider is not null || downHit.collider is not null)
            {
                if (upHit.collider is null)
                {
                    return new Vector2(direction.x, 1f);
                }
                else if (downHit.collider is null)
                {
                    return new Vector2(direction.x, -1f);
                }
            }
        }

        if (direction.x == 0f)
        {
            var origin = (Vector2)collider.bounds.center;
            var centerHit = Physics2DExtended.Raycast(origin, direction, 0.55f, wallLayermask);
            var leftHit = Physics2DExtended.Raycast(origin + (Vector2.left * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);
            var rightHit = Physics2DExtended.Raycast(origin + (Vector2.right * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);
            if (rightHit.collider is not null || leftHit.collider is not null)
            {
                if (rightHit.collider is null)
                {
                    return new Vector2(1f, direction.y);
                }
                else if (leftHit.collider is null)
                {
                    return new Vector2(-1f, direction.y);
                }
            }
        }
        return direction;
    }
    Vector2 deviateDirectionToAvoidStuck2(Vector2 direction)
    {
        return direction;
        RaycastHit2D? facingRightHit = null;
        RaycastHit2D? facingLeftHit = null;
        RaycastHit2D? facingTopHit = null;
        RaycastHit2D? facingDownHit = null;

        if (direction.x > 0)
        {
            facingRightHit = GetFacingHit(Vector2.right);
            if (facingRightHit.HasValue)
            {
                direction.x = 0f;
            }
        }
        else if (direction.x < 0)
        {
            facingLeftHit = facingRightHit = GetFacingHit(Vector2.left);
            if (facingLeftHit.HasValue)
            {
                direction.x = 0f;
            }
        }

        if (direction.y > 0)
        {
            facingTopHit = GetFacingHit(Vector2.up);
            if (facingTopHit.HasValue)
            {
                direction.y = 0f;
            }
        }
        else if (direction.y < 0)
        {
            facingDownHit = facingRightHit = GetFacingHit(Vector2.down);
            if (facingDownHit.HasValue)
            {
                direction.y = 0f;
            }
        }
        return direction;
        RaycastHit2D? GetFacingHit(Vector2 direction)
        {
            var origin = (Vector2)collider.bounds.center;
            var perpendicular = Vector2.Perpendicular(direction);
            var centerHit = Physics2DExtended.Raycast(origin, direction, 0.55f, wallLayermask);
            var leftHit = Physics2DExtended.Raycast(origin - (perpendicular * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);
            var rightHit = Physics2DExtended.Raycast(origin + (perpendicular * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);
            return centerHit.collider is not null ? centerHit : leftHit.collider is not null ? leftHit : rightHit.collider is not null ? rightHit : null;
        }

    }
    Vector2 deviateDirectionToAvoidStuck(Vector2 direction)
    {

        var origin = (Vector2)collider.bounds.center;
        var perpendicular = Vector2.Perpendicular(direction);
        var centerHit = Physics2DExtended.Raycast(origin, direction, 0.55f, wallLayermask);
        var leftHit = Physics2DExtended.Raycast(origin - (perpendicular * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);
        var rightHit = Physics2DExtended.Raycast(origin + (perpendicular * (collider.radius * toleranceToAvoidStuckness)), direction, 0.55f, wallLayermask);

        var facingWallHit = GetFacingWallHit();
        if (facingWallHit.HasValue)
        {

            var leftDirection = origin + (perpendicular * -0.6f) - origin;
            var rightDirection = origin + (perpendicular * 0.6f) - origin;
            var leftAngleDifference = Vector2.Angle(leftDirection, direction);
            var rightAngleDifference = Vector2.Angle(leftDirection, direction);

            if (leftAngleDifference < rightAngleDifference)
            {
                return leftDirection.normalized * direction.magnitude;
            }
            else if (leftAngleDifference > rightAngleDifference)
            {
                return rightDirection.normalized * direction.magnitude;
            }

            if (leftHit.collider is null)
            {
                var newDirection = origin + (perpendicular * -0.6f) - origin;
                var newDirectionWithProportionalMagnitude = newDirection.normalized * direction.magnitude;
                return newDirectionWithProportionalMagnitude;
            }
            else if (rightHit.collider is null)
            {
                var newDirection = origin + (perpendicular * 0.6f) - origin;
                var newDirectionWithProportionalMagnitude = newDirection.normalized * direction.magnitude;
                return newDirectionWithProportionalMagnitude;
            }
        }

        RaycastHit2D? GetFacingWallHit()
        {
            return centerHit.collider is not null ? centerHit : leftHit.collider is not null ? leftHit : rightHit.collider is not null ? rightHit : null;
        }

        return direction;
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
            if (result.normal * -1 == testDirection && (result.normal.x == 0f || result.normal.y == 0f))
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

    Vector2 clampAxisTo8Directions(Vector2 axis)
    {
        if (axis.magnitude < 0.1f)
            return Vector2.zero;

        var newAxis = axis.normalized;
        var angle = Vector2.SignedAngle(Vector2.up, newAxis);
        print("angle" + angle);
        var dir = Mathf.RoundToInt(angle / 45f);
        print("dir" + dir);

        switch (dir)
        {
            case 0: return Vector2.up;
            case -1: return Vector2.up + Vector2.right;
            case -2: return Vector2.right;
            case -3: return Vector2.right + Vector2.down;
            case 4: return Vector2.down;
            case 3: return Vector2.down + Vector2.left;
            case 2: return Vector2.left;
            case 1: return Vector2.left + Vector2.up;
            default: return Vector2.zero;
        }
    }
    void HandleWalk()
    {
        calculateWalkDirection();
        var axis = clampAxisTo8Directions(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        axis = deviateDirectionToAvoidStuck3(axis);

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
        foreach (var contact in col.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 0.33f, Color.cyan);
        }
    }
    public void OnCollisionStay2D(Collision2D col)
    {
        // teste(col);
        foreach (var contact in col.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 0.33f, Color.cyan);
        }

    }


}
