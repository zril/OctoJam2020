using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{

    private float levelSpeed = 1f;
    private int levelWidth = 18;
    private int leftWall = 3;
    private int rightWall = 3;


    private GameObject[] clouds;
    private GameObject[] deathclouds;
    private GameObject mainCamera;
    private GameObject level;

    private int lineCounter;

    private int[,] levelTiles;
    private List<LevelObject> objectList;
    private int objectTotalProbability;
    private List<GameObject> objectPrefabs;
    private GameObject wallPrefab;
    private List<LevelEnemy> enemyList;
    private List<GameObject> enemyPrefabs;
    private int enemyTotalProbability;

    private GameObject canvas;
    private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        clouds = GameObject.FindGameObjectsWithTag("Cloud");
        deathclouds = GameObject.FindGameObjectsWithTag("DeathCloud");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        level = GameObject.FindGameObjectWithTag("Level");
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        player = GameObject.FindGameObjectWithTag("Player");

        levelTiles = new int[levelWidth, 10000];
        for(int i = 0; i < levelWidth; i++)
        {
            levelTiles[i, 0] = -1;
        }

        objectList = new List<LevelObject>();
        objectList.Add(new LevelObject(0, "Nothing", 0, 0, 18));
        objectList.Add(new LevelObject(1, "Platform", 4, 1, 4));
        objectList.Add(new LevelObject(2, "SmallPlatform", 2, 1, 5));
        objectList.Add(new LevelObject(3, "BoxPlatform", 2, 2, 1));
        objectTotalProbability = 0;
        objectPrefabs = new List<GameObject>();
        foreach (LevelObject obj in objectList)
        {
            objectTotalProbability += obj.Probability;
            objectPrefabs.Add(Resources.Load<GameObject>("Prefabs/" + obj.Prefab));
        }
        wallPrefab = Resources.Load<GameObject>("Prefabs/Wall");

        enemyList = new List<LevelEnemy>();
        enemyList.Add(new LevelEnemy(0, "Nothing", 15));
        enemyList.Add(new LevelEnemy(1, "Enemy1", 1));
        enemyTotalProbability = 0;
        enemyPrefabs = new List<GameObject>();
        foreach (LevelEnemy obj in enemyList)
        {
            enemyTotalProbability += obj.Probability;
            enemyPrefabs.Add(Resources.Load<GameObject>("Prefabs/" + obj.Prefab));
        }


        lineCounter = 0;
        for (int i = 0; i < 50; i++)
        {
            SpawnLine(lineCounter);
            lineCounter++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject cloud in clouds)
        {
            cloud.transform.position += new Vector3(0, levelSpeed * Time.deltaTime, 0);
        }

        foreach (GameObject cloud in deathclouds)
        {
            cloud.transform.position += new Vector3(0, levelSpeed * Time.deltaTime, 0);
        }

        mainCamera.transform.position += new Vector3(0, levelSpeed * Time.deltaTime, 0);


        while (lineCounter < mainCamera.transform.position.y + 50)
        {
            SpawnLine(lineCounter);
            lineCounter++;
        }

        UpdateUI();
    }

    private void SpawnLine(int line)
    {
        if (line % 5 == 0)
        {
            SpawnWall(line);
        }

        int currentWidth = levelWidth - leftWall - rightWall;
        int offset = Random.Range(leftWall, currentWidth);

        for (int i = 0; i < currentWidth; i++)
        {
            var x = (offset + i) % currentWidth;

            if (levelTiles[leftWall + x, line] != -1)
            {
                int roll = Random.Range(0, objectTotalProbability);
                int index = -1;
                while (roll >= 0)
                {
                    index++;
                    roll -= objectList[index].Probability;
                }

                var obj = objectList[index];
                SpawnObject(line, leftWall + x, obj);
            }

            if (line > 5)
            {
                RollEnemy(line, x);
            }
        }


    }

    private bool SpawnObject(int line, int xpos, LevelObject obj)
    {
        if (obj.Prefab != "Nothing")
        {
            bool valid = true;

            for(int i = 0; i < obj.Width; i++)
            {
                for (int j = 0; j < obj.Height; j++)
                {
                    if (xpos + i >= levelWidth - rightWall)
                    {
                        valid = false;
                    }
                    else if(levelTiles[xpos + i, line + j] != 0)
                    {
                        valid = false;
                    }
                }
            }

            if (valid)
            {
                for (int i = -1; i <= obj.Width; i++)
                {
                    for (int j = -1; j <= obj.Height; j++)
                    {
                        if (xpos + i > 0 && xpos + i < levelWidth && line + j > 0)
                        {
                            if (i == -1 || j == -1)
                            {
                                levelTiles[xpos + i, line + j] = -1;
                            }
                            else if (i < obj.Width && j < obj.Height)
                            {
                                levelTiles[xpos + i, line + j] = obj.Id;
                            }
                            else
                            {
                                levelTiles[xpos + i, line + j] = -1;
                            }
                        }
                    }
                }

                var o = GameObject.Instantiate(objectPrefabs[obj.Id], level.transform);
                o.transform.position = new Vector3(0.5f -((float)levelWidth / 2) + xpos + (float)(obj.Width - 1) / 2, 0.5f + line + (float)(obj.Height - 1) / 2, 0);

                return true;
            } else
            {
                //var o1 = GameObject.Instantiate(objectPrefabs[0], level.transform);
                //o1.transform.position = new Vector3(-((float)levelWidth / 2) + xpos, line, 0);

                return false;
            }

            
        }

        //var o2 = GameObject.Instantiate(objectPrefabs[0], level.transform);
        //o2.transform.position = new Vector3(-((float)levelWidth / 2) + xpos, line, 0);

        return true;

    }

    private void SpawnWall(int line)
    {
        int roll1;
        if (leftWall > 4) roll1 = Random.Range(0, 4) - 2;
        else if (leftWall < 2) roll1 = Random.Range(0, 4) - 1;
        else roll1 = Random.Range(0, 3) - 1;
        leftWall += roll1;
        if (leftWall < 0) leftWall = 0;
        if (leftWall > 6) leftWall = 6;
        var wall1 = GameObject.Instantiate(wallPrefab, level.transform);
        wall1.transform.position = new Vector3(-((float)levelWidth / 2) - 1 + leftWall, line + 5f / 2f, 0);

        int roll2;
        if (rightWall > 4) roll2 = Random.Range(0, 4) - 2;
        else if (rightWall < 2) roll2 = Random.Range(0, 4) - 1;
        else roll2 = Random.Range(0, 3) - 1;
        rightWall += roll2;
        if (rightWall < 0) rightWall = 0;
        if (rightWall > 6) rightWall = 6;
        var wall2 = GameObject.Instantiate(wallPrefab, level.transform);
        wall2.transform.position = new Vector3(((float)levelWidth / 2) + 1 - rightWall, line + 5f / 2f, 0);
    }

    private void RollEnemy(int line, int x)
    {
        int roll = Random.Range(0, enemyTotalProbability);
        int index = -1;
        while (roll >= 0)
        {
            index++;
            roll -= enemyList[index].Probability;
        }

        var obj = enemyList[index];
        SpawnEnemy(line, leftWall + x, obj);
    }

    private void SpawnEnemy(int line, int xpos, LevelEnemy enemy)
    {
        if (enemy.Prefab != "Nothing")
        {
            var enemyRoot = transform.Find("Enemy");
            var o = GameObject.Instantiate(enemyPrefabs[enemy.Id], enemyRoot.transform);
            o.transform.position = new Vector3(0.5f - ((float)levelWidth / 2) + xpos, 0.5f + line + 1f, 0);
        }
    }

    private void UpdateUI()
    {
        var hp = canvas.transform.Find("HP");
        hp.GetComponent<Text>().text = string.Format("HP {0}/100", player.GetComponent<Player>().GetHP());

        var gas = canvas.transform.Find("Gas");
        gas.GetComponent<Text>().text = string.Format("Gas {0}/20", player.GetComponent<Player>().GetGas());
    }
}
