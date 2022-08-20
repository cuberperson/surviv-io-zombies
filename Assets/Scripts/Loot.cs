using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    int lootNum;
    int giveAmount;
    Color itemColor;
    public Sprite[] item;
    SpriteRenderer spriteRenderer;
    float despawnTimeStamp = 0f;
    float despawnTime = 30f;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item[lootNum];
        if (gameObject.tag == "Weapon")
        {   
            transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_Color", itemColor);
        }
        despawnTimeStamp = Time.time + despawnTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= despawnTimeStamp)
        {
            Destroy(gameObject);
        }
    }

    public void SetItem(int itemNum, int amount, Color iconColor)
    {
        lootNum = itemNum;
        giveAmount = amount;
        itemColor = iconColor;
    }

    public int GetLootNum()
    {
        return lootNum;
    }

    public int GetGiveAmount()
    {
        return giveAmount;
    }
}
