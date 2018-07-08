using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
using UnityEngine.Analytics.Experimental;
#endif

/// <summary>
/// Pushed on top of the GameManager during gameplay. Takes care of initializing all the UI and start the TrackManager
/// Also will take care of cleaning when leaving that state.
/// </summary>
public class GameState : AState
{
    
    private bool endGame;
    
    public static event Action OnEndGame = delegate { };

    // WaterLily 초기화에 사용
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
    
    //static int s_DeadHash = Animator.StringToHash("Dead");

    public Canvas canvas;
    //public TrackManager trackManager;
    public StageManager stageManager;
    public PlayerController playerController;

    public AudioClip gameTheme;

    [Header("UI")]
    public Text starText;
    public Text coinText;
    public Text scoreText;
    public Slider slider;
    public float decreaseSpeed;

    //public Text distanceText;
    //public Text multiplierText;
    //public Text countdownText;
    //public RectTransform powerupZone;
    //public RectTransform lifeRectTransform;

    public RectTransform pauseMenu;
    public RectTransform wholeUI;
    public Button pauseButton;

    //public Image inventoryIcon;

    public GameObject gameOverPopup;
    public Button coinForLifeButton;
    public GameObject adsForLifeButton;
    public Text premiumCurrencyOwned;

    [Header("Korean Language")]
    public Text highScoreText;
    public Text pauseTitle;
    public Text pauseExitTitle;
    public Text pauseExitDesc;
    public Text deathPopupTitle;
    public Text deathPopupExit;

    [Header("Prefabs")]
    //public GameObject PowerupIconPrefab;

    public Modifier currentModifier = new Modifier();

    public string adsPlacementId = "rewardedVideo";
#if UNITY_ANALYTICS
    public AdvertisingNetwork adsNetwork = AdvertisingNetwork.UnityAds;
#endif
    public bool adsRewarded = true;

    protected bool m_Finished;
    //protected float m_TimeSinceStart;
    //protected List<PowerupIcon> m_PowerupIcons = new List<PowerupIcon>();
    protected Image[] m_LifeHearts;

    protected RectTransform m_CountdownRectTransform;
    //protected bool m_WasMoving;

    protected bool m_AdsInitialised = false;
    protected bool m_GameoverSelectionDone = false;

    protected int k_MaxLives = 3;

    public override void Enter(AState from)
    {
        /*
        m_CountdownRectTransform = countdownText.GetComponent<RectTransform>();

        m_LifeHearts = new Image[k_MaxLives];
        for (int i = 0; i < k_MaxLives; ++i)
        {
            m_LifeHearts[i] = lifeRectTransform.GetChild(i).GetComponent<Image>();
        }
        */
        // !MusicPlayer
        if (MusicPlayer.instance.GetStem(0) != gameTheme)
        {
            MusicPlayer.instance.SetStem(0, gameTheme);
            CoroutineHandler.StartStaticCoroutine(MusicPlayer.instance.RestartAllStems());
        }
        
        m_AdsInitialised = false;
        m_GameoverSelectionDone = false;
        StartGame();
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);

