using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBox : MonoBehaviour
{
    private float hp = 5;

    public AudioClip impactClip;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Hammer"))
        {
            hp -= 5;
            audioSource.PlayOneShot(impactClip);
        }

        if (collision.gameObject.CompareTag("Sickle"))
        {
            hp -= 0;
            collision.gameObject.GetComponent<Sickle>().Stick(gameObject);
        }

        if (hp <= 0)
        {
            Destroy(gameObject);
            for (int i = 0; i < 3; i++)
            {
                if (Random.value > 0.5)
                {
                    var bonus = Instantiate(Resources.Load<GameObject>("Prefabs/BonusHP"));
                    bonus.transform.position = new Vector3(transform.position.x, transform.position.y);
                }
                else
                {
                    var bonus = Instantiate(Resources.Load<GameObject>("Prefabs/BonusGas"));
                    bonus.transform.position = new Vector3(transform.position.x, transform.position.y);
                }
            }
        }
    }


}
