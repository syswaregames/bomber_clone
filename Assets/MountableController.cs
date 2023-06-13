using UnityEngine;
using System.Linq;

public class MountableController : BomberController, IMountable
{

    public Vector2 mountOffset;
    public BomberController rider;
    private SpriteRenderer[] riderSpriteRenderers;

    void LateUpdate()
    {
        if (rider is not null)
        {
            if (riderSpriteRenderers is null)
            {
                riderSpriteRenderers = rider.GetComponentsInChildren<SpriteRenderer>();
            }
            foreach (var item in riderSpriteRenderers)
            {
                item.sortingOrder = spriteRenderer.sortingOrder + 1;
            }

            rider.SetWalkDirectionFromExternalMounting(walkDirection, invertFlip ? !spriteRenderer.flipX : spriteRenderer.flipX);
        }
    }

}
