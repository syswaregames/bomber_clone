using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IDamageEmitter
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
    public LayerMask wallLayerMask;
    public LayerMask damageableLayerMask;

    [Tooltip("Number of animation loops to destroy this object")]
    public int maxLoopCounts = 6;
    public Animator animator;
    [ReadOnly]
    public float timeToDelete = 999f;
    public bool autoExplode;
    [Serializable]
    public class Teste
    {
        public enum EInimigo { gato, cachorro, rato, elefante }
        public List<EInimigo> listaDeEnum = new();
        public List<List<int>> lista;
    }

    public void Start()
    {
        if (autoExplode)
        {
            ConfigureExplosionAndInitiate(explosionSize, maxLoopCounts);
        }
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
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
            var hit = Physics2DExtended.Raycast(transform.position, direction, explosionSize + 0.1f, wallLayerMask);
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
                segment.GetComponent<BoxCollider2D>().size = new Vector2(0.9f, targetSize); //0.9 porque se for 1 vai colidir em bombas que vc não quer que colida
                segment.transform.position = transform.position + ((Vector3)direction.normalized * 0.5f) + ((Vector3)direction.normalized * (targetSize * 0.5f));

                DestructibleBlock destructibleBlock = hit.collider.GetComponent<DestructibleBlock>();
                if (destructibleBlock is not null)
                {
                    destructibleBlock.DestroyIt();
                }
            }
            else
            {
                var sectionSize = explosionSize - 1;
                segment.GetComponent<SpriteRenderer>().size = new Vector2(1, sectionSize);
                segment.GetComponent<BoxCollider2D>().size = new Vector2(0.9f, sectionSize);
                segment.transform.position = transform.position + ((Vector3)direction.normalized * 0.5f) + ((Vector3)direction.normalized * (sectionSize * 0.5f));
                cap.SetActive(true);
                cap.transform.position = transform.position + ((Vector3)direction.normalized) + ((Vector3)direction.normalized * (sectionSize));
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (wallLayerMask.Contains(col.gameObject.layer) || damageableLayerMask.Contains(col.gameObject.layer))
        {
            var damageableComponent = col.gameObject.GetComponent<IDamageTaker>();
            if (damageableComponent is not null)
            {
                damageableComponent.TakeDamage(new DamageEmissionDto(emitter: this, damage: 5));
            }
        }


    }
}

