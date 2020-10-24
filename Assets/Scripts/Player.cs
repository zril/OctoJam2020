using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float basejumpspeed;
    public float holdjumpfactor;

    private float hp = 100;
    private float maxHp = 100;
    private float toxicDps = 1;

    private bool left = false;
    private bool right = false;
    private bool jump = false;
    private bool ground = false;
    private int toxic = 0;

    private float jumpTime = 0.15f;
    private float jumpTimer = 0;

    private float sickleCooldown = 0.5f;
    private float sickleCooldownTimer = 0;

    private float hammerDuration = 0.2f;
    private float hammerCooldown = 0.5f;
    private float hammerTimer = 0;

    private GameObject sicklePrefab;


    // Start is called before the first frame update
    void Start()
    {
        sicklePrefab = Resources.Load<GameObject>("Prefabs/Sickle");
    }

    private void FixedUpdate()
    {

        var rigidbody = GetComponent<Rigidbody2D>();
        var vel = rigidbody.velocity;
        if (right)
        {
            rigidbody.velocity = new Vector2(speed, vel.y);
        }

        if (left)
        {
            rigidbody.velocity = new Vector2(-speed, vel.y);
        }

        if (!right && !left)
        {
            rigidbody.velocity = new Vector2(0, vel.y);
        }

        if (jump && jumpTimer > 0)
        {
            rigidbody.velocity = new Vector2(vel.x, basejumpspeed + (holdjumpfactor * (jumpTime - jumpTimer) / jumpTime));
            ground = false;
        }

        if (jump && jumpTimer < 0)
        {
            rigidbody.velocity = new Vector2(vel.x, basejumpspeed + holdjumpfactor);
            jump = false;
        }

    }


    // Update is called once per frame
    void Update()
    {
        // movement
        right = Input.GetAxis("Horizontal") > 0;
        left = Input.GetAxis("Horizontal") < 0;

        // jump
        if (Input.GetButtonUp("Jump"))
        {
            jump = false;
        }

        if (jump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && ground)
        {
            jump = true;
            jumpTimer = jumpTime;
        }

        //todo buffer jump

        // sickle
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousepos.z = 0;

        if (sickleCooldownTimer > 0)
        {
            sickleCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Sickle") && sickleCooldownTimer <= 0)
        {
            var sickle = GameObject.Instantiate(sicklePrefab);
            sickle.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            var angle = Mathf.Atan2(mousepos.y - transform.position.y, mousepos.x - transform.position.x) * Mathf.Rad2Deg;
            sickle.GetComponent<Sickle>().Direction = angle;

            sickleCooldownTimer = sickleCooldown;
        }


        // hammer
        if (mousepos.x - transform.position.x > 0)
        {
            transform.Find("HammerHitbox").transform.localPosition = Vector3.right;
        }
        if (mousepos.x - transform.position.x < 0)
        {
            transform.Find("HammerHitbox").transform.localPosition = Vector3.left;
        }

        if (Input.GetButtonDown("Hammer") && hammerTimer <= 0)
        {
            hammerTimer = hammerCooldown;
            transform.Find("HammerHitbox").gameObject.SetActive(true);
        }

        if (hammerTimer > 0)
        {
            hammerTimer -= Time.deltaTime;
            if (hammerTimer < hammerCooldown - hammerDuration)
            {
                transform.Find("HammerHitbox").gameObject.SetActive(false);
            }
        }

        // damage
        if (toxic > 0)
        {
            hp -= Time.deltaTime * toxicDps;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Floor") && col.GetContact(0).normal.x == 0 && col.GetContact(0).normal.y > 0)
        {
            ground = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            toxic++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            toxic--;
        }
    }


}
