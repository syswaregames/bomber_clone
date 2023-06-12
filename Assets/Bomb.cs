using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float timeLeft = 5f;
    public int explosionSize = 3;
    public int explosionLoopsToDestroyFlames = 6;
    public GameObject explosionPrefab;
    public LayerMask flameLayerMask;
    private bool alreadyExplded = false;

    public BoxCollider2D collider;
    public void Awake()
    {
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
        IgnoreInitialOverlappedCollisions();
    }


    public void ConfigureBombAndInitiate(float timeLeft, int explosionSize)
    {
        this.timeLeft = timeLeft;
        this.explosionSize = explosionSize;
    }
    private List<Collider2D> overlappedCollidersToIgnore = new();
    void IgnoreInitialOverlappedCollisions()
    {
        Collider2D[] currentOverlappedTestedColliders = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max);
        overlappedCollidersToIgnore = currentOverlappedTestedColliders.ToList();
        foreach (var colIn in overlappedCollidersToIgnore)
        {
            Physics2D.IgnoreCollision(collider, colIn, true);
        }
    }
    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            Explode();
        }

        if (overlappedCollidersToIgnore.Count > 0)
        {
            overlappedCollidersToIgnore = overlappedCollidersToIgnore.Where(x => x != null).ToList();
            Collider2D[] currentOverlappedTestedColliders = Physics2D.OverlapAreaAll(collider.bounds.min, collider.bounds.max);
            var collidersOff = overlappedCollidersToIgnore.Except(currentOverlappedTestedColliders);

            foreach (var colOff in collidersOff)
            {
                Physics2D.IgnoreCollision(collider, colOff, false);
            }

            overlappedCollidersToIgnore = currentOverlappedTestedColliders.ToList();
        }
    }

    public void Explode()
    {
        if (alreadyExplded)
            return;

        alreadyExplded = true;
        Destroy(gameObject);
        var obj = (GameObject)Instantiate(explosionPrefab, transform.position, transform.rotation);
        Explosion explosionComponent = obj.GetComponent<Explosion>();
        explosionComponent.ConfigureExplosionAndInitiate(explosionSize, explosionLoopsToDestroyFlames);
    }

    public void OnCollisionEnter2D(Collision2D col)
    {


    }
    public void OnTriggerEnter2D(Collider2D collider)
    {

        if (flameLayerMask.Contains(collider.gameObject.layer))
        {
            Explode();
        }
    }
}
