using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beaver : CollidingObject {

    Animator animator;

    [SerializeField]
    private float rotateZSpeed = 50.0f;

    [SerializeField]
    private float rotationZDegree = 40.0f;
    private bool coStart;

    private void Awake()
    {

        animator = GetComponent<Animator>();
    }
    /*
    void Update () {

        transform.rotation = Quaternion.Euler(new Vector3(0, 180, Mathf.PingPong((Time.time) * rotateZSpeed, rotationZDegree * 2) - rotationZDegree));
    }
    */
    IEnumerator Enable()
    {
        coStart = true;

        while (coStart == true)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, Mathf.PingPong((Time.time) * rotateZSpeed, rotationZDegree * 2) - rotationZDegree));
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
            animator.SetBool("Collision", true);
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (IsMonsterTrigger(other))
        {
            coStart = false;
        }

        if (IsPlayer(other))
        {
            animator.SetBool("Collision", false);
        }
    }
}
