using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour, IDamageTaker
{
    public GameObject destructionEffectPrefab;
    public LayerMask playerLayer;
    private IDamageEmitter imunityToThisEmmiter;
    public MountableController ridableAnimalGameObject;
    public float ridableInstantiationDelay = 0.5f;
    public Collider2D collider;
    public Collider2D hitboxCollider;
    public SpriteRenderer spriteRenderer;

    public DamageResponseDto TakeDamage(DamageEmissionDto emission)
    {
        if (borning)
            return null;

        if (emission.emitter == imunityToThisEmmiter)
        {
            return null;
        }
        Instantiate(destructionEffectPrefab, WorldUtility.SnapPositionToCenterOfUnit(transform.position), Quaternion.identity);
        Destroy(gameObject);

        return new();
    }

    public void AddImunityTo(IDamageEmitter emitter)
    {
        imunityToThisEmmiter = emitter;
    }
    // Start is called before the first frame update
    void Start()
    {
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
    }
    bool borning;
    public void OnTriggerEnter2D(Collider2D col)
    {
        if (borning)
            return;


    }

    public void BornNow(BomberController bomber)
    {
        if (borning)
            return;
        StartCoroutine(BornNowCo(bomber));
    }
    IEnumerator BornNowCo(BomberController bomber)
    {
        borning = true;
        Instantiate(destructionEffectPrefab, WorldUtility.SnapPositionToCenterOfUnit(transform.position), Quaternion.identity);
        spriteRenderer.enabled = false;
        collider.enabled = false;
        hitboxCollider.enabled = false;

        var mountableObj = Instantiate(ridableAnimalGameObject.gameObject, WorldUtility.SnapPositionToCenterOfUnit(transform.position), Quaternion.identity);
        var mountableComponent = mountableObj.GetComponent<MountableController>();
        mountableObj.SetActive(false);
        bomber.StartMounting(mountableComponent);

        yield return new WaitForSeconds(ridableInstantiationDelay);
        mountableObj.SetActive(true);
        mountableComponent.playable = true;
        mountableComponent.rider = bomber;
        Destroy(gameObject);

    }
}
