using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBlock : MonoBehaviour
{
    public LayerMask explosionLayer;
    public Animator animator;
#if UNITY_EDITOR

    [TextArea]
    public string informative = "Destruction animation state should be named as 'destructing' and set to not loop.";
#endif
    // Start is called before the first frame update
    void Start()
    {
        transform.position = WorldUtility.SnapPositionToCenterOfUnit(transform.position);
    }

    public void DestroyIt()
    {
        if (isDestroying)
            return;

        isDestroying = true;
        StartCoroutine(DestroyItCo());
    }
    bool isDestroying;
    IEnumerator DestroyItCo()
    {
        animator.CrossFade("destructing", 0, 0, 0);
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
