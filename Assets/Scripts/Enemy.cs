using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator animator;
    protected Rigidbody2D rb;
    protected AudioSource death;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        death = GetComponent<AudioSource>();

    }

    public void JumpedOn()
    {
        animator.SetTrigger("Death");
        death.Play();

    }

    private void Death()
    {
        Destroy(this.gameObject);
    }
}
