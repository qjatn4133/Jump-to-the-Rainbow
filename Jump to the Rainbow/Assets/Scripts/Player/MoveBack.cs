using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBack : MonoBehaviour {

    public float backForce;
    public GameObject player;
    Vector3 playerPosition;

    private void Update()
    {
        playerPosition = player.transform.position;
    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.attachedRigidbody.velocity = Vector3.zero;
            other.attachedRigidbody.AddForce(transform.parent.position - other.transform.position * backForce);
        }
    }
    

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.attachedRigidbody.velocity = Vector3.zero;
            other.attachedRigidbody.AddForce(transform.parent.position - other.transform.position * backForce);
        }
    }
*/
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.rigidbody.velocity = Vector3.zero;
            collision.rigidbody.AddForce(transform.parent.position - playerPosition * backForce, ForceMode.VelocityChange);
            Debug.Log(transform.parent.name);
        }
    }

    /*
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.rigidbody.velocity = Vector3.zero;
            collision.rigidbody.AddForce(parent.transform.position - collision.transform.position * backForce, ForceMode.Impulse);
        }
    }
    */
}
