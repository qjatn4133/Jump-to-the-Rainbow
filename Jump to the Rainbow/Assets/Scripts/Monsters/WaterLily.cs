using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterLily : CollidingObject {

    public float moveXDistance;
    public float moveXSpeed;
    private bool endGame;
    private bool playerLeave;
    private float increaseT;
    Vector3 firstPosition;
    public float initialMoveXSpeed;
    private bool coStart;

    private void Awake()
    {
        PlayerController.EndGame += EndGame;
        PlayerController.StartGame += StartGame;
    }

    void EndGame()
    {
        endGame = true;
    }

    void StartGame()
    {
        endGame = false;

    }
    
    void Start () {
        firstPosition = transform.position;
        initialMoveXSpeed = moveXSpeed;
    }

    /*
    void Update () {

        float t = Time.deltaTime;

        if(!playerLeave && !endGame)
        {
            increaseT += t;

            transform.position = new Vector3(Mathf.PingPong(increaseT * moveXSpeed, moveXDistance * 2) + (firstPosition.x - moveXDistance), transform.position.y, transform.position.z);
        }
        else
        {

            float tx = increaseT;
            increaseT = tx;

            float x = transform.position.x;
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

    }
    */
    IEnumerator Enable()
    {
        coStart = true;

        while (coStart == true)
        {
            float t = Time.deltaTime;

            if (!playerLeave && !endGame)
            {
                increaseT += t;

                transform.position = new Vector3(Mathf.PingPong(increaseT * moveXSpeed, moveXDistance * 2) + (firstPosition.x - moveXDistance), transform.position.y, transform.position.z);
            }

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
            if (!endGame)
            {
                playerLeave = false;
                moveXSpeed = initialMoveXSpeed;
            }
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
            playerLeave = true;
            moveXSpeed = 0;
        }
    }
}
