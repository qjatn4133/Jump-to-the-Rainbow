using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake_head : CollidingObject {

    public GameObject snake;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
            Destroy(snake);
    }


}
