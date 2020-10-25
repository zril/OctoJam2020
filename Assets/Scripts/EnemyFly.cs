using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    private float speed = 30f;


    private float hp = 2;

    private Vector3 startpos;
    private float timer;

    public AudioClip impactClip;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        timer = Random.value * 6;
        startpos = transform.position;
        transform.position += new Vector3(Random.value - 0.5f, Random.value - 0.5f);

        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        var angle = timer * Mathf.Deg2Rad * speed;
        transform.position = startpos + new Vector3(Mathf.Cos(angle) * 0.5f, Mathf.Sin(angle) * 0.5f);

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
