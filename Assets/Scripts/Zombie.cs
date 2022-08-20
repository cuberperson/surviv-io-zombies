using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rb2d;

    GameObject gameManager;

    GameObject player;
    Player playerScript;
    GameObject fists;

    float alertnessLevel = 0f;
    float alertDistance = 10f;
    int isAlert = 0; // not alert: 0, alertness 50: 1, alertness 75: 2, alert: 3
    float alertTimeStamp = 0f;
    float alertCooldownNear = 1.5f;
    float alertCooldownFar = 3f;

    Vector2 target;
    float timeStamp = 0f;
    float minTargetSwitchTime = 3.5f;
    float maxTargetSwitchTime = 5f;

    float minWanderRange = 2f;
    float maxWanderRange = 3f;

    float alertSpeedBoost = 1.5f;

    float speed = 2.5f;
    float health = 35f;

    public GameObject weapon;
    public GameObject ammo;
    public GameObject item;
    public int[] lootTable;
    public int[] lootProb;
    string[] weapons = { "M9", "G18C", "MAC-10", "MP5", "OT-38", "AK47", "PKP Pecheneg", "Flare Gun", "Bandage", "Med Kit", "Soda", "Pills", "Vest", "Helmet", "2x Scope", "Yellow Ammo", "Blue Ammo", "Flare Ammo" };
    int[] itemAmount = { 25, 26, 48, 45, 10, 45, 175, 1, 5, 1, 1, 1, 1, 1, 1, 60, 60, 1};

    float punchCooldownTimeStamp = 0f;
    float punchCooldown = 0.25f;
    float punchHitTime = 0.1f;
    float punchThreshold = 0.75f;
    public LayerMask playerLayer;

    bool first = true;

    public AudioClip[] punchSounds;
    AudioSource audioSource;

    void Start()
    {
        fists = transform.GetChild(0).gameObject;
        playerScript = player.GetComponent<Player>();
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            alertnessLevel = GetAlertnessLevel();
            //print(alertnessLevel);

            /*
            if (alertnessLevel >= 75)
            {
                if (!isAlert)
                {
                    isAlert = true;
                    speed += alertSpeedBoost;
                }
                target = player.transform.position;
                timeStamp = Time.time;
            }
            else if (!isAlert && alertnessLevel >= 50)
            {
                target = player.transform.position;
                timeStamp = Time.time;
            }
            */
            if (isAlert < 3 && alertnessLevel >= 75)
            {
                
                if (isAlert == 1)
                {
                    alertTimeStamp -= alertCooldownFar - alertCooldownNear;
                    isAlert = 2;
                    Wander();
                } 
                else if (isAlert == 0)
                {
                    alertTimeStamp = Time.time + alertCooldownNear;
                    isAlert = 2;
                    Wander();
                }
                else if (isAlert == 2 && Time.time >= alertTimeStamp)
                {
                    isAlert = 3;
                    speed += alertSpeedBoost;
                } else
                {
                    Wander();
                }
            } 
            else if (isAlert < 3 && alertnessLevel >= 50)
            {
                if (isAlert == 2)
                {
                    alertTimeStamp += alertCooldownFar - alertCooldownNear;
                    isAlert = 1;
                    Wander();
                } 
                else if (isAlert == 0)
                {
                    alertTimeStamp = Time.time + alertCooldownFar;
                    isAlert = 1;
                    Wander();
                }
                else if (isAlert == 1 && Time.time >= alertTimeStamp)
                {
                    isAlert = 3;
                    speed += alertSpeedBoost;
                } else
                {
                    Wander();
                }
            }
            else if (isAlert < 3 && alertnessLevel > 0)
            {
                Wander();
            }
            else if (alertnessLevel <= 0)
            {
                if (isAlert == 3)
                {
                    isAlert = 0;
                    speed -= alertSpeedBoost;
                }

                Wander();
            }
            if (isAlert == 3)
            {
                target = player.transform.position;
                timeStamp = Time.time;
            }
            Walk();

            if (isAlert == 3 && Vector2.Distance(player.transform.position, transform.position) <= alertDistance * punchThreshold)
            {
                Punch();
            }
            else
            {
                fists.GetComponent<Fists>().switchSprite(0);
            }
            Vector2 dir = transform.TransformDirection(Vector2.up);
            Debug.DrawRay(transform.position, dir, Color.red);
        } else
        {
            if (first)
            {
                first = false;
                fists.GetComponent<Fists>().switchSprite(0);
                timeStamp = Time.time;
            }
            Wander();
            Walk();

        }
    }

    public void SetPlayer(GameObject obj)
    {
        player = obj;
    }

    public void SetGameManager(GameObject obj)
    {
        gameManager = obj;
    }

    void Punch()
    {
        if (Time.time >= punchCooldownTimeStamp)
        {
            fists.GetComponent<Fists>().switchSprite(Random.Range(1, 3));
            punchCooldownTimeStamp = Time.time + punchCooldown;
            Vector2 fwd = transform.TransformDirection(Vector2.up);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, fwd, 1f, playerLayer);
            if (hit.collider != null)
            {
                audioSource.PlayOneShot(punchSounds[1]);
                hit.collider.GetComponent<Player>().TakeDamage(10f);
            }
            else
            {
                audioSource.PlayOneShot(punchSounds[0]);
            }
        }
        else if (Time.time >= punchCooldownTimeStamp - (punchCooldown - punchHitTime))
        {
            fists.GetComponent<Fists>().switchSprite(0);
        }
    }

    void Walk()
    {
        if (Vector2.Distance(target, transform.position) >= 0.1f)
        {
            rb2d.velocity = (new Vector2(target.x - transform.position.x, target.y - transform.position.y)).normalized * speed;
        }
        else
        {
            rb2d.velocity = Vector2.zero;
        }
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, GetCurrentAngle()));

    }
    void Wander()
    {
        if (Time.time >= timeStamp)
        {
            timeStamp = Time.time + Random.Range(minTargetSwitchTime, maxTargetSwitchTime);
            float xOff = (Random.Range(0, 2) * 2 - 1) * Random.Range(minWanderRange, maxWanderRange);
            float yOff = (Random.Range(0, 2) * 2 - 1) * Random.Range(minWanderRange, maxWanderRange);
            target = new Vector2(transform.position.x + xOff, transform.position.x + yOff);
        }
    }

    float GetCurrentAngle()
    {
        return -1 * Mathf.Atan2(target.x - transform.position.x, target.y - transform.position.y) * Mathf.Rad2Deg;
    }

    float GetAlertnessLevel()
    {
        float result = 0;
        if (Vector2.Distance(transform.position, player.transform.position) <= alertDistance)
        {
            result += 75f;
        }
        else if (Vector2.Distance(transform.position, player.transform.position) <= alertDistance * 1.5f)
        {
            result += 50f;
        }
        if (playerScript.GetIsFiring())
        {
            result += 25f;
        }
        return result;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            int lootNum = chooseLoot();
            GiveLoot(lootNum);
            gameManager.GetComponent<Game_Manager>().IncreaseKill(1);
            Destroy(gameObject);
        }
    }

    int chooseLoot()
    {
        float lootNum = Random.Range(1, 101);
        for (int i = 0; i < lootProb.Length; i++)
        {
            if (lootNum > lootProb[i])
            {
                print(lootTable[i]);
                return lootTable[i];
            }
        }
        print("failed");
        return 0;
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
}
