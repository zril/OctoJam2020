using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sickle : MonoBehaviour
{
    public float Direction;

    private float speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dist = speed * Time.deltaTime;
        transform.position += new Vector3(Mathf.Cos(Mathf.Deg2Rad * Direction) * dist, Mathf.Sin(Mathf.Deg2Rad * Direction) * dist, 0);
    }
}
