
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fists : MonoBehaviour
{
    // Start is called before the first frame update
    SpriteRenderer spriteRenderer;
    public Sprite[] spriteArray;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void switchSprite(int spriteNum)
    {
        spriteRenderer.sprite = spriteArray[spriteNum];
    }
}
