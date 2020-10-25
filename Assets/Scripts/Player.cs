using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private float speed = 2;
    private float basejumpspeed = 6;
    private float holdjumpfactor = 4f;

    private float hp = 100;
    private float maxHp = 100;
    private float gas = 10;
    private int maxGas = 10;
    private float toxicDps = 2.5f;
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
    private GameObject gun;
    private GameObject hammer;

    public AudioClip shootClip;
    public AudioClip jumpClip;
    public AudioClip hammerClip;
    public AudioClip looseClip;
    private AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        sicklePrefab = Resources.Load<GameObject>("Prefabs/Sickle");
        graplePrefab = Resources.Load<GameObject>("Prefabs/Graple");
        gun = transform.Find("Gun").gameObject;
        hammer = transform.Find("Hammer").gameObject;
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();


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
                rigidbody.velocity = new Vector2(vel.x - 3, vel.y + 1);
            } else
            {
                rigidbody.velocity = new Vector2(vel.x + 3, vel.y + 1);
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
            GetComponent<SpriteRenderer>().color = Color.red;
        } else
        {
            GetComponent<SpriteRenderer>().color = Color.white;
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
            audioSource.PlayOneShot(jumpClip);
        }

        //todo buffer jump

        // sickle
        Vector3 mousepos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousepos.z = 0;

        var angle = Mathf.Atan2(mousepos.y - transform.position.y, mousepos.x - transform.position.x) * Mathf.Rad2Deg;
        if (mousepos.x - transform.position.x > 0 && grapleTimer <= 0)
        {
            gun.GetComponent<SpriteRenderer>().flipX = false;
            gun.transform.rotation = Quaternion.Euler(0, 0, angle);
            gun.transform.Find("Sickle").localPosition = new Vector3(0.642f, 0.05f, 0);
            gun.transform.Find("Sickle").localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (mousepos.x - transform.position.x < 0 && grapleTimer <= 0)
        {
            gun.GetComponent<SpriteRenderer>().flipX = true;
            gun.transform.rotation = Quaternion.Euler(0, 0, angle + 180);
            gun.transform.Find("Sickle").localPosition = new Vector3(-0.642f, -0.05f, 0);
            gun.transform.Find("Sickle").localRotation = Quaternion.Euler(0, 0, 180);
        }

        if (sickleCooldownTimer > 0)
        {
            sickleCooldownTimer -= Time.deltaTime;
            gun.transform.Find("Sickle").gameObject.SetActive(false);
        } else
        {
            gun.transform.Find("Sickle").gameObject.SetActive(true);
        }

        if (Input.GetButtonDown("Sickle") && sickleCooldownTimer <= 0 && damageTimer <= 0 && gas > 0 && grapleTimer <= 0)
        {
            var sickle = GameObject.Instantiate(sicklePrefab);
            sickle.gameObject.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            sickle.GetComponent<Sickle>().Direction = angle;

            sickleCooldownTimer = sickleCooldown;
            gas--;
            currentSickle = sickle;
            audioSource.PlayOneShot(shootClip);
        }
        if (Input.GetButtonUp("Sickle"))
        {
            if (currentSickle != null && grapleTimer <= 0 && currentSickle.GetComponent<Sickle>().GetStick() && (transform.position - currentSickle.transform.position).magnitude > 3)
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
                while (count > 20f * remaining)
                {
                    transform.position = grapleObj[0].transform.position;
                    Destroy(grapleObj[0]);
                    grapleObj.RemoveAt(0);
                    count--;
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
            hammer.GetComponent<SpriteRenderer>().flipX = false;
            hammer.transform.localPosition = new Vector3(1, 0, 0);
        }
        if (mousepos.x - transform.position.x < 0)
        {
            transform.Find("HammerHitbox").transform.localPosition = Vector3.left;
            GetComponent<SpriteRenderer>().flipX = true;
            hammer.GetComponent<SpriteRenderer>().flipX = true;
            hammer.transform.localPosition = new Vector3(-1, 0, 0);
        }

        if (Input.GetButtonDown("Hammer") && hammerTimer <= 0 && damageTimer <= 0)
        {
            hammerTimer = hammerCooldown;
            transform.Find("HammerHitbox").gameObject.SetActive(true);
            hammer.gameObject.SetActive(true);
            audioSource.PlayOneShot(hammerClip);
        }

        if (hammerTimer > 0)
        {
            hammerTimer -= Time.deltaTime;
            var time = (hammerCooldown - hammerTimer) / hammerDuration;
            if (mousepos.x - transform.position.x > 0)
            {
                hammer.transform.rotation = Quaternion.Euler(0, 0, 50 - 100 * time);
            } else
            {
                hammer.transform.rotation = Quaternion.Euler(0, 0, -50 + 100 * time);
            }

            if (hammerTimer < hammerCooldown - hammerDuration)
            {
                transform.Find("HammerHitbox").gameObject.SetActive(false);
                hammer.gameObject.SetActive(false);
            }
        }

        // damage
        if (toxic > 0)
        {
            Damage(Time.deltaTime * toxicDps);
            gas += Time.deltaTime * gasPerSec;
            if (gas > maxGas) gas = maxGas;
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

        if (collision.gameObject.CompareTag("DeathCloud"))
        {
            Damage(100);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            damageTaken = collision.gameObject;
            Damage(5);
        }

        if (collision.gameObject.CompareTag("BonusHP"))
        {
            hp += 3;
            if (hp > maxHp) hp = maxHp;
            Destroy(collision.gameObject, 0.15f);
        }

        if (collision.gameObject.CompareTag("BonusGas"))
        {
            gas += 1;
            if (gas > maxGas) gas = maxGas;
            Destroy(collision.gameObject, 0.15f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Cloud"))
        {
            toxic--;
        }

    }

    private void Damage(float damage)
    {
        hp -= damage;
        if (hp < 0)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(looseClip);
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
