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
    private float gas = 20;
    private int maxGas = 20;
    private float toxicDps = 1;
    private float gasPerSec = 1;

    private bool left = false;
    private bool right = false;
    private bool jump = false;
    private bool ground = false;
    private int toxic = 0;

    private float jumpTime = 0.15f;
    private float jumpTimer = 0;

    private float sickleCooldown = 0.3f;
    private float sickleCooldownTimer = 0;

    private float hammerDuration = 0.2f;
    private float hammerCooldown = 0.4f;
    private float hammerTimer = 0;

    private float grapleDuration = 0.3f;
    private float grapleTimer = 0f;
    private int grapleNumber = 30;
    private bool graple = false;


    private GameObject damageTaken;
    private float damageTimer = 0;
    private float damageDuration = 0.25f;

    private GameObject sicklePrefab;

    private GameObject currentSickle;
    private List<GameObject> grapleObj;
    private GameObject graplePrefab;


    // Start is called before the first frame update
    void Start()
    {
        sicklePrefab = Resources.Load<GameObject>("Prefabs/Sickle");
        graplePrefab = Resources.Load<GameObject>("Prefabs/Graple");

        grapleObj = new List<GameObject>();
    }

    private void FixedUpdate()
    {
        // movement
        var rigidbody = GetComponent<Rigidbody2D>();
        var vel = rigidbody.velocity;
        var run = false;
        if (right)
        {
            rigidbody.velocity = new Vector2(speed, vel.y);
            run = true;
        }

        if (left)
        {
            rigidbody.velocity = new Vector2(-speed, vel.y);
            run = true;
        }

        if (!right && !left && damageTimer <= 0 && grapleTimer <= 0)
        {
            rigidbody.velocity = new Vector2(0, vel.y);
        }

        if (run)
        {
            GetComponent<Animator>().Play("run");
        } else
        {
            GetComponent<Animator>().Play("idle");
        }

        // jump
        vel = rigidbody.velocity;
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

        if (rigidbody.velocity.y < 0)
        {
            ground = false;
        }

        // graple
        if (graple)
        {
            rigidbody.velocity = new Vector2(vel.x, basejumpspeed + 1);
            graple = false;
        }

        // damage
        vel = rigidbody.velocity;
        if (damageTaken != null)
        {
            if (damageTaken.transform.position.x > transform.position.x)
            {
                rigidbody.velocity = new Vector2(vel.x - 4, vel.y + 1);
            } else
            {
                rigidbody.velocity = new Vector2(vel.x + 4, vel.y + 1);
            }
            
            damageTaken = null;
            damageTimer = damageDuration;
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
            right = false;
            left = false;
        }

        // movement
        if (damageTimer <= 0 && grapleTimer <= 0)
        {
            right = Input.GetAxis("Horizontal") > 0;
            left = Input.GetAxis("Horizontal") < 0;
        }

        // jump
        if (Input.GetButtonUp("Jump") && damageTimer <= 0 && grapleTimer <= 0)
        {
            jump = false;
        }

        if (jump)
        {
            jumpTimer -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && ground && damageTimer <= 0 && grapleTimer <= 0)
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

        if (Input.GetButtonDown("Sickle") && sickleCooldownTimer <= 0 && damageTimer <= 0 && gas > 0 && grapleTimer <= 0)
        {
            var sickle = GameObject.Instantiate(sicklePrefab);
            sickle.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            var angle = Mathf.Atan2(mousepos.y - transform.position.y, mousepos.x - transform.position.x) * Mathf.Rad2Deg;
            sickle.GetComponent<Sickle>().Direction = angle;

            sickleCooldownTimer = sickleCooldown;
            gas--;
            currentSickle = sickle;
        }
        if (Input.GetButtonUp("Sickle"))
        {
            if (currentSickle != null && grapleTimer <= 0 && currentSickle.GetComponent<Sickle>().GetStick())
            {
                grapleTimer = grapleDuration;
            }

            currentSickle = null;
        }
        if (grapleTimer > 0)
        {
            grapleTimer -= Time.deltaTime;
            var count = grapleObj.Count;
            if (count > 1)
            {
                var remaining = grapleTimer / grapleDuration;
                if (count > 20f * remaining)
                {
                    transform.position = grapleObj[0].transform.position;
                    Destroy(grapleObj[0]);
                    grapleObj.RemoveAt(0);
                }
            } else
            {
                graple = true;
            }
        } else
        {
            UpdateGraple();
        }



        // hammer
        if (mousepos.x - transform.position.x > 0)
        {
            transform.Find("HammerHitbox").transform.localPosition = Vector3.right;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (mousepos.x - transform.position.x < 0)
        {
            transform.Find("HammerHitbox").transform.localPosition = Vector3.left;
            GetComponent<SpriteRenderer>().flipX = true;
        }

        if (Input.GetButtonDown("Hammer") && hammerTimer <= 0 && damageTimer <= 0)
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
            Damage(Time.deltaTime * toxicDps);
            gas += Time.deltaTime * gasPerSec;
        }
    }

    private void UpdateGraple()
    {
        if (currentSickle == null)
        {
            if (grapleObj.Count > 0)
            {
                foreach (GameObject obj in grapleObj)
                {
                    Destroy(obj);
                }
                grapleObj.Clear();
            }
        } else
        {
            if ((transform.position - currentSickle.transform.position).magnitude > 10)
            {
                currentSickle = null;
            } else
            {
                if (grapleObj.Count == 0)
                {

                    for (int i = 0; i < grapleNumber; i++)
                    {
                        var graple = GameObject.Instantiate(graplePrefab);
                        grapleObj.Add(graple);
                    }
                }

                var angle = Mathf.Atan2(currentSickle.transform.position.y - transform.position.y, currentSickle.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

                for (int i = 0; i < grapleNumber; i++)
                {
                    var graple = grapleObj[i];
                    graple.transform.position = Vector3.Lerp(transform.position, currentSickle.transform.position, i / (float)grapleNumber);
                    graple.transform.rotation = Quaternion.Euler(0, 0, angle + 90);
                    graple.transform.localScale = new Vector3(graple.transform.localScale.x, (transform.position - currentSickle.transform.position).magnitude / 10, graple.transform.localScale.z); ;
                }
            }
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            damageTaken = collision.gameObject;
            Damage(5);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            toxic--;
        }

        if (collision.gameObject.CompareTag("DeathCloud"))
        {
            Damage(100);
        }
    }

    private void Damage(float damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            Destroy(gameObject);
        }
    }

    public int GetHP()
    {
        return Mathf.CeilToInt(hp);
    }

    public int GetGas()
    {
        return Mathf.CeilToInt(gas);
    }


}
