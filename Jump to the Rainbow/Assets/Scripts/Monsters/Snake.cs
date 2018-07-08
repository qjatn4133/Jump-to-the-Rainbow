using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : CollidingObject {

    public GameObject body;
    public GameObject head;

    public float changeScaleVal;
    public float changeScaleSpeed;
    public float headMoveVal;
    public float headMoveSpeed;
    public float headPositionX;

    private Vector3 startScale;
    private bool coStart;

    private void Start()
    {
        startScale = transform.localScale;
    }

    /*
	// Update is called once per frame
	void Update () {
        body.transform.localScale = new Vector3(Mathf.PingPong(Time.time * changeScaleSpeed, changeScaleVal) + startScale.x,
            Mathf.PingPong(Time.time * changeScaleSpeed, changeScaleVal) + startScale.y,
            Mathf.PingPong(Time.time * changeScaleSpeed, changeScaleVal) + startScale.z);

        head.transform.position = new Vector3(Mathf.PingPong(Time.time * headMoveSpeed, headMoveVal + startPosition.x) - headMoveVal/2, head.transform.position.y, head.transform.position.z);

        if(head.transform.position.x >= headMoveVal/2 - 0.1)
        {
            head.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        if (head.transform.position.x <= -(headMoveVal / 2 - 0.1))
        {
            head.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    */

    IEnumerator Enable()
    {
        coStart = true;

        while (coStart == true)
        {
            body.transform.localScale = new Vector3(Mathf.PingPong(Time.time * changeScaleSpeed, changeScaleVal) + startScale.x,
    Mathf.PingPong(Time.time * changeScaleSpeed, changeScaleVal) + startScale.y,
    Mathf.PingPong(Time.time * changeScaleSpeed, changeScaleVal) + startScale.z);

            head.transform.position = new Vector3(Mathf.PingPong(Time.time * headMoveSpeed, headMoveVal) + headPositionX, head.transform.position.y, head.transform.position.z);

            if (head.transform.position.x >= headMoveVal / 2 - 0.1)
            {
                head.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            }

            if (head.transform.position.x <= -(headMoveVal / 2 - 0.1))
            {
                head.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
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

    }

    private void OnTriggerExit(Collider other)
    {
        if (IsMonsterTrigger(other))
        {
            coStart = false;
        }
    }
}
