using UnityEngine;
using System.IO;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
using UnityEngine.Analytics.Experimental;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

public struct HighscoreEntry : System.IComparable<HighscoreEntry>
{
    public string name;
    public int score;

    public int CompareTo(HighscoreEntry other)
    {
        // We want to sort from highest to lowest, so inverse the comparison.
        return other.score.CompareTo(score);
    }
}

/// <summary>
/// Save data for the game. This is stored locally in this case, but a "better" way to do it would be to store it on a server
/// somewhere to avoid player tampering with it. Here potentially a player could modify the binary file to add premium currency.
/// </summary>
public class PlayerData
{
    static protected PlayerData m_Instance;
    static public PlayerData instance { get { return m_Instance; } }

    protected string saveFile = "";

    public bool tutorialCompletion;
    public bool tutorialCheck;

    public bool koreanCheck;

    public int coins;
    public int stars;
    public long adsActivatingTime;

    //public int premium;
    // public Dictionary<Consumable.ConsumableType, int> consumables = new Dictionary<Consumable.ConsumableType, int>();   // Inventory of owned consumables and quantity.

    public List<string> characters = new List<string>();    // Inventory of characters owned.
    public int usedCharacter;                               // Currently equipped character.
    //public int usedAccessory = -1;
    //public List<string> characterAccessories = new List<string>();  // List of owned accessories, in the form "charName:accessoryName".
    //public List<string> themes = new List<string>();                // Owned themes.
    //public int usedTheme;                                           // Currently used theme.
    public List<string> skins = new List<string>();
    public int usedSkin;

    public List<HighscoreEntry> highscores = new List<HighscoreEntry>();
    public List<MissionBase> missions = new List<MissionBase>();
    public List<AchievementMissionBase> achievementMissions = new List<AchievementMissionBase>();


    public string previousName = "USER";
    //public bool licenceAccepted;

    public float masterVolume = float.MinValue, musicVolume = float.MinValue, masterSFXVolume = float.MinValue;

    //ftue = First Time User Expeerience. This var is used to track thing a player do for the first time. It increment everytime the user do one of the step
    //e.g. it will increment to 1 when they click Start, to 2 when doing the first run, 3 when running at least 300m etc.
    public int ftueLevel = 0;
    //Player win a rank ever 300m (e.g. a player having reached 1200m at least once will be rank 4)
    public int rank = 0;

    // This will allow us to add data even after production, and so keep all existing save STILL valid. See loading & saving for how it work.
    // Note in a real production it would probably reset that to 1 before release (as all dev save don't have to be compatible w/ final product)
    // Then would increment again with every subsequent patches. We kept it to its dev value here for teaching purpose. 
    static int s_Version = 11;

    public void AddCharacter(string name)
    {
        characters.Add(name);
    }

    public void AddSkin(string name)
    {
        skins.Add(name);
    }

    /*
    public void AddTheme(string theme)
    {
        themes.Add(theme);
    }

    public void AddAccessory(string name)
    {
        characterAccessories.Add(name);
    }
    */

    // Mission management

    // Will add missions until we reach 2 missions.
    public void CheckMissionsCount()
    {
        while (missions.Count < 2)
            AddMission();
    }

    public void CheckAchievementMissionCount()
    {
        if (achievementMissions.Count < (int)AchievementMissionBase.MissionType.MAX)
        {
            for (int i = 0; i < (int)AchievementMissionBase.MissionType.MAX; i++)
                AddAchievementMission(i);

        }
    }

    public void AddMission()
    {
        int val = Random.Range(0, (int)MissionBase.MissionType.MAX);

        MissionBase newMission = MissionBase.GetNewMissionFromType((MissionBase.MissionType)val);
        newMission.Created();

        missions.Add(newMission);
    }

    public void AddAchievementMission(int num)
    {
        //int val = Random.Range(0, (int)MissionBase.MissionType.MAX);

        AchievementMissionBase newMission = AchievementMissionBase.GetNewMissionFromType((AchievementMissionBase.MissionType)num);
        newMission.Created();

        achievementMissions.Add(newMission);
    }

