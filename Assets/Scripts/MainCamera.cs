﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCamera : MonoBehaviour
{
    private GameObject player;

    private float distThreshold = 3;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }

        if (player.transform.position.y - transform.position.y > distThreshold)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y - distThreshold, transform.position.z);
        } else if (player.transform.position.y - transform.position.y < -distThreshold)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y + distThreshold, transform.position.z);
        } else
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
