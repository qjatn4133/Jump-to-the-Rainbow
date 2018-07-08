using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaOtter_body : CollidingObject {

    Animator animator;

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            animator.SetTrigger("Out");
        }
    }
}
