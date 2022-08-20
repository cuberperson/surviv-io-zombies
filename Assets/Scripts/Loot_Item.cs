using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot_Item : MonoBehaviour
{
    // Start is called before the first frame update
    int lootNum;
    int giveAmount;
    public Sprite[] item;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item[lootNum - 8];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetItem(int itemNum, int amount){
        lootNum = itemNum;
        giveAmount = amount;
    }

    public int GetLootNum(){
        return lootNum;
    }

    public int GetGiveAmount(){
        return giveAmount;
    }
}
