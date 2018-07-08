using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alocasia : CollidingObject
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("Exit", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            animator.SetTrigger("Enter");
            animator.SetBool("Exit", true);
            other.transform.root.GetComponent<PlayerController>().BounceFromAlocasia(transform.localEulerAngles.y);
        }
    }
}
