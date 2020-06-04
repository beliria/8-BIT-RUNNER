using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : Enemy
{
    [SerializeField]private float leftCap;
    [SerializeField]private float rightCap;

    [SerializeField] private float jumpLength;
    [SerializeField] private float jumpHeight;
    [SerializeField] private LayerMask ground;

    private bool facingLeft = true;

    private Collider2D coll;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        coll    = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("jumping"))
        {
            if (rb.velocity.y < .1)
            {
                animator.SetBool("falling", true);
                animator.SetBool("jumping", false);
            }
        }

        if (coll.IsTouchingLayers(ground) && animator.GetBool("falling"))
        {
            animator.SetBool("falling", false);
        }
    }

    private void Move()
    {
        if (facingLeft)
        {
            //si estamos dentro del rango
            if (transform.position.x > leftCap)
            {   //canviar direccion si no esta mirando en la direccion correcta
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    animator.SetBool("jumping", true);
                }

            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            //si estamos dentro del rango
            if (transform.position.x < rightCap)
            {   //canviar direccion si no esta mirando en la direccion correcta
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }

                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    animator.SetBool("jumping", true);

                }
            }
            else
            {
                facingLeft = true;

            }
        }
    }



}
