using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public StageManager stageManager;
    public PlayerController playerController;

    Slider slider;
    public float decreaseSpeed;
    public bool inTutorial = false;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    void Update () {
        if(inTutorial)
        {
            if (playerController.Life > 3)
            {
                playerController.Life -= decreaseSpeed * Time.deltaTime;
            }
            else
            {
                playerController.Life = 3.5f;
            }

            slider.value = playerController.Life;

        }
        else
        {
            if (playerController.Life > 0)
            {
                playerController.Life -= decreaseSpeed * Time.deltaTime;
            }
            slider.value = playerController.Life;
        }

	}
}