        //ClearPowerup();
    }

    public void StartGame()
    {

        canvas.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        wholeUI.gameObject.SetActive(true);
        pauseButton.gameObject.SetActive(true);
        gameOverPopup.SetActive(false);

        if (!PlayerData.instance.koreanCheck)
        {
            if (PlayerData.instance.highscores.Count != 0)
            {
                highScoreText.text = "HighScore : " + PlayerData.instance.highscores[0].score.ToString() + "m";
            }
            else
            {
                highScoreText.text = "HighScore : 0m";
            }

            pauseTitle.text = "PAUSE";
            pauseExitTitle.text = "Really?";
            pauseExitDesc.text = "If you go out on the way, you will lose the stars and coins from the game.";
            deathPopupTitle.text = "CONTINUE?";
            deathPopupExit.text = "MAIN MENU";
        }
        else
        {
            if (PlayerData.instance.highscores.Count != 0)
            {
                highScoreText.text = "최고기록 : " + PlayerData.instance.highscores[0].score.ToString() + "m";
            }
            else
            {
                highScoreText.text = "최고기록 : 0m";
            }

            pauseTitle.text = "일시 정지";
            pauseExitTitle.text = "정말?";
            pauseExitDesc.text = "도중에 나가면 게임에서 얻은 별과 코인을 잃게 됩니다.";
            deathPopupTitle.text = "이어서 하시겠습니까?";
            deathPopupExit.text = "나가기";
        }

        Time.timeScale = 1;

        /*
        if (!trackManager.isRerun)
        {
            m_TimeSinceStart = 0;
            trackManager.characterController.currentLife = trackManager.characterController.maxLife;
        }
        */

        currentModifier.OnRunStart(this);
        stageManager.Begin();
        m_Finished = false;

        //m_PowerupIcons.Clear();
    }

    public override string GetName()
    {
        return "Game";
    }

    public override void Tick()
    {
        if (m_Finished)
        {
            //if we are finished, we check if advertisement is ready, allow to disable the button until it is ready
#if UNITY_ADS
            if (!m_AdsInitialised && Advertisement.IsReady(adsPlacementId))
            {
                adsForLifeButton.SetActive(true);
                m_AdsInitialised = true;
#if UNITY_ANALYTICS
                AnalyticsEvent.AdOffer(adsRewarded, adsNetwork, adsPlacementId, new Dictionary<string, object>
            {
                { "level_index", PlayerData.instance.rank },
                { "distance", StageManager.instance == null ? 0 : StageManager.instance.playerPosZ },
            });
#endif
            }
            else if(!m_AdsInitialised)
                adsForLifeButton.SetActive(false);
#else
            adsForLifeButton.SetActive(false); //Ads is disabled
#endif

            return;
        }

        //CharacterInputController chrCtrl = trackManager.characterController;
        //PlayerController chrCtrl = playerController;

        //m_TimeSinceStart += Time.deltaTime;

        /*
        if (chrCtrl.Life <= 0)
        {
            Die();
        }
        */
        /*
        // Consumable ticking & lifetime management
        List<Consumable> toRemove = new List<Consumable>();
        List<PowerupIcon> toRemoveIcon = new List<PowerupIcon>();

        for (int i = 0; i < chrCtrl.consumables.Count; ++i)
        {
            PowerupIcon icon = null;
            for (int j = 0; j < m_PowerupIcons.Count; ++j)
            {
                if (m_PowerupIcons[j].linkedConsumable == chrCtrl.consumables[i])
                {
                    icon = m_PowerupIcons[j];
                    break;
                }
            }

            chrCtrl.consumables[i].Tick(chrCtrl);
            if (!chrCtrl.consumables[i].active)
            {
                toRemove.Add(chrCtrl.consumables[i]);
                toRemoveIcon.Add(icon);
            }
            else if (icon == null)
            {
                // If there's no icon for the active consumable, create it!
                GameObject o = Instantiate(PowerupIconPrefab);
                icon = o.GetComponent<PowerupIcon>();

                icon.linkedConsumable = chrCtrl.consumables[i];
                icon.transform.SetParent(powerupZone, false);

                m_PowerupIcons.Add(icon);
            }
        }

        for (int i = 0; i < toRemove.Count; ++i)
        {
            toRemove[i].Ended(trackManager.characterController);

            Destroy(toRemove[i].gameObject);
            if (toRemoveIcon[i] != null)
                Destroy(toRemoveIcon[i].gameObject);

            chrCtrl.consumables.Remove(toRemove[i]);
            m_PowerupIcons.Remove(toRemoveIcon[i]);
        }
        */
        UpdateUI();

        currentModifier.OnRunTick(this);
    }


    public void Die()
    {
        pauseButton.gameObject.SetActive(false);

        //StartCoroutine(DieAnimationPlay());
        //playerController.moveDirection = Vector3.zero;

        //chrCtrl.CleanConsumable();
        //chrCtrl.characterCollider.koParticle.gameObject.SetActive(true);
    }

    /*
    IEnumerator DieAnimationPlay()
    {
        //yield return new WaitUntil(() => playerController.jumpEnable == true);

        if (!playerController.jumpEnable)
        {
            yield return null;

        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(WaitForGameOver());
        }

    }
    */

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) Pause();
    }

    public void Pause()
    {
        //check if we aren't finished OR if we aren't already in pause (as that would mess states)
        if (m_Finished || AudioListener.pause == true)
            return;

        AudioListener.pause = true;
        Time.timeScale = 0;

        pauseButton.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        wholeUI.gameObject.SetActive(false);
        //m_WasMoving = trackManager.isMoving;
        //trackManager.StopMove();
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        pauseButton.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        wholeUI.gameObject.SetActive(true);
        /*
        if (m_WasMoving)
        {
            trackManager.StartMove(false);
        }
        */

        AudioListener.pause = false;
    }

    public void QuitToLoadout()
    {
        // Used by the pause menu to return immediately to loadout, canceling everything.
        Time.timeScale = 1.0f;
        AudioListener.pause = false;
        stageManager.End();

        stageManager.isRerun = false;
        EndGame = true;

        manager.SwitchState("Loadout");
    }

    protected void UpdateUI()
    {
        starText.text = " x " + playerController.Star.ToString();
        coinText.text = " x " + playerController.Coin.ToString();

        /*
        for (int i = 0; i < 3; ++i)
        {

            if (trackManager.characterController.currentLife > i)
            {
                m_LifeHearts[i].color = Color.white;
            }
            else
            {
                m_LifeHearts[i].color = Color.black;
            }
        }
        */
        
        scoreText.text = Mathf.FloorToInt(playerController.transform.position.z).ToString() + "m";

        if (playerController.Life > 0)
        {
            if (playerController.Life <= 1)
            {
                playerController.Life -= (decreaseSpeed / 1.5f) * Time.deltaTime;
            }
            else
            {
                playerController.Life -= decreaseSpeed * Time.deltaTime;
            }
        }
        slider.value = playerController.Life;

        //multiplierText.text = "x " + trackManager.multiplier;
        //distanceText.text = Mathf.FloorToInt(trackManager.worldDistance).ToString() + "m";

        /*
        if (trackManager.timeToStart >= 0)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = Mathf.Ceil(trackManager.timeToStart).ToString();
            m_CountdownRectTransform.localScale = Vector3.one * (1.0f - (trackManager.timeToStart - Mathf.Floor(trackManager.timeToStart)));
        }
        else
        {
            m_CountdownRectTransform.localScale = Vector3.zero;
        }
        */

        // Consumable
        /*
        if (trackManager.characterController.inventory != null)
        {
            inventoryIcon.transform.parent.gameObject.SetActive(true);
            inventoryIcon.sprite = trackManager.characterController.inventory.icon;
        }
        else
            inventoryIcon.transform.parent.gameObject.SetActive(false);
            */
    }

    public IEnumerator WaitForGameOver()
    {
        endGame = true;

        m_Finished = true;
        //playerController.WaitForGameOver();
        //trackManager.StopMove();

        // Reseting the global blinking value. Can happen if game unexpectly exited while still blinking
        //Shader.SetGlobalFloat("_BlinkingValue", 0.0f);

        yield return new WaitForSeconds(0.5f);

        if (currentModifier.OnRunEnd(this))
        {
            if (stageManager.isRerun)
            {
                GameOver();
                //manager.SwitchState("GameOver");

            }
            else
                OpenGameOverPopup();
        }
    }

    /*
    protected void ClearPowerup()
    {
        for (int i = 0; i < m_PowerupIcons.Count; ++i)
        {
            if (m_PowerupIcons[i] != null)
                Destroy(m_PowerupIcons[i].gameObject);
        }

        m_PowerupIcons.Clear();
    }
    */

    public void OpenGameOverPopup()
    {
        coinForLifeButton.interactable = PlayerData.instance.coins + playerController.Coin >= 3;

        premiumCurrencyOwned.text = (PlayerData.instance.coins + playerController.Coin).ToString();

        //ClearPowerup();

        gameOverPopup.SetActive(true);
    }

    public void GameOver()
    {
        manager.SwitchState("GameOver");
    }

    public void PremiumForLife()
    {
        //This check avoid a bug where the video AND premium button are released on the same frame.
        //It lead to the ads playing and then crashing the game as it try to start the second wind again.
        //Whichever of those function run first will take precedence
        if (m_GameoverSelectionDone)
            return;

        m_GameoverSelectionDone = true;

        if(playerController.Coin >= 3)
        {
            playerController.Coin -= 3;
        }
        else
        {
            PlayerData.instance.coins -= 3;
        }

        SecondWind();
    }

    public void SecondWind()
    {
        //trackManager.characterController.currentLife = 1;
        playerController.SecondWind();
        stageManager.isRerun = true;
        StartGame();
    }

    public void ShowRewardedAd()
    {
        if (m_GameoverSelectionDone)
            return;

        m_GameoverSelectionDone = true;

#if UNITY_ADS
        if (Advertisement.IsReady(adsPlacementId))
        {
#if UNITY_ANALYTICS
            AnalyticsEvent.AdStart(adsRewarded, adsNetwork, adsPlacementId, new Dictionary<string, object>
            {
                { "level_index", PlayerData.instance.rank },
                { "distance", StageManager.instance == null ? 0 : StageManager.instance.playerPosZ },
            });
#endif
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show(adsPlacementId, options);
        }
        else
        {
#if UNITY_ANALYTICS
            AnalyticsEvent.AdSkip(adsRewarded, adsNetwork, adsPlacementId, new Dictionary<string, object> {
                { "error", Advertisement.GetPlacementState(adsPlacementId).ToString() }
            });
#endif
        }
#else
        GameOver();
#endif
    }

    //=== AD
#if UNITY_ADS

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
#if UNITY_ANALYTICS
                AnalyticsEvent.AdComplete(adsRewarded, adsNetwork, adsPlacementId);
#endif
                SecondWind();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
#if UNITY_ANALYTICS
                AnalyticsEvent.AdSkip(adsRewarded, adsNetwork, adsPlacementId);
#endif
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
#if UNITY_ANALYTICS
                AnalyticsEvent.AdSkip(adsRewarded, adsNetwork, adsPlacementId, new Dictionary<string, object> {
                    { "error", "failed" }
                });
#endif
                break;
        }
    }
#endif
}