using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InputUIController : MonoBehaviour{

    public PlayerController playerController;
    public LaunchArcMesh lanchArcMesh;
    public Slider slider;

    public GameObject sliderOBJ;
    public GameObject hendle;
    public GameObject player;
    public GameObject inputUITriangle;
    public GameObject inputUIcircle;

    private float sliderValue;
    public float dragRatio;
    public int rotDragPosYAdjValue;
    private bool dragEnd;

    public bool drag;

    static int s_ReadyHash = Animator.StringToHash("Ready");
    static int s_ReadySpeedHash = Animator.StringToHash("ReadySpeed");


    public static event Action OnDragEnd = delegate { };


    /*
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    */

    public bool DragEnd
    {
        get
        {
            return dragEnd;
        }

        set
        {
            dragEnd = value;
            if (dragEnd)
            {
                OnDragEnd();
                dragEnd = false;
                playerController.jumpEnable = false;
            }
        }
    }

    private void Update()
    {
        if (playerController.jumpEnable)
        {
            inputUIcircle.SetActive(true);
            hendle.SetActive(true);
            sliderOBJ.SetActive(true);

            if(drag)
            {
                inputUITriangle.SetActive(true);
                player.transform.rotation = Quaternion.Euler(new Vector3(0, 
                        Mathf.Clamp(-((Input.mousePosition.x - Screen.width / 2) * -(200 / (Input.mousePosition.y - (Screen.width / 2 + rotDragPosYAdjValue)))) * dragRatio, -25f, 25f), 
                        0));
            }
        }
        else
        {
            drag = false;
            slider.value = 0.0f;
            inputUIcircle.SetActive(false);
            inputUITriangle.SetActive(false);
            hendle.SetActive(false);
            sliderOBJ.SetActive(false);
        }
        inputUITriangle.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
    }

    public void OnDraging()
    {
        playerController.character.animator.SetBool(s_ReadyHash, true);
        playerController.character.animator.SetFloat(s_ReadySpeedHash, sliderValue);

        drag = true;

        //StartCoroutine(lanchArcMesh.StartMakeArcMesh());
        lanchArcMesh.gameObject.SetActive(true);
    }

    public void EndDraging()
    {
        playerController.character.animator.SetBool(s_ReadyHash, false);

        DragEnd = true;
        drag = false;

        //StopCoroutine(lanchArcMesh.StartMakeArcMesh());
        lanchArcMesh.gameObject.SetActive(false);

    }

    public void UpdateSliderValue()
    {
        sliderValue = slider.value;
    }
}
