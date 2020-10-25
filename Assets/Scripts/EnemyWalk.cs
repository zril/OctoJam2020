using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    private bool right;
    private float speed = 0.2f;


    private float hp = 5;

    public AudioClip impactClip;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (Random.value > 0.5f) right = !right;

        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        var rigidbody = GetComponent<Rigidbody2D>();
        var vel = rigidbody.velocity;
        if (right)
        {
            rigidbody.velocity = new Vector2(speed, vel.y);
        } 
        else
        {
            rigidbody.velocity = new Vector2(-speed, vel.y);
        }
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Floor") && col.GetContact(0).normal.y == 0)
        {
            right = !right;
        }
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
            hp -= 4;
            collision.gameObject.GetComponent<Sickle>().Stick(gameObject);
        }

        if (hp <= 0)
        {
            Destroy(gameObject);

            if (Random.value > 0.85)
            {
                var bonus = Instantiate(Resources.Load<GameObject>("Prefabs/BonusHP"));
                bonus.transform.position = new Vector3(transform.position.x, transform.position.y);
            }
        }
    }


}
