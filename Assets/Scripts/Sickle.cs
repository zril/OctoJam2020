using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sickle : MonoBehaviour
{
    public float Direction;

    private float speed = 20;
    private float rotSpeed = 5;

    private bool stick = false;

    public AudioClip impactClip;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stick)
        {
            var dist = speed * Time.deltaTime;
            transform.position += new Vector3(Mathf.Cos(Mathf.Deg2Rad * Direction) * dist, Mathf.Sin(Mathf.Deg2Rad * Direction) * dist, 0);
        }

        if (!stick)
        {
            transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotSpeed * 360));
        }
    }

    public void Stick(GameObject target)
    {
        if (!stick)
        {
            stick = true;
            transform.parent = target.transform;
            audioSource.PlayOneShot(impactClip);
        }
    }

    public bool GetStick()
    {
        return stick;
    }
}
