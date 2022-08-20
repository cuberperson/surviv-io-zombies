using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot_Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    
    int lootNum;
    public Sprite[] weapon;
    SpriteRenderer spriteRenderer;
    Color weaponColor;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weapon[lootNum];
        transform.GetChild(0).GetComponent<SpriteRenderer>().material.SetColor("_Color", weaponColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeapon(int weaponNum, Color iconColor){
        lootNum = weaponNum;
        weaponColor = iconColor;
    }

    public int GetWeapon()
    {
        return lootNum;
    }
}
