using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

public class GPGSManager : MonoBehaviour
{
    //싱글톤 패턴
    private bool mAuthenticating = false;

    // what is the highest score we have posted to the leaderboard?
    private int mHighestPostedScore = 0;

    private static GPGSManager _instance;
    public static GPGSManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GPGSManager>();

                if (_instance == null)
                {
                    _instance = new GameObject("GPGSManager").AddComponent<GPGSManager>();
                }
            }

            return _instance;
        }
    }

    public bool Authenticated
    {
        get
        {
            return Social.Active.localUser.authenticated;
        }
    }

    public bool Authenticating
    {
        get
        {
            return mAuthenticating;
        }

    }
    public void Authenticate()
    {
        if (Authenticated || mAuthenticating)
        {
            Debug.LogWarning("Ignoring repeated call to Authenticate().");
            return;
        }

        // Enable/disable logs on the PlayGamesPlatform
        PlayGamesPlatform.DebugLogEnabled = false;

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);

        // Activate the Play Games platform. This will make it the default
        // implementation of Social.Active
        PlayGamesPlatform.Activate();

        // Set the default leaderboard for the leaderboards UI
        ((PlayGamesPlatform)Social.Active).SetDefaultLeaderboardForUI(GPGSIds.leaderboard_leaderboard);

        // Sign in to Google Play Games
        mAuthenticating = true;
        Social.localUser.Authenticate((bool success) =>
        {
            mAuthenticating = false;
            if (success)
            {
                // if we signed in successfully, load data from cloud
                Debug.Log("Login successful!");
            }
            else
            {
                // no need to show error message (error messages are shown automatically
                // by plugin)
                Debug.LogWarning("Failed to sign in with Google Play Games.");
            }
        });
    }

    public void SignOut()
    {
        ((PlayGamesPlatform)Social.Active).SignOut();
    }

    /*
    public void Login()
    {
        Social.localUser.Authenticate((bool success) => { if (!success) { Debug.Log("Login Fail"); } });
    }
    */

    public bool isAuthenticated
    {
        get{
            return Social.localUser.authenticated;
        }
    }

    // ----- 업적 -----
    public void APassionateFrog()
    {
        if(!isAuthenticated)
        {
            Authenticate();
            return;
        }

        Social.ReportProgress(GPGSIds.achievement_a_passionate_frog, 100.0, (bool success) => { if (success) { PlayerPrefs.SetInt("A Passionate Frog", 1);}});
    }

    public void AExtraordinaryQuokka()
    {
        if (!isAuthenticated)
        {
            Authenticate();
            return;
        }

        Social.ReportProgress(GPGSIds.achievement_a_extraordinary_quokka, 100.0, (bool success) => { if (success) { PlayerPrefs.SetInt("A Extraordinary Quokka", 1); } });
    }

    public void AProudRabbit()
    {
        if (!isAuthenticated)
        {
            Authenticate();
            return;
        }

        Social.ReportProgress(GPGSIds.achievement_a_proud_rabbit, 100.0, (bool success) => { if (success) { PlayerPrefs.SetInt("A Proud Rabbit", 1); } });
    }

    //--------
    public void ShowAchievementsUI()
    {
        if (!isAuthenticated)
        {
            Authenticate();
            return;
        }

        Social.ShowAchievementsUI();
    }

    //리더보드 조회하기
    public void ShowLeaderboardUI()
    {
        if (!isAuthenticated)
        {
            Authenticate();
            return;
        }

        Social.ShowLeaderboardUI();
    }

    public void PostToLeaderboard()
    {
        int score = PlayerData.instance.highscores[0].score;
        if (Authenticated && score > mHighestPostedScore)
        {
            // post score to the leaderboard
            Social.ReportScore(score, GPGSIds.leaderboard_leaderboard, (bool success) =>
            {
            });
            mHighestPostedScore = score;
        }
        else
        {
            Debug.LogWarning("Not reporting score, auth = " + Authenticated + " " +
                score + " <= " + mHighestPostedScore);
        }
    }
    /*
    private bool _authenticating = false;
    public bool Authenticated { get { return Social.Active.localUser.authenticated; } }

    //list of achievements we know we have unlocked (to avoid making repeated calls to the API)
    private Dictionary<string, bool> _unlockedAchievements = new Dictionary<string, bool>();

    //achievement increments we are accumulating locally, waiting to send to the games API
    private Dictionary<string, int> _pendingIncrements = new Dictionary<string, int>();

    //GooglePlayGames 초기화
    public void Initialize()
    {
        //PlayGamesPlatform 로그 활성화/비활성화
        PlayGamesPlatform.DebugLogEnabled = false;
        //Social.Active 초기화
        PlayGamesPlatform.Activate();
    }

    //GooglePlayGames 로그인
    public void SignInToGooglePlay()
    {
        if (Authenticated || _authenticating)
        {
            Debug.LogWarning("Ignoring repeated call to Authenticate().");
            return;
        }

        _authenticating = true;
        Social.localUser.Authenticate((bool success) =>
        {
            _authenticating = false;
            if (success)
            {
                Debug.Log("Sign in successful!");
            }
            else
            {
                Debug.LogWarning("Failed to sign in with Google Play");
            }
        });
    }

    //GooglePlayGames 로그아웃
    public void SignOutFromGooglePlay()
    {
        GooglePlayGames.PlayGamesPlatform.Instance.SignOut();
    }

    //업적 조회하기
    public void ShowGooglePlayAchievements()
    {
        if (Authenticated)
        {
            Social.ShowAchievementsUI();
        }
    }

    //리더보드 조회하기
    public void ShowLeaderboardUI()
    {
        if (Authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }
    */
}