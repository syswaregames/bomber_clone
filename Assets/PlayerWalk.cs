using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

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
    [Header("Bomb")]
    public GameObject bombPrefab;
    public float bombTimeLeft = 3f;
    public int bombSize = 4;

    private List<Collider2D> throwedBombColliders = new();
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(WorldUtility.SnapPositionToCenterOfUnit(collider.bounds.center), collider.radius);
            if (colliders.Where(x => x.GetComponent<Bomb>() is not null).Count() == 0)
            {
                var bomb = (GameObject)Instantiate(bombPrefab, collider.bounds.center, Quaternion.identity);
                bomb.GetComponent<Bomb>().ConfigureBombAndInitiate(bombTimeLeft, bombSize);
                var bombCollider = bomb.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(collider, bombCollider);
                throwedBombColliders.Add(bombCollider);
            }

        }
        if (throwedBombColliders.Count > 0)
        {
            throwedBombColliders = throwedBombColliders.Where(x => x != null).ToList();
            var collidersToRemove = new List<Collider2D>();
            for (int i = 0; i < throwedBombColliders.Count; i++)
            {
                Collider2D bombCollider = throwedBombColliders[i];
                Collider2D[] colliders = Physics2D.OverlapCircleAll(collider.transform.position, collider.radius);
                if (bombCollider is not null && !colliders.Contains(bombCollider))
                {
                    Physics2D.IgnoreCollision(collider, bombCollider, false);
                    collidersToRemove.Add(bombCollider);
                }
            }
            throwedBombColliders = throwedBombColliders.Except(collidersToRemove).ToList();
        }
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
    Vector2 deviateDirectionToAvoidStuck(Vector2 direction)
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


    Vector2 clampAxisTo8Directions(Vector2 axis)
    {
        if (axis.magnitude < 0.1f)
            return Vector2.zero;

        var newAxis = axis.normalized;
        var angle = Vector2.SignedAngle(Vector2.up, newAxis);
       // print("angle" + angle);
        var dir = Mathf.RoundToInt(angle / 45f);
       // print("dir" + dir);

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
        var axis = clampAxisTo8Directions(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")));
        axis = deviateDirectionToAvoidStuck(axis);
        calculateWalkDirection(axis);


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
    }

    void calculateWalkDirection(Vector2 direction)
    {

        var x = rb.velocity.x;
        var y = rb.velocity.y;

        if (x == 0f && y == 0f && direction != Vector2.zero)
        {
            x = direction.x;
            y = direction.y;
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
    public void OnTriggerExit2D(Collider2D other)
    {
        if (throwedBombColliders.Contains(other))
        {
            Physics2D.IgnoreCollision(collider, other, false);
            throwedBombColliders.Remove(collider);
        }
    }


}
