using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWalk : MonoBehaviour
{
    private bool right;
    private float speed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
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


}
