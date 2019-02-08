using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpPower;
    [SerializeField]
    private LayerName groundLayer;
    private int groundLayerMask;

    // Use this for initialization
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        groundLayerMask = LayerMask.GetMask(groundLayer.GetString());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float x = Input.GetAxisRaw("Horizontal");
        if (x != 0.0f)
        {
            rb2d.velocity = new Vector2(x * speed, rb2d.velocity.y);
        }
        else
        {
            rb2d.velocity = new Vector2(0, rb2d.velocity.y);
        }

        if (Physics2D.Linecast(transform.position,
            transform.position - transform.up * 1.0f,
            groundLayerMask))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
            }
        }
    }
}