    public void StartRunMissions(StageManager manager)
    {
        for (int i = 0; i < missions.Count; ++i)
        {
            missions[i].RunStart(manager);
        }

        for (int i = 0; i < achievementMissions.Count; ++i)
        {
            achievementMissions[i].RunStart(manager);
        }
    }

    public void UpdateMissions(StageManager manager)
    {
        for (int i = 0; i < missions.Count; ++i)
        {
            missions[i].Update(manager);
        }

        for (int i = 0; i < achievementMissions.Count; ++i)
        {
            achievementMissions[i].Update(manager);
        }
    }
    

    public bool AnyMissionComplete()
    {
        for (int i = 0; i < missions.Count; ++i)
        {
            if (missions[i].isComplete) return true;
        }

        for (int i = 0; i < achievementMissions.Count; ++i)
        {
            if (achievementMissions[i].isComplete) return true;
        }

        return false;
    }

    public void ClaimMission(MissionBase mission)
    {
        stars += mission.reward;

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Premium, // Currency type
            "mission",               // Context
            mission.reward,          // Amount
            "coins",             // Item ID
            //premium,                 // Item balance
            //"consumable",            // Item type
            rank.ToString()          // Level
        );
#endif

        missions.Remove(mission);

        CheckMissionsCount();

        Save();
    }

    public void ClaimAchievementMission(AchievementMissionBase mission)
    {
        stars += mission.reward;

#if UNITY_ANALYTICS // Using Analytics Standard Events v0.3.0
        AnalyticsEvent.ItemAcquired(
            AcquisitionType.Premium, // Currency type
            "mission",               // Context
            mission.reward,          // Amount
            "coins",             // Item ID
            //premium,                 // Item balance
            //"consumable",            // Item type
            rank.ToString()          // Level
        );
#endif

        achievementMissions.Remove(mission);

        //CheckMissionsCount();

        Save();
    }

    // High Score management

    public int GetScorePlace(int score)
    {
        HighscoreEntry entry = new HighscoreEntry();
        entry.score = score;
        entry.name = "";

        int index = highscores.BinarySearch(entry);

        return index < 0 ? (~index) : index;
    }

    public void InsertScore(int score, string name)
    {
        HighscoreEntry entry = new HighscoreEntry();
        entry.score = score;
        entry.name = name;

        highscores.Insert(GetScorePlace(score), entry);

        // Keep only the 10 best scores.
        while (highscores.Count > 10)
            highscores.RemoveAt(highscores.Count - 1);
    }

    // File management

    static public void Create()
    {
        if (m_Instance == null)
        {
            m_Instance = new PlayerData();

            //if we create the PlayerData, mean it's the very first call, so we use that to init the database
            //this allow to always init the database at the earlier we can, i.e. the start screen if started normally on device
            //or the Loadout screen if testing in editor
            AssetBundlesDatabaseHandler.Load();
        }

        m_Instance.saveFile = Application.persistentDataPath + "/save.bin";

        if (File.Exists(m_Instance.saveFile))
        {
            // If we have a save, we read it.
            m_Instance.Read();
        }
        else
        {
            // If not we create one with default data.
            NewSave();
            
        }

        m_Instance.CheckMissionsCount();
    }

    static public void NewSave()
    {
        m_Instance.characters.Clear();
        m_Instance.skins.Clear();
        //m_Instance.themes.Clear();
        m_Instance.missions.Clear();
        m_Instance.achievementMissions.Clear();
        //m_Instance.characterAccessories.Clear();
        //m_Instance.consumables.Clear();

        m_Instance.tutorialCompletion = false;
        m_Instance.tutorialCheck = true;
        m_Instance.koreanCheck = false;

        m_Instance.usedCharacter = 0;
        m_Instance.usedSkin = 0;
        //m_Instance.usedTheme = 0;
        //m_Instance.usedAccessory = -1;

        m_Instance.stars = 0;
        m_Instance.adsActivatingTime = 0;

        m_Instance.coins = 0;
        //m_Instance.premium = 0;

        m_Instance.characters.Add("frog");
        m_Instance.skins.Add("frog_basic");

        //m_Instance.themes.Add("Day");

        m_Instance.ftueLevel = 0;
        m_Instance.rank = 0;

        m_Instance.CheckMissionsCount();
        m_Instance.CheckAchievementMissionCount();
        m_Instance.Save();
    }

    public void Read()
    {
        BinaryReader r = new BinaryReader(new FileStream(saveFile, FileMode.Open));

        int ver = r.ReadInt32();

        if (ver < 6)
        {
            r.Close();

            NewSave();
            r = new BinaryReader(new FileStream(saveFile, FileMode.Open));
            ver = r.ReadInt32();
        }

        stars = r.ReadInt32();

        /*
        consumables.Clear();
        int consumableCount = r.ReadInt32();
        for (int i = 0; i < consumableCount; ++i)
        {
            consumables.Add((Consumable.ConsumableType)r.ReadInt32(), r.ReadInt32());
        }
        */

        // Read character.
        characters.Clear();
        int charCount = r.ReadInt32();
        for (int i = 0; i < charCount; ++i)
        {
            string charName = r.ReadString();
            /*
            if (charName.Contains("Raccoon") && ver < 11)
            {//in 11 version, we renamed Raccoon (fixing spelling) so we need to patch the save to give the character if player had it already
                charName = charName.Replace("Racoon", "Raccoon");
            }
            */
            characters.Add(charName);
        }

        // Read skin.
        skins.Clear();
        int skinCount = r.ReadInt32();
        for (int i = 0; i < skinCount; ++i)
        {
            string skinName = r.ReadString();
            /*
            if (charName.Contains("Raccoon") && ver < 11)
            {//in 11 version, we renamed Raccoon (fixing spelling) so we need to patch the save to give the character if player had it already
                charName = charName.Replace("Racoon", "Raccoon");
            }
            */
            skins.Add(skinName);
        }

        tutorialCompletion = r.ReadBoolean();
        tutorialCheck = r.ReadBoolean();
        koreanCheck = r.ReadBoolean();

        usedCharacter = r.ReadInt32();
        usedSkin = r.ReadInt32();

        /*
        // Read character accesories.
        characterAccessories.Clear();
        int accCount = r.ReadInt32();
        for (int i = 0; i < accCount; ++i)
        {
            characterAccessories.Add(r.ReadString());
        }

        // Read Themes.
        themes.Clear();
        int themeCount = r.ReadInt32();
        for (int i = 0; i < themeCount; ++i)
        {
            themes.Add(r.ReadString());
        }

        usedTheme = r.ReadInt32();
        */
        // Save contains the version they were written with. If data are added bump the version & test for that version before loading that data.
        if (ver >= 2)
        {
            coins = r.ReadInt32();
        }
        

        // Added highscores.
        if (ver >= 3)
        {
            highscores.Clear();
            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                HighscoreEntry entry = new HighscoreEntry();
                entry.name = r.ReadString();
                entry.score = r.ReadInt32();

                highscores.Add(entry);
            }
        }

        // Added missions.
        if (ver >= 4)
        {
            missions.Clear();

            int count = r.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                MissionBase.MissionType type = (MissionBase.MissionType)r.ReadInt32();
                MissionBase tempMission = MissionBase.GetNewMissionFromType(type);

                tempMission.Deserialize(r);

                if (tempMission != null)
                {
                    missions.Add(tempMission);
                }
            }

            achievementMissions.Clear();

            int aCount = r.ReadInt32();
            for (int i = 0; i < aCount; ++i)
            {
                AchievementMissionBase.MissionType type = (AchievementMissionBase.MissionType)r.ReadInt32();
                AchievementMissionBase achievementTempMission = AchievementMissionBase.GetNewMissionFromType(type);

                achievementTempMission.Deserialize(r);

                if (achievementTempMission != null)
                {
                    achievementMissions.Add(achievementTempMission);
                }
            }
        }

        // Added highscore previous name used.
        if (ver >= 7)
        {
            previousName = r.ReadString();
        }
        /*
        if (ver >= 8)
        {
            licenceAccepted = r.ReadBoolean();
        }
        */
        if (ver >= 9)
        {
            masterVolume = r.ReadSingle();
            musicVolume = r.ReadSingle();
            masterSFXVolume = r.ReadSingle();
        }

        if (ver >= 10)
        {
            ftueLevel = r.ReadInt32();
            rank = r.ReadInt32();
            adsActivatingTime = r.ReadInt64();

        }

        r.Close();
    }

    public void Save()
    {
        BinaryWriter w = new BinaryWriter(new FileStream(saveFile, FileMode.OpenOrCreate));

        w.Write(s_Version);
        w.Write(stars);

        /*
        w.Write(consumables.Count);
        foreach (KeyValuePair<Consumable.ConsumableType, int> p in consumables)
        {
            w.Write((int)p.Key);
            w.Write(p.Value);
        }
        */

        // Write characters.
        w.Write(characters.Count);
        foreach (string c in characters)
        {
            w.Write(c);
        }

        // Write skins.
        w.Write(skins.Count);
        foreach (string s in skins)
        {
            w.Write(s);
        }

        w.Write(tutorialCompletion);
        w.Write(tutorialCheck);
        w.Write(koreanCheck);

        w.Write(usedCharacter);
        w.Write(usedSkin);

        /*
        w.Write(characterAccessories.Count);
        foreach (string a in characterAccessories)
        {
            w.Write(a);
        }
        */

        /*
        // Write themes.
        w.Write(themes.Count);
        foreach (string t in themes)
        {
            w.Write(t);
        }
        */

        //w.Write(usedTheme);
        //w.Write(premium);
        w.Write(coins);

        // Write highscores.
        w.Write(highscores.Count);
        for (int i = 0; i < highscores.Count; ++i)
        {
            w.Write(highscores[i].name);
            w.Write(highscores[i].score);
        }

        // Write missions.
        w.Write(missions.Count);
        for (int i = 0; i < missions.Count; ++i)
        {
            w.Write((int)missions[i].GetMissionType());
            missions[i].Serialize(w);
        }

        w.Write(achievementMissions.Count);
        for (int i = 0; i < achievementMissions.Count; ++i)
        {
            w.Write((int)achievementMissions[i].GetMissionType());
            achievementMissions[i].Serialize(w);
        }

        // Write name.
        w.Write(previousName);

        //w.Write(licenceAccepted);

        w.Write(masterVolume);
        w.Write(musicVolume);
        w.Write(masterSFXVolume);

        w.Write(ftueLevel);
        w.Write(rank);
        w.Write(adsActivatingTime);

        w.Close();
    }


}

// Helper class to cheat in the editor for test purpose
#if UNITY_EDITOR
public class PlayerDataEditor : Editor
{
    [MenuItem("Jump to the Rainbow Debug/Clear Save")]
    static public void ClearSave()
    {
        File.Delete(Application.persistentDataPath + "/save.bin");
    }

    [MenuItem("Jump to the Rainbow Debug/Give 1000000 stars and 1000 coins")]
    static public void GiveCoins()
    {
        PlayerData.instance.stars += 1000000;
        PlayerData.instance.coins += 1000000;
        //PlayerData.instance.premium += 1000;
        PlayerData.instance.Save();
    }

    /*
    [MenuItem("Trash Dash Debug/Give 10 Consumables of each types")]
    static public void AddConsumables()
    {

        for (int i = 0; i < ShopItemList.s_ConsumablesTypes.Length; ++i)
        {
            Consumable c = ConsumableDatabase.GetConsumbale(ShopItemList.s_ConsumablesTypes[i]);
            if (c != null)
            {
                PlayerData.instance.consumables[c.GetConsumableType()] = 10;
            }
        }

        PlayerData.instance.Save();
    }
    */
}
#endif
