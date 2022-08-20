using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D crosshair;

    public int mapX;
    public int mapY;
    public GameObject[] obstacles;
    public int[] obstacleSpawnNum;

    public GameObject player;
    Player playerScript;
    int playerKills = 0;

    public Sprite[] weaponIcon;
    public GameObject[] itemPanel;

    public TextMeshProUGUI[] textUI;

    public Image slot1Icon;
    public Image selectedIcon;

    public GameObject zombie;
    int maxZombiesAlive = 5;
    int zombiesAlive = 0;
    int absoluteMaxZombies = 50;
    int waveNum = 0;
    float timeBetweenWaves = 30f;
    float waveTimeStamp = 0f;

    string[] weapons = { "M9", "G18C", "MAC-10", "MP5", "OT-38", "AK47", "PKP Pecheneg", "Flare Gun", "Bandage", "Med Kit", "Soda", "Pills", "Vest", "Helmet", "2x Scope", "Yellow Ammo", "Blue Ammo", "Flare Ammo" };

    // Update is called once per frame
    void Start()
    {
        Cursor.SetCursor(crosshair, new Vector2(crosshair.width / 2, crosshair.height / 2), CursorMode.Auto);
        SpawnObstacles();
        playerScript = player.GetComponent<Player>();
    }

    void SpawnObstacles()
    {
        for (int obstacleType = 0; obstacleType < obstacles.Length; obstacleType++)
        {
            for (int i = 0; i < obstacleSpawnNum[obstacleType]; i++)
            {
                float xPos = Random.Range(-9 * mapX, 9 * mapX);
                float yPos = Random.Range(-6 * mapY, 6 * mapY);
                bool canSpawn = false;
                if (obstacles[obstacleType].GetComponent<BoxCollider2D>() != null)
                {
                    canSpawn = Physics2D.OverlapBox(new Vector2(xPos, yPos), obstacles[obstacleType].GetComponent<BoxCollider2D>().size, 0) == null;
                } 
                else if (obstacles[obstacleType].GetComponent<CircleCollider2D>() != null)
                {
                    canSpawn = Physics2D.OverlapCircle(new Vector2(xPos, yPos), obstacles[obstacleType].GetComponent<CircleCollider2D>().radius) == null;
                }
                if (canSpawn)
                {
                    Instantiate(obstacles[obstacleType], new Vector2(xPos, yPos), Quaternion.identity);
                }
            }
        }
    }

    public void Pressed()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    void SummonZombieWave(float spawnRange)
    {
        waveNum++;
        waveTimeStamp = Time.time + timeBetweenWaves;
        maxZombiesAlive = Mathf.Min((int)(15f * Mathf.Log10((float)waveNum + 1f) + 5f), absoluteMaxZombies);
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;
        for (int i = 0; i < maxZombiesAlive - zombiesAlive; i++)
        {
            float xPos = Random.Range(Mathf.Max(playerX - spawnRange, -9f * mapX), Mathf.Min(playerX + spawnRange, 9f * mapX));
            float yPos = Random.Range(Mathf.Max(playerY - spawnRange, -6f * mapY), Mathf.Min(playerY + spawnRange, 6f * mapY));
            float spawnAngle = Random.Range(0f, 360f);
            if (Physics2D.OverlapCircle(new Vector2(xPos, yPos), zombie.GetComponent<CircleCollider2D>().radius) == null)
            {
                GameObject spawned = Instantiate(zombie, new Vector2(xPos, yPos), Quaternion.Euler(0f, 0f, spawnAngle));
                spawned.GetComponent<Zombie>().SetPlayer(player);
                spawned.GetComponent<Zombie>().SetGameManager(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInventoryUI();
        zombiesAlive = GameObject.FindGameObjectsWithTag("Zombie").Length;
        //print("thresh: " + (int)(maxZombiesAlive * 0.4f));
        if (player != null && (zombiesAlive <= (int)(maxZombiesAlive * 0.4f) || Time.time >= waveTimeStamp))
        {
            //print("spawn");
            SummonZombieWave(25f);
        }
        //print(GameObject.FindGameObjectsWithTag("Zombie").Length);
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < textUI.Length; i++)
        {
            if (i == 0)
            {   
                if (playerScript.GetInventory(i) != -1)
                {
                    textUI[i].text = weapons[playerScript.GetInventory(i)];
                    slot1Icon.sprite = weaponIcon[playerScript.GetInventory(i)];
                }
                else
                {
                    textUI[i].text = "Fists";
                    slot1Icon.sprite = weaponIcon[weaponIcon.Length - 1];
                }
                
            }
            if (i >= 2 && i <= 5)
            {
                textUI[i].text = (playerScript.GetInventory(i)).ToString();
            }
            if (i >= 6 && i <= 8)
            {
                textUI[i].text = (playerScript.GetInventory(i + 3)).ToString();
            }
            if (i == 9)
            {
                textUI[i].text = (playerScript.GetInventory(12)).ToString();
            }
            if (i == 10)
            {
                int gunNum = playerScript.GetInventory(0);
                int ammoNum = playerScript.GetInventory(playerScript.CurrentAmmoType());
                textUI[i].text = ammoNum.ToString();
                if (ammoNum == 0)
                {
                    itemPanel[3].SetActive(false);
                }
            }
            if (i == 11 && playerScript.GetIsHealing())
            {
                textUI[i].text = "Using " + playerScript.GetCurrentHealingObject();
            }
            if (i == 12)
            {
                int minutesSurvived = (int)(playerScript.GetSurvivedTime() / 60f);
                int secondsSurvived = (int)(playerScript.GetSurvivedTime() - minutesSurvived * 60f);
                string survivedTime = "Survived Time:";
                if (minutesSurvived > 0)
                {
                    survivedTime += " " + minutesSurvived + "m";
                }
                survivedTime += " " + secondsSurvived + "s";
                textUI[i].text = survivedTime;
            }
            if (i == 13)
            {
                textUI[i].text = "Kills: " + playerKills;
            }
        }
        for (int i = 0; i < 2; i++)
        {
            if (playerScript.GetInventory(i + 6) == 1)
            {
                itemPanel[i].SetActive(true);
            }
        }
        if (playerScript.GetInventory(8) == 2)
        {
            itemPanel[2].SetActive(true);
        }
        if (playerScript.GetCurrentSlot() == 1)
        {
            itemPanel[3].SetActive(true);
            if (playerScript.GetInventory(playerScript.CurrentAmmoType()) > 0)
            {
                itemPanel[4].SetActive(true);
            } else
            {
                itemPanel[4].SetActive(false);
            }
            
        } else
        {
            itemPanel[3].SetActive(false);
            itemPanel[4].SetActive(false);
        }
        itemPanel[5].SetActive(playerScript.GetIsReloading());
        itemPanel[6].SetActive(playerScript.GetIsHealing());
        selectedIcon.rectTransform.anchoredPosition = new Vector2(selectedIcon.rectTransform.anchoredPosition.x, 150f - 83.8f * (playerScript.GetCurrentSlot() - 1));
    }

    public void IncreaseKill(int amount)
    {
        playerKills += amount;
    }
}
