using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEmissionDto
{
    public IDamageEmitter emitter;
    public float damage;

    public DamageEmissionDto(IDamageEmitter emitter, float damage)
    {
        this.emitter = emitter;
        this.damage = damage;
    }
}
