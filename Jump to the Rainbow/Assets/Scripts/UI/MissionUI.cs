using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour {

    public RectTransform repetitionMissionPlace;
    public RectTransform achievementMissionPlace;
    public MissionEntry missionEntryPrefab;
    public AchievementMissionEntry achievementMissionEntryPrefab;
    public GameObject achievementPopup;
    public Text rewardCount;

    [Header("Korean Language")]
    public Text title;
    public Text achievementTitle;

    //public AdsForMission addMissionButtonPrefab;

    public void Open()
    {
        gameObject.SetActive(true);

        if(!PlayerData.instance.koreanCheck)
        {
            title.text = "MISSION";
            achievementTitle.text = "REWARD";
        }
        else
        {
            title.text = "미 션";
            achievementTitle.text = "별 획득 !";
        }


        foreach (Transform t in repetitionMissionPlace)
            Destroy(t.gameObject);

        for (int i = 0; i < 2; ++i)
        {
            if (PlayerData.instance.missions.Count > i)
            {
                MissionEntry entry = Instantiate(missionEntryPrefab);
                entry.transform.SetParent(repetitionMissionPlace, false);

                entry.FillWithMission(PlayerData.instance.missions[i], this);
            }
            /*
            else
            {
                AdsForMission obj = Instantiate(addMissionButtonPrefab);
                obj.missionUI = this;
                obj.transform.SetParent(repetitionMissionPlace, false);
            }
            */
        }

        foreach (Transform t in achievementMissionPlace)
            Destroy(t.gameObject);

        for (int i = 0; i < PlayerData.instance.achievementMissions.Count; ++i)
        {
            AchievementMissionEntry entry = Instantiate(achievementMissionEntryPrefab);
            entry.transform.SetParent(achievementMissionPlace, false);

            entry.FillWithMission(PlayerData.instance.achievementMissions[i], this);
        }
    }

    public void Claim(MissionBase m)
    {
        PlayerData.instance.ClaimMission(m);

        // Rebuild the UI with the new missions
        Open();
        achievementPopup.gameObject.SetActive(true);
        rewardCount.text = m.reward.ToString();
        
    }

    public void AchievementClaim(AchievementMissionBase m)
    {
        PlayerData.instance.ClaimAchievementMission(m);

        // Rebuild the UI with the new missions
        Open();
        achievementPopup.gameObject.SetActive(true);
        rewardCount.text = m.reward.ToString();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
