using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
using UnityEngine.Analytics.Experimental;
#endif
using System.Collections.Generic;

/// <summary>
/// state pushed on top of the GameManager when the player dies.
/// </summary>
public class GameOverState : AState
{
    public StageManager stageManager;
    public Canvas canvas;
    public MissionUI missionPopup;

    public Text starResult;
    public Text coinResult;

    public AudioClip gameOverTheme;

    //public Leaderboard miniLeaderboard;
    public Leaderboard fullLeaderboard;

    [Header("Korean Language")]
    public Text title;
    public Text scoreResult;
    public Text highScoreText;
    public Text retryButtonText;
    public Text exitButtonText;

    protected bool m_CoinCredited = false;

    public override void Enter(AState from)
    {
        canvas.gameObject.SetActive(true);

        //fullLeaderboard.playerEntry.inputName.text = PlayerData.instance.previousName;
        starResult.text = "x " + stageManager.playerController.deathData.stars.ToString();
        coinResult.text = "x " + stageManager.playerController.deathData.coins.ToString();

        if (!PlayerData.instance.koreanCheck)
        {
            title.text = "GAMEOVER";
            scoreResult.text = "SCORE : " + stageManager.playerController.deathData.score.ToString() + "m";

            if (PlayerData.instance.highscores.Count != 0)
            {
                highScoreText.text = "HighScore : " + PlayerData.instance.highscores[0].score.ToString() + "m";
            }
            else
            {
                highScoreText.text = "HighScore : 0m";
            }

            retryButtonText.text = "TRY AGAIN";
            exitButtonText.text = "MAIN MENU";
        }
        else
        {
            title.text = "게임 오버";
            scoreResult.text = "기록 : " + stageManager.playerController.deathData.score.ToString() + "m";

            if (PlayerData.instance.highscores.Count != 0)
            {
                highScoreText.text = "최고기록 : " + PlayerData.instance.highscores[0].score.ToString() + "m";
            }
            else
            {
                highScoreText.text = "최고기록 : 0m";
            }

            retryButtonText.text = "다시 하기";
            exitButtonText.text = "그만 두기";
        }

        fullLeaderboard.playerEntry.score.text = stageManager.playerPosZ.ToString();

        //miniLeaderboard.Populate();

        // ----- 구글 게임 서비스 업적 관련

        if (stageManager.playerController.deathData.score > 1000 && !PlayerPrefs.HasKey("A Passionate Frog"))
        {
            GPGSManager.Instance.APassionateFrog();
        }

        if (stageManager.playerController.deathData.score > 2000 && !PlayerPrefs.HasKey("A Extraordinary Quokka"))
        {
            GPGSManager.Instance.AExtraordinaryQuokka();
        }

        if (stageManager.playerController.deathData.stars > 50 && !PlayerPrefs.HasKey("A Proud Rabbit"))
        {
            GPGSManager.Instance.AProudRabbit();
        }

        // ----------------------------------------

        if (PlayerData.instance.AnyMissionComplete())
            missionPopup.Open();
        else
            missionPopup.gameObject.SetActive(false);

        m_CoinCredited = false;

        CreditCoins();

        if (MusicPlayer.instance.GetStem(0) != gameOverTheme)
        {
            MusicPlayer.instance.SetStem(0, gameOverTheme);
            StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
    }

    public override void Exit(AState to)
    {
        canvas.gameObject.SetActive(false);
        FinishRun();
    }

    public override string GetName()
    {
        return "GameOver";
    }

    public override void Tick()
    {

    }
    /*
    public void OpenLeaderboard()
    {
        fullLeaderboard.forcePlayerDisplay = false;
        fullLeaderboard.displayPlayer = true;
        fullLeaderboard.playerEntry.playerName.text = PlayerData.instance.previousName;
        fullLeaderboard.playerEntry.score.text = stageManager.playerPosZ.ToString();

        fullLeaderboard.Open();
    }

    
    public void GoToStore()
    {
        //UnityEngine.SceneManagement.SceneManager.LoadScene("shop", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
    */

    public void GoToLoadout()
    {
        stageManager.isRerun = false;
        manager.SwitchState("Loadout");
    }

    public void RunAgain()
    {
        stageManager.isRerun = false;
        manager.SwitchState("Game");
    }

    protected void CreditCoins()
    {
        if (m_CoinCredited)
            return;

        // -- give coins gathered
        PlayerData.instance.coins += stageManager.playerController.Coin;
        PlayerData.instance.stars += stageManager.playerController.Star;

        PlayerData.instance.Save();

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        var transactionId = System.Guid.NewGuid().ToString();
        var transactionContext = "gameplay";
        //var level = PlayerData.instance.rank.ToString();
        //var itemType = "consumable";
        
        if (stageManager.playerController.Star > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Soft, // Currency type
                transactionContext,
                stageManager.playerController.Star,
                "stars",
                PlayerData.instance.stars,
                //itemType,
                //level,
                transactionId
            );
        }

        if (stageManager.playerController.Coin > 0)
        {
            AnalyticsEvent.ItemAcquired(
                AcquisitionType.Premium, // Currency type
                transactionContext,
                stageManager.playerController.Coin,
                "coins",
                PlayerData.instance.coins,
                //itemType,
                //level,
                transactionId
            );
        }
#endif 

        m_CoinCredited = true;
    }

    protected void FinishRun()
    {
        /*
        if (miniLeaderboard.playerEntry.inputName.text == "")
        {
            miniLeaderboard.playerEntry.inputName.text = "Trash Cat";
        }
        else
        {
            PlayerData.instance.previousName = miniLeaderboard.playerEntry.inputName.text;
        }
        */
        PlayerData.instance.previousName = fullLeaderboard.playerEntry.playerName.text;

        PlayerData.instance.InsertScore(stageManager.playerPosZ, fullLeaderboard.playerEntry.playerName.text);
        

        PlayerController.DeathEvent de = stageManager.playerController.deathData;

        //register data to analytics
#if UNITY_ANALYTICS
        AnalyticsEvent.GameOver(null, new Dictionary<string, object> {
            { "stars", de.stars },
            { "coins", de.coins },
            //{ "premium", de.premium },
            { "score", de.score },
            //{ "distance", de.worldDistance },
            //{ "obstacle",  de.obstacleType },
            //{ "theme", de.themeUsed },
            { "character", de.character },
        });
#endif

        PlayerData.instance.Save();

        stageManager.End();
    }

    //----------------
}
