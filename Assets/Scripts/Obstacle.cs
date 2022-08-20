using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    // Start is called before the first frame update
    public int startHealth;
    public bool shrinkSize;
    public bool canLoot;
    float health;
    public GameObject remains;
    public GameObject weapon;
    public GameObject ammo;
    public GameObject item;
    public int[] lootTable;
    public int[] lootProb;
    string[] weapons = {"M9", "G18C", "MAC-10", "MP5", "OT-38", "AK47", "PKP Pecheneg", "Flare Gun", "Bandage", "Med Kit", "Soda", "Pills", "Vest", "Helmet", "2x Scope", "Yellow Ammo", "Blue Ammo", "Flare Ammo"};
    int[] itemAmount = {25, 26, 48, 45, 10, 45, 175, 1, 5, 1, 1, 1, 1, 1, 1};
    void Start()
    {
        health = startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage){
        health -= damage;
        if (health <= 0){
            Instantiate(remains, transform.position, Quaternion.identity);
            if (canLoot){
                int lootNum = chooseLoot();
                GiveLoot(lootNum);
            }
            Destroy(gameObject);
        } else if (shrinkSize){
            transform.localScale -= new Vector3(0.25f * (damage/startHealth), 0.25f * (damage/startHealth), 0);
        }
    }

    void GiveLoot(int lootNum)
    {
        if (lootNum <= 3)
        {
            GameObject yellowWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
            Color yellowAmmoColor = new Color(0.9490196f, 0.6470588f, 0f, 1f);
            yellowWeapon.GetComponent<Loot>().SetItem(lootNum, 1, yellowAmmoColor);
            for (int i = 0; i < 2; i++)
            {
                Vector3 ammoPos = new Vector3(transform.position.x - 1.6f + i * 3.2f, transform.position.y - 1.3f, transform.position.z);
                GameObject yellowAmmo = Instantiate(ammo, ammoPos, Quaternion.identity);
                yellowAmmo.GetComponent<Loot>().SetItem(15, itemAmount[lootNum], Color.clear);
            }
        }
        else if (lootNum <= 6)
        {
            GameObject blueWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
            Color blueAmmoColor = new Color(0f, 0.3803922f, 0.9490197f, 1f);
            blueWeapon.GetComponent<Loot>().SetItem(lootNum, 1, blueAmmoColor);
            for (int i = 0; i < 2; i++)
            {
                Vector3 ammoPos = new Vector3(transform.position.x - 1.6f + i * 3.2f, transform.position.y - 1.3f, transform.position.z);
                GameObject blueAmmo = Instantiate(ammo, ammoPos, Quaternion.identity);
                blueAmmo.GetComponent<Loot>().SetItem(16, itemAmount[lootNum], Color.clear);
            }
        }
        else if (lootNum == 7)
        {
            GameObject flareGun = Instantiate(weapon, transform.position, Quaternion.identity);
            Color flareAmmoColor = new Color(0.8313726f, 0.2745098f, 0f, 1f);
            flareGun.GetComponent<Loot>().SetItem(lootNum, 1, flareAmmoColor);

            Vector3 ammoPos = new Vector3(transform.position.x - 1.6f, transform.position.y - 1.3f, transform.position.z);
            GameObject flareAmmo = Instantiate(ammo, ammoPos, Quaternion.identity);
            flareAmmo.GetComponent<Loot>().SetItem(17, itemAmount[lootNum], Color.clear);
        }
        else if (lootNum >= 15)
        {
            GameObject itemIcon = Instantiate(ammo, transform.position, Quaternion.identity);
            itemIcon.GetComponent<Loot>().SetItem(lootNum, itemAmount[lootNum], Color.clear);
        }
        else
        {
            GameObject itemIcon = Instantiate(item, transform.position, Quaternion.identity);
            itemIcon.GetComponent<Loot>().SetItem(lootNum, itemAmount[lootNum], Color.clear);
        }
    }
    int chooseLoot(){
        float lootNum = Random.Range(1, 101);
        for (int i = 0; i < lootProb.Length; i++){
            if (lootNum > lootProb[i]){
                print(lootTable[i]);
                return lootTable[i];
            }
        }
        print("failed");
        return 0;
    }
}
