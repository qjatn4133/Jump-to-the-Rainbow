using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaOtter :CollidingObject {

    SpriteRenderer circle;
    Animator animator;

    [SerializeField]
    private float timer = 3.0f;

    [SerializeField]
    private float throwForce = 5f;

    private bool playerOut;
    static int s_HitHash = Animator.StringToHash("Hit");

    void Start () {
        animator = GetComponent<Animator>();
        circle = GetComponentInChildren<SpriteRenderer>();
        circle.color = new Color(0.9f, 0.7f, 0.57f, 0.7f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            playerOut = false;
            bool playerout = playerOut;
            animator.SetTrigger("Enter");
            StartCoroutine(CheckTime(playerout, other.transform.root));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            playerOut = true;
            circle.color = new Color(0.9f, 0.7f, 0.57f, 0.7f);
        }
    }
 
    
    IEnumerator CheckTime(bool PlayerOut, Transform characterTransform)
    {
        float i = 0;
        while (i <= timer)
        {
            if (playerOut)
            {
                circle.color = new Color(0.9f, 0.7f, 0.57f, 0.7f);
                animator.SetTrigger("Out");
                yield break;
            }
            i += Time.deltaTime;
            float lerp = Mathf.Lerp(0, timer, i) / (timer + 2);
            circle.color = Color.Lerp(new Color(0.9f, 0.7f, 0.57f, 0.7f), new Color(1.0f, 0.1f, 0.1f, 0.7f), lerp);
            yield return null;
        }
        Throw(characterTransform);
    }
    
    void Throw(Transform characterTransform)
    {
        animator.SetTrigger("Out");
        characterTransform.GetComponent<PlayerController>().character.animator.SetTrigger(s_HitHash);

        var moveDirection = new Vector3(Random.Range(-1.0f, 1.0f), 1, Random.Range(-1.0f, 1.0f)) * throwForce;
        characterTransform.GetComponent<PlayerController>().moveDirection = moveDirection;
    }
}
