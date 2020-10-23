using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float basejumpspeed;
    public float holdjumpfactor;



    private bool left = false;
    private bool right = false;
    private bool jump = false;
    private bool ground = false;
    private bool falling = true;

    private float jumpTime = 0.15f;
    private float jumpTimer = 0;

    private float sickleCooldown = 0.5f;
    private float sickleCooldownTimer = 0;

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
            falling = false;
        }

        if (jump && jumpTimer < 0)
        {
            rigidbody.velocity = new Vector2(vel.x, basejumpspeed + holdjumpfactor);
            jump = false;
        }


        if (rigidbody.velocity.y < 0)
        {
            falling = true;
        }

        if (falling)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Floor"), LayerMask.NameToLayer("Player"), false);
        } else
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Floor"), LayerMask.NameToLayer("Player"), true);
        }

    }


    // Update is called once per frame
    void Update()
    {

        right = Input.GetAxis("Horizontal") > 0;
        left = Input.GetAxis("Horizontal") < 0;


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

        if (sickleCooldownTimer > 0)
        {
            sickleCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Sickle") && sickleCooldownTimer <= 0)
        {
            var sickle = GameObject.Instantiate(sicklePrefab);
            sickle.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousepos.z = 0;
            var angle = Mathf.Atan2(mousepos.y - transform.position.y, mousepos.x - transform.position.x) * Mathf.Rad2Deg;
            sickle.GetComponent<Sickle>().Direction = angle;

            sickleCooldownTimer = sickleCooldown;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.CompareTag("Floor"))
        {
            ground = true;
        }
    }


}
