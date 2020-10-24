using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    private float levelSpeed = 0.5f;
    private int levelWidth = 18;
    private int leftWall = 3;
    private int rightWall = 3;


    private GameObject[] clouds;
    private GameObject mainCamera;
    private GameObject level;

    private float spawnTimer;
    private int lineCounter;

    private int[,] levelTiles;
    private List<LevelObject> objectList;
    private int totalProbability;
    private List<GameObject> objectPrefabs;
    private GameObject wallPrefab;
    // Start is called before the first frame update
    void Start()
    {
        clouds = GameObject.FindGameObjectsWithTag("Cloud");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        level = GameObject.FindGameObjectWithTag("Level");

        spawnTimer = 1;
        levelTiles = new int[levelWidth, 10000];

        objectList = new List<LevelObject>();
        objectList.Add(new LevelObject(0, "Nothing", 0, 0, 18));
        objectList.Add(new LevelObject(1, "Platform", 4, 1, 4));
        objectList.Add(new LevelObject(2, "SmallPlatform", 2, 1, 5));
        objectList.Add(new LevelObject(3, "BoxPlatform", 2, 2, 1));
        totalProbability = 0;
        objectPrefabs = new List<GameObject>();
        foreach (LevelObject obj in objectList)
        {
            totalProbability += obj.Probability;
            objectPrefabs.Add(Resources.Load<GameObject>("Prefabs/" + obj.Prefab));
        }
        wallPrefab = Resources.Load<GameObject>("Prefabs/Wall");


        lineCounter = 0;
        for (int i = 0; i < 15; i++)
        {
            SpawnLine(lineCounter);
            lineCounter++;
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject cloud in clouds)
        {
            cloud.transform.position += new Vector3(0, levelSpeed * Time.deltaTime, 0);
        }

        mainCamera.transform.position += new Vector3(0, levelSpeed * Time.deltaTime, 0);


        spawnTimer -= Time.deltaTime * levelSpeed;
        if (spawnTimer < 0)
        {
            spawnTimer += 1;
            SpawnLine(lineCounter);
            lineCounter++;
        }
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
                int roll = Random.Range(0, totalProbability);
                int index = -1;
                while (roll >= 0)
                {
                    index++;
                    roll -= objectList[index].Probability;
                }

                var obj = objectList[index];
                SpawnObject(line, leftWall + x, obj);
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
}
