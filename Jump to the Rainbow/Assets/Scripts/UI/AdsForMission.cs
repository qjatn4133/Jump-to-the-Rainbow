using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
#if UNITY_ADS
using UnityEngine.Advertisements;
#endif
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
using UnityEngine.Analytics.Experimental;
#endif

public class AdsForMission : MonoBehaviour
{
    public MissionUI missionUI;

    //public Text newMissionText;
    public Button adsButton;
    public Text adsButtonText;
#if UNITY_ANALYTICS
    public AdvertisingNetwork adsNetwork = AdvertisingNetwork.UnityAds;
#endif
    public string adsPlacementId = "rewardedVideo";
    public bool adsRewarded = true;
    public int adsStandbyTimeValue;
    public int rewardCoinCount = 5;

    private DateTime adsActivatingTime;
    private DateTime currentDate;
    private DateTime displayTime;
    private string AdsDescLanguage;

    [Header("Korean Text")]
    public Text adsDesc;

    //private bool isAdsRun = false;

    void OnEnable()
    {
        adsButton.gameObject.SetActive(false);

        //currentDate = DateTime.UtcNow;

        if (!PlayerData.instance.koreanCheck)
        {
            AdsDescLanguage = "Watch AD and get 5 Coins";
        }
        else
        {
            AdsDescLanguage = "광고보고 5코인 받기";
        }

        adsButtonText.text = AdsDescLanguage;

        adsActivatingTime = DateTime.FromBinary(PlayerData.instance.adsActivatingTime);
        /*
        if(currentDate >= adsActivatingTime)
        {
            adsButtonText.text = "광고보고 5 코인 받기 !";
        }
        else
        {
            adsButton.interactable = false;

            int adsT = adsStandbyTimeValue;
            adsT = (int)(adsActivatingTime - currentDate).TotalSeconds;

            adsButtonText.text = adsT + " s";

            if (!isAdsRun)
            {
                InvokeRepeating("TimeDisplay", 0, 0.5f);
                Invoke("AdsActivating", adsT);
            }
        }
        */

#if UNITY_ADS
        var isReady = Advertisement.IsReady(adsPlacementId);
        if (isReady)
        {
#if UNITY_ANALYTICS
            AnalyticsEvent.AdOffer(adsRewarded, adsNetwork, adsPlacementId, new Dictionary<string, object>
            {
                { "level_index", PlayerData.instance.rank },
                { "distance", StageManager.instance == null ? 0 : StageManager.instance.playerPosZ },
            });
#endif
        }

        //newMissionText.gameObject.SetActive(isReady);
        adsButton.gameObject.SetActive(isReady);
#endif
    }

    public void ShowAds()
    {
#if UNITY_ADS
        if (Advertisement.IsReady(adsPlacementId))
        {
            PlayerData.instance.coins += rewardCoinCount;
#if UNITY_ANALYTICS
            AnalyticsEvent.AdStart(adsRewarded, adsNetwork, adsPlacementId, new Dictionary<string, object>
            {
                { "level_index", PlayerData.instance.rank },
                { "distance", StageManager.instance == null ? 0 : StageManager.instance.playerPosZ },
            });
#endif
            var options = new ShowOptions {resultCallback = HandleShowResult};
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
#endif
    }

#if UNITY_ADS

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                AdsTimeCheck();
#if UNITY_ANALYTICS
                AnalyticsEvent.AdComplete(adsRewarded, adsNetwork, adsPlacementId);
#endif
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

    private void Update()
    {
        currentDate = DateTime.UtcNow;

        if (currentDate >= adsActivatingTime)
        {
            adsButton.interactable = true;

            adsButtonText.text = AdsDescLanguage;

        }
        else
        {
            adsButton.interactable = false;
            displayTime = DateTime.UtcNow;

            adsButtonText.text = (int)(adsActivatingTime - displayTime).TotalSeconds + " s";
        }
    }

    void AdsTimeCheck()
    {
        //adsButton.interactable = false;

        DateTime tt = DateTime.UtcNow;

        adsActivatingTime = tt.AddSeconds(adsStandbyTimeValue);

        long dt = adsActivatingTime.ToBinary();
        PlayerData.instance.adsActivatingTime = dt;

        //InvokeRepeating("TimeDisplay", 0, 0.5f);
        //Invoke("AdsActivating", adsStandbyTimeValue);

        //isAdsRun = true;
        //displayTime = DateTime.UtcNow;

        PlayerData.instance.Save();
    }

    /*
    void TimeDisplay()
    {
        displayTime = DateTime.UtcNow;

        adsButtonText.text = (int)(adsActivatingTime - displayTime).TotalSeconds + " s";
    }

    void AdsActivating()
    {
        CancelInvoke("TimeDisplay");
        isAdsRun = false;

        adsButton.interactable = true;

        adsButtonText.text = "광고보고 5 코인 받기 !";

        PlayerData.instance.adsActivatingTime = 0;
        PlayerData.instance.Save();
    }
    */
}