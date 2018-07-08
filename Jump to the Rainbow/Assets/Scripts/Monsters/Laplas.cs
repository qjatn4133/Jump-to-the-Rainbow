using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laplas : CollidingObject
{

    [SerializeField]
    private float timer = 1.5f;
    [SerializeField]
    private float throwForce = 10f;
    [SerializeField]
    private float yOffset = 3f;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            //other.transform.root.GetComponent<PlayerController>().Life--;

            if (!other.transform.root.GetComponent<PlayerController>().dieFlag)
            {
                StartCoroutine(CheckTime(other.transform.root));

            }
            else
                return;

        }
    }

    IEnumerator CheckTime(Transform characterTransform)
    {

        if (characterTransform.GetComponent<PlayerController>().dieFlag)
        {
            characterTransform.GetComponent<PlayerController>().moveDirection = Vector3.zero;
            StopAllCoroutines();
            yield break;
        }
        //float life = characterTransform.GetComponent<PlayerController>().Life;
        //characterTransform.GetComponent<PlayerController>().Life = life;
        animator.SetTrigger("Eat");
        characterTransform.GetComponent<PlayerController>().isMovable = false;
        characterTransform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
        float i = 0;
        while (i <= timer)
        {
            i += Time.deltaTime;
            yield return null;
        }
        animator.SetTrigger("Spit");
        var moveDirection = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)) * throwForce;
        characterTransform.GetComponent<PlayerController>().FinishedUnmovableTime(moveDirection);

    }
}
