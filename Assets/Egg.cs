using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour, IDamageTaker
{
    public GameObject destructionEffectPrefab;
    private IDamageEmitter imunityToThisEmmiter;
    public DamageResponseDto TakeDamage(DamageEmissionDto emission)
    {
        if(emission.emitter == imunityToThisEmmiter) {
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
}
