using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catfish : CollidingObject {

    Animator animator;

    public float moveYDistance;
    public float moveYSpeed;
    private Vector3 startPosition;
    private bool coStart;

    void Start () {
        animator = GetComponent<Animator>();
        animator.SetBool("Wriggle", false);
        startPosition = transform.position;

    }

    /*
    void Update () {

        transform.position = new Vector3(transform.position.x, 
            Mathf.PingPong((Time.time) * moveYSpeed, moveYDistance * 2) + startPosition.y,
            transform.position.z);
    }
    */

    IEnumerator Enable()
    {
        coStart = true;

        while (coStart == true)
        {
            transform.position = new Vector3(transform.position.x, Mathf.PingPong((Time.time) * moveYSpeed, moveYDistance * 2) + startPosition.y, transform.position.z);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!coStart)
        {
            if (IsMonsterTrigger(other))
            {
                StartCoroutine(Enable());
            }
        }

        if (IsPlayer(other))
        {
            animator.SetTrigger("Eat");
            animator.SetBool("Wriggle", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsMonsterTrigger(other))
        {
            coStart = false;
        }
    }
}
