using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float timeLeft = 5f;
    public int explosionSize = 3;
    public int explosionLoopsToDestroyFlames = 6;
    public GameObject explosionPrefab;
    public LayerMask flameLayerMask;
    private bool alreadyExplded = false;
    public void Start()
    {
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
    }
    public void ConfigureBombAndInitiate(float timeLeft, int explosionSize)
    {
        this.timeLeft = timeLeft;
        this.explosionSize = explosionSize;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            Explode();
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

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (flameLayerMask.Contains(collider.gameObject.layer))
        {
            Explode();
        }
    }
}
