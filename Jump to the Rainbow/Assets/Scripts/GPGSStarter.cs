using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPGSStarter : MonoBehaviour {

    static bool sAutoAuthenticate = true;

    void Start()
    {

        // if this is the first time we're running, bring up the sign in flow
        if (sAutoAuthenticate)
        {
            GPGSManager.Instance.Authenticate();
            sAutoAuthenticate = false;
        }
    }

    public void OnSignIn()
    {
        if (GPGSManager.Instance.Authenticating)
        {
            return;
        }

        if (GPGSManager.Instance.Authenticated)
        {
            GPGSManager.Instance.SignOut();
        }
        else
        {
            GPGSManager.Instance.Authenticate();
        }
    }

    /*
    public void OnAchievements()
    {
        GPGSManager.Instance.ShowAchievementsUI();
    }

    public void OnHighScores()
    {
        GPGSManager.Instance.ShowLeaderboardUI();
    }
    */
}
