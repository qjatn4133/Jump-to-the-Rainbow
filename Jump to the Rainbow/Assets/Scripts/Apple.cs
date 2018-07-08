using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : CollidingObject {

    public float moveYSpeed;
    public float moveYDistance;
    private float startPosition;
    private bool coStart;

    private void Awake()
    {
        startPosition = transform.position.y;
    }
    /*
    void Update () {
        transform.position = new Vector3(transform.position.x,
    Mathf.PingPong(Time.time * moveYSpeed, moveYDistance) + startPosition,
    transform.position.z);
    }
    */
    IEnumerator Enable()
    {
        while (true)
        {
            transform.position = new Vector3(transform.position.x,
        Mathf.PingPong(Time.time * moveYSpeed, moveYDistance) + startPosition,
        transform.position.z); yield return null;
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
            coStart = true;
        }
    }
}
