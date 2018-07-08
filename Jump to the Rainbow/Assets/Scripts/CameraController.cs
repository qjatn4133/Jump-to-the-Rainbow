using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    //public GameManager gameManager;
    //public PlayerController playerController;
    public Transform player;
    public StageManager stageManager;
    public TutorialManager tutorialManager;
    public Vector3 initPosition;
    public float initRotationX;
    public float ingameRotationX;
    public Vector3 positionAdjustment;
    public float followSpeed;
    private bool reset;
    private bool inGame;
    private bool playerOut;
    private Vector3 currentPos;

    // loadout.enter에서 실행
    public void InLoadout()
    {
        inGame = false;
        StopAllCoroutines();
        //StopCoroutine(InGame());
        //StopCoroutine(WaitingPlayer(0));
        playerOut = false;

        transform.position = initPosition;
        transform.rotation = Quaternion.Euler(new Vector3(initRotationX, 0, 0));
    }

    public IEnumerator WaitingPlayer(float T)
    {
        //StopCoroutine(InGame());

        currentPos = transform.position;
        //transform.position = currentPos;

        playerOut = true;
        yield return new WaitForSeconds(T);
        playerOut = false;
        //StartCoroutine(Init());
        //StartCoroutine(InGame());
    }

    
    public IEnumerator Init()
    {
        reset = true;

        yield return new WaitForEndOfFrame();
        reset = false;
        transform.position = player.transform.position + positionAdjustment;

        //StartCoroutine(InGame());

    }
    

    // PlayerController.Begin()에서 시작
    public void Begin()
    {

        player = stageManager.playerController.characterSlot;
        inGame = true;
        transform.rotation = Quaternion.Euler(new Vector3(ingameRotationX, 0, 0));
        //StartCoroutine(InGame());
    }

    public void TutorialBegin()
    {

        player = tutorialManager.playerController.characterSlot;
        inGame = true;
        transform.rotation = Quaternion.Euler(new Vector3(ingameRotationX, 0, 0));
        //StartCoroutine(InGame());
    }

    void LateUpdate () {
        if (inGame)
        {
            if (reset)
            {
                transform.position = player.transform.position + positionAdjustment;
            }
            else
            {
                if (playerOut)
                {
                    transform.position = currentPos;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, player.transform.position + positionAdjustment, Time.deltaTime * followSpeed);
                }
            }
        }
    }
    
}
