using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLetters : MonoBehaviour
{
    public List<SpriteRenderer> letterSpriteRenderers;
    public List<Sprite> numberSprites;
    public int number;
    public bool initiateOnStart = false;
    // Start is called before the first frame update
    void Start()
    {
        if (initiateOnStart)
            Initiate(number);
    }

    public void Initiate(int number)
    {
        var letters = number.ToString();
        letterSpriteRenderers.ForEach(x => x.sprite = null);

        for (int i = 0; i < letters.Length; i++)
        {
            char char_ = letters[i];
            var spriteIndex = int.Parse(char_.ToString());
            var sprite = numberSprites[spriteIndex];
            letterSpriteRenderers[i].sprite = sprite;
        }
        var initialBound = letterSpriteRenderers[0].bounds.min.x;
        var finalBound = letterSpriteRenderers[letters.Length - 1].bounds.max.x;
        var middleBound = (initialBound + finalBound) * 0.5f;
        var offset = middleBound - transform.position.x;
        foreach (var renderer in letterSpriteRenderers)
        {
            renderer.transform.position += Vector3.left * offset;
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
