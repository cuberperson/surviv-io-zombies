
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb2d;
    CircleCollider2D circleCollider2D;
    SpriteRenderer spriteRenderer;
    public LayerMask obstacleLayer;
    public LayerMask itemLayer;
    public GameObject bullet;
    public GameObject weapon;
    float playerAngle;
    private float xVel;
    private float yVel;
    public Fists fists;

    float speed;
    float defaultSpeed = 8.5f;
    float adrenalineSpeedBoost = 1.85f;
    float fistsSpeedBoost = 1f;
    float healingSpeedBoost = -3.5f;

    int[] inventory = { -1, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 }; //Slot 1, Slot 2, Bandages, Medkit, Soda, Pills, Vest, Helmet, Scope, Yellow Ammo, Blue Ammo, Flare Ammo, Bullets In Magazine
    float punchCooldownTimeStamp = 0f;
    float punchCooldown = 0.25f;
    float punchHitTime = 0.1f;
    int currentSlot = 2;
    float[] weaponDamage = { 13.8f, 10.35f, 10.6375f, 12.65f, 29.9f, 15.525f, 20.7f, 0f }; //M9, G18C, MAC-10, MP5, OT-38, AK-47, PKP Pecheneg, Flare Gun
    float[] weaponSpread = { 8f, 12f, 10f, 3f, 1.5f, 2.5f, 2.5f, 0f }; //M9, G18C, MAC-10, MP5, OT-38, AK-47, PKP Pecheneg, Flare Gun
    float[] weaponCooldown = { 0.12f, 0.06f, 0.045f, 0.09f, 0.4f, 0.1f, 0.1f, 0.4f }; //M9, G18C, MAC-10, MP5, OT-38, AK-47, PKP Pecheneg, Flare Gun
    int[] magazineSize = { 15, 17, 32, 30, 5, 30, 200, 1 }; //M9, G18C, MAC-10, MP5, OT-38, AK-47, PKP Pecheneg, Flare Gun
    float[] reloadTime = { 1.6f, 1.95f, 1.8f, 2f, 2f, 2.5f, 5f }; //M9, G18C, MAC-10, MP5, OT-38, AK-47, PKP Pecheneg, Flare Gun
    bool[] isAutomatic = { false, true, true, true, false, true, true, false }; //M9, G18C, MAC-10, MP5, OT-38, AK-47, PKP Pecheneg, Flare Gun
    float shootingTimeStamp = 0f;
    bool isReloading = false;
    float reloadTimeStamp;
    string[] itemName = { "M9", "G18C", "MAC-10", "MP5", "OT-38", "AK47", "PKP Pecheneg", "Flare Gun", "Bandage", "Med Kit", "Soda", "Pills", "Vest", "Helmet", "2x Scope", "Yellow Ammo", "Blue Ammo", "Flare Ammo" };

    bool isHealing = false;
    string[] healingObject = { "Bandage", "Medkit", "Soda", "Pills" };
    float[] healingAmount = { 15f, 100f, 25f, 50f };
    float[] healingTime = { 3f, 6f, 3f, 5f };
    float healingTimeStamp = 0f;
    bool[] givesHealth = { true, true, false, false };
    int currentHeal = -1;


    float health = 100f;
    float damageMulti = 1f;
    public Slider healthBar;
    public GameObject deathScreen;
    public GameObject deathIcon;

    float survivedTime = 0f;

    float adrenaline = 0f;
    float adrenalineHealRate = 5f;
    float adrenalineDepleteRate = 0.375f;
    public Slider adrenalineBar;

    public Sprite[] playerSprite;

    bool canHit = true;
    bool isFiring = false;

    public AudioClip[] soundEffects;
    AudioSource audioSource;

    float footStepCooldown = 0.4f;
    float footStepTimeStamp = 0f;

    public GameObject pickUpPanel;
    public TextMeshProUGUI itemNameText;
    void Start()
    {
        speed = defaultSpeed + fistsSpeedBoost;
        rb2d = GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        print(speed);

        survivedTime = Time.time;
        spriteRenderer.sprite = playerSprite[CurrentPlayerSprite()];
        damageMulti = 1f - (inventory[6] * 0.2f + inventory[7] * 0.1f);
        if (yVel != 1 && Input.GetKey(KeyCode.W)) {
            yVel = 1;
        }
        if (Input.GetKeyUp(KeyCode.W)) {
            yVel -= 1;
        }
        if (yVel != -1 && Input.GetKey(KeyCode.S)) {
            yVel = -1;
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            yVel += 1;
        }
        if (xVel != -1 && Input.GetKey(KeyCode.A)) {
            xVel = -1;
        }
        if (Input.GetKeyUp(KeyCode.A)) {
            xVel += 1;
        }
        if (xVel != 1 && Input.GetKey(KeyCode.D)) {
            xVel = 1;
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            xVel -= 1;
        }

        if (!isReloading && Input.GetKey(KeyCode.R) && currentSlot == 1 && inventory[CurrentAmmoType()] > 0 && inventory[12] < magazineSize[inventory[0]])
        {
            isReloading = true;
            if (isHealing)
            {
                isHealing = false;
                speed -= healingSpeedBoost;
            }
            audioSource.clip = soundEffects[inventory[0] + 11];
            audioSource.Play();
            reloadTimeStamp = Time.time + reloadTime[inventory[0]];
        }
        if (isReloading && Time.time >= reloadTimeStamp)
        {
            isReloading = false;
            audioSource.clip = null;
            int reloadAmount = ReloadAmount();
            inventory[12] += reloadAmount;
            inventory[CurrentAmmoType()] -= reloadAmount;
        }
        if (Input.GetKeyDown(KeyCode.F)) {
            GameObject pickedItem = GetItemForPickup();
            if (pickedItem != null && CanPickUp(pickedItem))
            {
                AddToInventory(pickedItem);
                audioSource.PlayOneShot(soundEffects[23]);
                Destroy(pickedItem);
            }
        }

        if (currentSlot != 1 && Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (inventory[0] != -1)
            {
                currentSlot = 1;
                speed -= fistsSpeedBoost;
                shootingTimeStamp = Time.time;
                canHit = false;
            }
        }

        if (currentSlot != 2 && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.E)))
        {
            if (isReloading)
            {
                isReloading = false;
                audioSource.clip = null;
            }
            currentSlot = 2;
            speed += fistsSpeedBoost;
            fists.GetComponent<Fists>().switchSprite(0);
            canHit = false;
        }

        if (currentSlot == 1)
        {
            switchToWeapon();
        }
        SetPlayerAngle();
        Vector2 fwd = transform.TransformDirection(Vector2.up);
        Debug.DrawRay(transform.position, fwd, Color.red);

        CheckForHit();

        if (currentHeal != -1 && isHealing && Time.time >= healingTimeStamp)
        {
            isHealing = false;
            speed -= healingSpeedBoost;
            if (givesHealth[currentHeal])
            {
                health = Mathf.Min(health + healingAmount[currentHeal], 100f);
            } else
            {
                if (adrenaline <= 50f && Mathf.Min(adrenaline + healingAmount[currentHeal], 100f) > 50f)
                {
                    speed += adrenalineSpeedBoost;
                }
                adrenaline = Mathf.Min(adrenaline + healingAmount[currentHeal], 100f);
            }
            inventory[currentHeal + 2]--;
            currentHeal = -1;
        }
        
        healthBar.value = health;
        adrenalineBar.value = adrenaline;
        if (health <= 0)
        {
            //die code here
            deathScreen.SetActive(true);
            deathIcon.transform.position = transform.position;
            deathIcon.SetActive(true);
            Destroy(gameObject);
        }
        health = Mathf.Min(health + (adrenaline / 100f) * adrenalineHealRate * Time.deltaTime, 100f);
        if (adrenaline > 50f && Mathf.Max(adrenaline - adrenalineDepleteRate * Time.deltaTime, 0f) <= 50f)
        {
            speed -= adrenalineSpeedBoost;
        }
        adrenaline = Mathf.Max(adrenaline - adrenalineDepleteRate * Time.deltaTime, 0f);
        if (rb2d.velocity.magnitude > 0 && Time.time >= footStepTimeStamp)
        {
            footStepTimeStamp = Time.time + footStepCooldown;
            audioSource.PlayOneShot(soundEffects[Random.Range(1, 3)]);
        }

        ShowItemForPickup();
    }

    void ShowItemForPickup()
    {
        GameObject item = GetItemForPickup();
        if (item != null)
        {
            itemNameText.text = itemName[item.GetComponent<Loot>().GetLootNum()];
        }

        pickUpPanel.SetActive(item != null);
    }
    void CheckForHit()
    {
        if (currentSlot == 1)
        {
            int curWeapon = inventory[0];
            if (inventory[12] > 0 && (isAutomatic[curWeapon] || canHit) && Time.time >= shootingTimeStamp && Input.GetMouseButton(0))
            {
                shootingTimeStamp = Time.time + weaponCooldown[curWeapon];
                //audioSource.PlayOneShot(soundEffects[curWeapon + 3]);
                PlaySound(curWeapon + 3);
                isFiring = true;
                canHit = false;
                GameObject firedBullet = Instantiate(bullet);
                float spread = Random.Range(-1 * weaponSpread[curWeapon] / 2, weaponSpread[curWeapon] / 2);
                Vector3 fireAngle = new Vector3(0f, 0f, playerAngle + spread + 90f);
                firedBullet.GetComponent<Bullet>().SetBullet(weaponDamage[curWeapon], 50f, 15f, transform.GetChild(1).transform.position, fireAngle);
                //add remove amunition
                inventory[12] -= 1;
            }
            else
            {
                isFiring = false;
            }
            if (!Input.GetMouseButton(0))
            {
                canHit = true;
            }
            else
            {
                if (isReloading)
                {
                    isReloading = false;
                    audioSource.clip = null;
                }
                if (isHealing)
                {
                    isHealing = false;
                    speed -= healingSpeedBoost;
                }
            }
        }

        if (currentSlot == 2)
        {
            if (!Input.GetMouseButton(0))
            {
                canHit = true;
            }
            else
            {
                if (isHealing)
                {
                    isHealing = false;
                    audioSource.clip = null;
                    speed -= healingSpeedBoost;
                }
                
            }

            if (canHit && Time.time >= punchCooldownTimeStamp && Input.GetMouseButton(0))
            {
                canHit = false;
                
                fists.switchSprite(Random.Range(1, 3));
                punchCooldownTimeStamp = Time.time + punchCooldown;
                Vector2 fwd2 = transform.TransformDirection(Vector2.up);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, fwd2, 1f, obstacleLayer);
                if (hit.collider != null)
                {
                    //audioSource.Play();
                    audioSource.PlayOneShot(soundEffects[24]);
                    if (hit.collider.tag == "Obstacle")
                    {
                        hit.collider.GetComponent<Obstacle>().TakeDamage(20f);
                    }
                    else if (hit.collider.tag == "Zombie")
                    {
                        print("test");
                        hit.collider.GetComponent<Zombie>().TakeDamage(20f);
                    }

                } else
                {
                    //audioSource.Play();
                    audioSource.PlayOneShot(soundEffects[0]);
                }
            }
            else if (Time.time >= punchCooldownTimeStamp - (punchCooldown - punchHitTime))
            {
                fists.switchSprite(0);
            }

            
        }
    }

    void PlaySound(int soundNum)
    {
        print("playing sound");
        audioSource.PlayOneShot(soundEffects[soundNum]);
    }

    void SetPlayerAngle(){
        Vector2 object_pos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 mouse_pos = Input.mousePosition;
        float angle = -1 * Mathf.Atan2(object_pos.x - mouse_pos.x, object_pos.y - mouse_pos.y) * Mathf.Rad2Deg + 180;
        playerAngle = angle;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    GameObject GetItemForPickup(){
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, circleCollider2D.radius/4, Vector2.zero, 0f, itemLayer);
        if (hit.Length == 0)
        {
            return null;
        }
        float minDistance = Vector2.Distance(transform.position, hit[0].transform.position);
        GameObject closestObject = hit[0].transform.gameObject;
        for (int i = 1; i < hit.Length; i++){
            if (Vector2.Distance(transform.position, hit[i].transform.position) < minDistance){
                minDistance = Vector2.Distance(transform.position, hit[i].transform.position);
                closestObject = hit[i].transform.gameObject;
            }
        }
        return closestObject;
    }

    
    int ReloadAmount()
    {
        if (inventory[CurrentAmmoType()] >= (magazineSize[inventory[0]] - inventory[12]))
        {
            return magazineSize[inventory[0]] - inventory[12];
        }
        return inventory[CurrentAmmoType()];
    }

    int CurrentPlayerSprite()
    {
        return inventory[6] + (inventory[7] * 2);
    }
    public bool GetIsReloading()
    {
        return isReloading;
    }

    public bool GetIsHealing()
    {
        return isHealing;
    }

    public string GetCurrentHealingObject()
    {
        return healingObject[currentHeal];
    }

    public float GetSurvivedTime()
    {
        return survivedTime;
    }

    public int CurrentAmmoType()
    {
        if (inventory[0] >= 7)
        {
            return 11;
        } else if (inventory[0] >= 4)
        {
            return 10;
        } else if (inventory[0] >= 0)
        {
            return 9;
        }
        return 0;
    }


    bool CanPickUp(GameObject pickedItem)
    {
        if (pickedItem.tag == "Weapon" && ((currentSlot == 2 && inventory[0] != -1) || pickedItem.GetComponent<Loot>().GetLootNum() == inventory[0]))
        {
            return false;
        }
        if (pickedItem.tag == "Item")
        {
            int itemNum = pickedItem.GetComponent<Loot>().GetLootNum();
            return !((itemNum == 12 && inventory[6] == 1) || (itemNum == 13 && inventory[7] == 1) || (itemNum == 14 && inventory[8] == 2));
        }
        return true;
    }
    void AddToInventory(GameObject pickedItem){
        if (pickedItem.tag == "Weapon")
        {
            if (inventory[0] != -1)
            {
                //add replacing weapon code here
                GameObject ejectedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
                Color weaponColor = new Color(1f, 1f, 1f);
                if (CurrentAmmoType() == 9)
                {
                    weaponColor = new Color(0.9490196f, 0.6470588f, 0f, 1f);
                } else if (CurrentAmmoType() == 10)
                {
                    weaponColor = new Color(0f, 0.3803922f, 0.9490197f, 1f);
                } else if (CurrentAmmoType() == 11)
                {
                    weaponColor = new Color(0.8313726f, 0.2745098f, 0f, 1f);
                }
                ejectedWeapon.GetComponent<Loot>().SetItem(inventory[0], 1, weaponColor);
                inventory[CurrentAmmoType()] += inventory[12];
                inventory[12] = 0;
                if (isReloading)
                {
                    isReloading = false;
                    audioSource.clip = null;
                }
                
            }
            inventory[0] = pickedItem.GetComponent<Loot>().GetLootNum();
        }
        else
        {
            inventory[pickedItem.GetComponent<Loot>().GetLootNum() - 6] += pickedItem.GetComponent<Loot>().GetGiveAmount();
        }
    }

    void switchToWeapon()
    {
        fists.GetComponent<Fists>().switchSprite(inventory[0] + 3);
    }

    public void Heal(int num)
    {

        if (!isHealing && inventory[num + 2] > 0 && (health < 100 && num < 2 || adrenaline < 100 && num > 1))
        {
            print("initiated heal");
            isHealing = true;
            speed += healingSpeedBoost;
            audioSource.clip = soundEffects[num + 19];
            audioSource.Play();
            currentHeal = num;
            healingTimeStamp = Time.time + healingTime[currentHeal];
        }
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(health - (damage * damageMulti), 0f);
    } 

    public bool GetIsFiring()
    {
        return isFiring;
    }
    public int GetInventory(int index)
    {
        return inventory[index];
    }

    public int GetCurrentSlot()
    {
        return currentSlot;
    }
    void FixedUpdate()
    {
        rb2d.velocity = new Vector2(xVel, yVel);
        rb2d.velocity = speed * (rb2d.velocity.normalized);
        //make action done by clicking under one helper function
        
    }
}
