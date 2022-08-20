using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot_Ammo : MonoBehaviour
{
    // Start is called before the first frame update
    int lootNum;
    int giveAmount;
    public Sprite[] ammo;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = ammo[lootNum];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAmmo(int type, int amount){
        lootNum = type;
        giveAmount = amount;
    }

    public int GetLootNum(){
        return lootNum;
    }

    public int GetGiveAmount(){
        return giveAmount;
    }

}
