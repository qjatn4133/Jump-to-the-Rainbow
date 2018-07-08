using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : CollidingObject {

    public float rotateSpeed;
    private bool coStart;

    /*
	void Update () {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
    */
    /*
    IEnumerator Enable()
    {
        while (true)
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
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
            coStart = true;
        }
    }
    */
}
