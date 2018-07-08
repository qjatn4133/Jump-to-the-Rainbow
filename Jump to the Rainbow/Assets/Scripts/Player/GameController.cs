using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviour {
    
    public PlayerController player;
    public Text scoreLabel;
    public Text starCountLabel;
    public GameObject loadoutPanel;
    public GameObject gamePanel;
    public GameObject gameoverPanel;
    public GameObject stopButton;
    public GameObject stopPanel;
    public GameObject setupButton;
    public GameObject setupPanel;

    public static event Action OnEndGame = delegate { };

    public int score;
    public int starCount;

    private bool endGame;
    public bool EndGame
    {
        get
        {
            return endGame;
        }

        set
        {
            endGame = value;
            if (endGame)
            {
                OnEndGame();
                endGame = false;
            }
        }
    }

    private void Awake()
    {
        Time.timeScale = 0;
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(false);
        stopPanel.SetActive(false);
        setupPanel.SetActive(false);

    }

    public void Update()
    {
        // 스코어 레이블을 업데이트
        score = CalcScore();
        starCount = calcStarCount();

        scoreLabel.text = score + "m";
        starCountLabel.text = " X " + starCount;

        // 라이프 패널을 업데이트
        //lifePanel.UpdateLife(player.Life);
        //lifePanel.UpdateFeather(player.Feather);

        // 플레이어의 라이프가 0이 되면 게임 종료
        if (player.Life <= 0)
        {
            // 이후의 업데이트는 멈춘다
            enabled = false;

            // 하이 스코어를 업데이트
            if (PlayerPrefs.GetInt("HighScore") < score)
            {
                PlayerPrefs.SetInt("HighScore", score);
            }

            // Star 갯수를 업데이트
            if (PlayerPrefs.GetInt("StarCount") < score)
            {
                PlayerPrefs.SetInt("HighScore", score);
            }

            // 2초 후에 ReturnToTitle을 호출
            Invoke("GameOver", 0f);
        }
    }

    int CalcScore()
    {
        // 플레이어의 주행 거리를 스코어로 한다
        return (int)player.transform.position.z;
    }

    int calcStarCount()
    {
        // Star 갯수
        return (int)player.Star;

    }

    void GameOver()
    {
        player.enabled = false;
        player.transform.position = player.lastPosition;
        player.moveDirection = Vector3.zero;
        player.transform.eulerAngles = Vector3.zero;
        Time.timeScale = 0;
        stopButton.SetActive(false);
        gameoverPanel.SetActive(true);
    }

    public void StartGame()
    {
        loadoutPanel.SetActive(true);
        gamePanel.SetActive(true);
        stopButton.SetActive(true);

        enabled = true;
        player.Star = 0;
        player.Life = 10.0f;
        player.enabled = true;
        Time.timeScale = 1;

        loadoutPanel.SetActive(false);
    }

    public void Continue()
    {
        enabled = true;
        player.Life = 10.0f;
        player.enabled = true;
        Time.timeScale = 1;
        stopButton.SetActive(true);
        gameoverPanel.SetActive(false);
    }

    public void GiveUp()
    {
        gamePanel.SetActive(false);
        EndGame = true;
        gameoverPanel.SetActive(false);
        loadoutPanel.SetActive(true);
        player.transform.position = new Vector3(0, 1, 0);
    }

    public void Stop()
    {
        Time.timeScale = 0;
        stopButton.SetActive(false);
        stopPanel.SetActive(true);
    }

    public void Resume()
    {
        Time.timeScale = 1;
        stopButton.SetActive(true);
        stopPanel.SetActive(false);
    }

    public void SetUp()
    {
        setupButton.SetActive(false);
        setupPanel.SetActive(true);
    }

    public void SetUpExit()
    {
        setupButton.SetActive(true);
        setupPanel.SetActive(false);
    }
}
