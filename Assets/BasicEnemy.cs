using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour, IDamageTaker, IDamageEmitter
{
    public Animator animator;
    public LayerMask wallLayer;
    public bool flipSpriteHorizontal;
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public float speed = 1f;
    public float waitBeforeChangeDirection = 0.5f;
    public GameObject genericDeathEffectsPrefab;
    public SpriteLetters scorePrefab;
    public Egg eggPrefab;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WalkCicle());
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator WalkCicle()
    {
        Vector2 freeDirection = GetFreeDirection();

        while (freeDirection == Vector2.zero)
        {
            yield return new WaitForSeconds(1f);
            freeDirection = GetFreeDirection();
        }



        if (freeDirection == Vector2.right)
        {
            animator.SetInteger("walkDirection", (int)PlayerWalk.WalkDirection.right);
            spriteRenderer.flipX = flipSpriteHorizontal;
        }
        else if (freeDirection == Vector2.left)
        {
            animator.SetInteger("walkDirection", (int)PlayerWalk.WalkDirection.left);
            spriteRenderer.flipX = !flipSpriteHorizontal;
        }
        else if (freeDirection == Vector2.up)
        {
            animator.SetInteger("walkDirection", (int)PlayerWalk.WalkDirection.up);
        }
        else if (freeDirection == Vector2.down)
        {
            animator.SetInteger("walkDirection", (int)PlayerWalk.WalkDirection.down);
        }

        rb.velocity = freeDirection * speed;
        while (rb.velocity != Vector2.zero)
        {
            rb.velocity = freeDirection * speed;
            yield return new WaitForEndOfFrame();
        }
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(waitBeforeChangeDirection);
        StartCoroutine(WalkCicle());
    }
    public class DirectionStat
    {
        public Vector2 direction;
        public int usedTimes;

        public DirectionStat(Vector2 direction)
        {
            this.direction = direction;
        }
    }
    List<DirectionStat> directionStats = new List<DirectionStat>() {
        new DirectionStat(Vector2.right),
        new DirectionStat(Vector2.left),
        new DirectionStat(Vector2.down),
        new DirectionStat(Vector2.up)
    };

    Vector3 GetFreeDirection()
    {
        Random.InitState((int)(System.DateTime.Now.Ticks));
        directionStats.Sort((a, b) =>
        {
            if (a.usedTimes < b.usedTimes)
            {
                return -1;
            }
            else if (a.usedTimes > b.usedTimes)
            {
                return 1;
            }
            else
            {
                return Mathf.RoundToInt(Random.Range(-1.5f, 1.5f));
            }
        });
        List<DirectionStat> notTestedDirections = new List<DirectionStat>(directionStats);
        while (notTestedDirections.Count > 0)
        {
            var currentDirStats = notTestedDirections[Mathf.RoundToInt(Random.Range(-0.5f, notTestedDirections.Count - 0.51f))];
            var hit = Physics2DExtended.Raycast(WorldUtility.SnapPositionToCenterOfUnit(transform.position), currentDirStats.direction, 1f, wallLayer);
            notTestedDirections.Remove(currentDirStats);
            if (hit.collider is null)
            {
                currentDirStats.usedTimes++;
                return currentDirStats.direction;
            }
        }
        return Vector2.zero;
    }
    public DamageResponseDto TakeDamage(DamageEmissionDto emission)
    {
        Destroy(gameObject);
        Instantiate(genericDeathEffectsPrefab, transform.position, Quaternion.identity);
        var eggObj = (GameObject)Instantiate(eggPrefab.gameObject, transform.position, Quaternion.identity);
        var eggComponent = eggObj.GetComponent<Egg>();
        eggComponent.AddImunityTo(emission.emitter);

        var scoreObj = (GameObject)Instantiate(scorePrefab.gameObject, transform.position, Quaternion.identity);
        var scoreObjComponent = scoreObj.GetComponent<SpriteLetters>();
        scoreObjComponent.Initiate(100);
        throw new System.NotImplementedException();
    }
}
