using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepPosition : MonoBehaviour {

    public Transform player;
    public float distance;

    private bool inGame = false;

    public void IsGame(bool isGame)
    {
        inGame = isGame;
    }

    void Update () {

        if(inGame)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z + distance);
        }
    }
}
