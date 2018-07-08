using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarSide : MonoBehaviour {

    public float rotateSpeed;
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime);
    }
}
