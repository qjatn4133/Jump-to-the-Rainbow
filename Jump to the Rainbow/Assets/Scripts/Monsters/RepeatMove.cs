using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatMove : MonoBehaviour {

    public float moveXDistance;
    public float moveYDistance;
    public float moveZDistance;

    public float moveXSpeed;
    public float moveYSpeed;
    public float moveZSpeed;


    Vector3 firstPosition;

	void Start () {
        firstPosition = transform.position;
	}

    void Update()
    {

        if (transform.position.x >= Mathf.Abs(firstPosition.x + moveXDistance))
        {
            moveXSpeed *= -1;
        }

        if (transform.position.y >= Mathf.Abs(firstPosition.y + moveYDistance))
        {
            moveYSpeed *= -1;
        }

        if (transform.position.z >= Mathf.Abs(firstPosition.z + moveZDistance))
        {
            moveZSpeed *= -1;
        }

        if (transform.position.x <= -Mathf.Abs(firstPosition.x + moveXDistance))
        {
            moveXSpeed *= -1;
        }

        if (transform.position.y <= -Mathf.Abs(firstPosition.y + moveYDistance))
        {
            moveYSpeed *= -1;
        }

        if (transform.position.z <= -Mathf.Abs(firstPosition.z + moveZDistance))
        {
            moveZSpeed *= -1;
        }

        transform.position = new Vector3(
            transform.position.x + moveXSpeed * Time.deltaTime, 
            transform.position.y + moveYSpeed * Time.deltaTime, 
            transform.position.z + moveZSpeed * Time.deltaTime);
    }

    }
