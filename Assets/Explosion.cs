using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{

    public int explosionSize = 3;
    public GameObject centerPrefab;
    public GameObject capRight;
    public GameObject capLeft;
    public GameObject capUp;
    public GameObject capDown;

    public GameObject segmentRight;
    public GameObject segmentLeft;
    public GameObject segmentUp;
    public GameObject segmentDown;
    public LayerMask layerMask;
    [Tooltip("Number of animation loops to destroy this object")]
    public int maxLoopCounts = 6;
    public Animator animator;
    [ReadOnly]
    public float timeToDelete = 999f;
    public bool autoExplode;
    public class Teste
    {
        public Dictionary<string, string> dicionario { get; set; }
    }

    public void Start()
    {
        if (autoExplode)
        {
            ConfigureExplosionAndInitiate(explosionSize, maxLoopCounts);
        }
        WorldUtility.SnapPositionToCenterOfUnit(transform.position);
    }

    public void Update()
    {
        timeToDelete -= Time.deltaTime;
        if (timeToDelete <= 0f)
        {
            Destroy(gameObject);
        }
    }


    public void ConfigureExplosionAndInitiate(int explosionSize, int maxLoopCounts)
    {
        this.maxLoopCounts = maxLoopCounts;
        this.explosionSize = explosionSize;

        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
        timeToDelete = animationDuration * maxLoopCounts;

        this.explosionSize = explosionSize;
        HandleDirection(segmentRight, capRight, Vector2.right);
        HandleDirection(segmentLeft, capLeft, Vector2.left);
        HandleDirection(segmentUp, capUp, Vector2.up);
        HandleDirection(segmentDown, capDown, Vector2.down);

        void HandleDirection(GameObject segment, GameObject cap, Vector2 direction)
        {
            var hit = Physics2DExtended.Raycast(transform.position, direction, 20, layerMask);
            if (hit.collider is not null)
            {
                var targetSize = Mathf.RoundToInt((hit.point - (Vector2)transform.position).magnitude - 0.5f);
                if (targetSize > explosionSize)
                {
                    targetSize = explosionSize;
                    cap.SetActive(true);
                    cap.transform.position = transform.position + ((Vector3)direction.normalized) + ((Vector3)direction.normalized * (targetSize));
                }
                else
                {
                    cap.SetActive(false);
                }
                segment.GetComponent<SpriteRenderer>().size = new Vector2(1, targetSize);
                segment.GetComponent<BoxCollider2D>().size = new Vector2(1, targetSize);
                segment.transform.position = transform.position + ((Vector3)direction.normalized * 0.5f) + ((Vector3)direction.normalized * (targetSize * 0.5f));
            }
        }
    }
}

