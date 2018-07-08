using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

/// <summary>
/// State pushed on the GameManager during the Loadout, when player select player, theme and accessories
/// Take care of init the UI, load all the data used for it etc.
/// </summary>
public class LoadoutState : AState
{

    public Canvas loadout;

    [Header("Main UI")]
    public Button startButton;
    public MissionUI missionPopup;
    public Leaderboard leaderboard;
    public GameObject BuySkinPopup;
    public GameObject tutorialButton;
    public GameObject tutorialReward;

    [Header("Char UI")]
    public Text charNameDisplay;
    public RectTransform charSelector;
    public Button charSkinSelectButton;
    public GameObject skinPriceButton;
    public GameObject lockImage;
    public Text skinStarText;
    public Text skinCoinText;

    [Header("Skin UI")]
    public RectTransform skinSelector;

    //public Text skinNameDisplay;
    //public Transform skinPosition;
    //public float skinInterval = 2f;

    [Header("Buy Skin Popup")]
    public Text buySkinName;
    public Button buySkinByStarButton;
    public Button buySkinByCoinButton;

    [Header("Achievement Popup")]
    public Text achievementName;

    [Header("StarCoin")]
    public Text currentStar;
    public Text currentCoin;
    public Text buySkinStar;
    public Text buySkinCoin;

    [Header("Camera")]
    public CameraController cameraController;

    /*
    [Header("Theme UI")]
    public Text themeNameDisplay;
    public RectTransform themeSelect;
    public Image themeIcon;

    [Header("PowerUp UI")]
    public RectTransform powerupSelect;
    public Image powerupIcon;
    public Text powerupCount;
    public Sprite noItemIcon;

    [Header("Accessory UI")]
    public RectTransform accessoriesSelector;
    public Text accesoryNameDisplay;
    public Image accessoryIconDisplay;
    */

    [Header("Other Data")]
    public float charInterval = 2.2f;
    public Transform charPosition;
    public GameObject loading;
    public GameObject loadoutBackGroundStage;
    public GameObject startGround;
    public AudioClip menuTheme;
    public Text loginSign;

    //public GameObject BuyCharacterPopup;
    //public Text buyCharStar;
    //public Text buyCharCoin;
    //public Text buyCharName;

    [Header("Toggle")]
    public Toggle tutorialToggle;
    public Toggle koreanToggle;

    //public MeshFilter skyMeshFilter;
    //public MeshFilter UIGroundFilter;

    [Header("Korean Language")]
    public Text tutorialText;
    public Text buySkinPopupTitle;
    public Text buySkinAchievementTitle;

    //[Header("Prefabs")]
    //public ConsumableIcon consumableIcon;

    //Consumable.ConsumableType m_PowerupToUse = Consumable.ConsumableType.NONE;

    protected const string k_TutorialSceneName = "tutorial";
    protected string m_CharacterName;
    protected GameObject m_Character;
    protected GameObject c1;
    protected GameObject c3;

    protected Skin m_Skin;
    //protected GameObject s1;
    //protected GameObject s3;

    protected int charNum;
    protected int skinNum;
    protected int basicIndex;

    //protected List<int> m_OwnedAccesories = new List<int>();
    //protected int m_UsedAccessory = -1;
    //protected int m_UsedPowerupIndex;
    protected bool m_IsLoadingCharacter;

    protected Modifier m_CurrentModifier = new Modifier();

    protected const float k_CharacterRotationSpeed = 45f;
    //protected const string k_ShopSceneName = "shop";
    //protected const float k_OwnedAccessoriesCharacterOffset = -0.1f;
    //protected int k_UILayer;
    protected readonly Quaternion k_FlippedYAxisRotation = Quaternion.Euler(0f, 180f, 0f);


    public override void Enter(AState from)
    {
        loading.gameObject.SetActive(false);
        loadout.gameObject.SetActive(true);
        loadoutBackGroundStage.gameObject.SetActive(true);
        startGround.gameObject.SetActive(false);
        missionPopup.gameObject.SetActive(false);
        //BuyCharacterPopup.gameObject.SetActive(false);
        BuySkinPopup.gameObject.SetActive(false);

        charNameDisplay.text = "";
        //skinNameDisplay.text = "";
        //themeNameDisplay.text = "";

        //k_UILayer = LayerMask.NameToLayer("UI");

        //skyMeshFilter.gameObject.SetActive(true);
        //UIGroundFilter.gameObject.SetActive(true);

        // Reseting the global blinking value. Can happen if the game unexpectedly exited while still blinking
        //Shader.SetGlobalFloat("_BlinkingValue", 0.0f);

        
        if (MusicPlayer.instance.GetStem(0) != menuTheme)
        {
            MusicPlayer.instance.SetStem(0, menuTheme);
            StartCoroutine(MusicPlayer.instance.RestartAllStems());
        }
        

        startButton.interactable = false;
        startButton.GetComponentInChildren<Text>().text = "Loading...";

        /*
        if (m_PowerupToUse != Consumable.ConsumableType.NONE)
        {
            //if we come back from a run and we don't have any more of the powerup we wanted to use, we reset the powerup to use to NONE
            if (!PlayerData.instance.consumables.ContainsKey(m_PowerupToUse) || PlayerData.instance.consumables[m_PowerupToUse] == 0)
                m_PowerupToUse = Consumable.ConsumableType.NONE;
        }
        */

        //charNameList = new List<string>(CharacterDatabase.dictionary.Keys);
        /*
        foreach (KeyValuePair<string, Character> pair in CharacterDatabase.dictionary)
        {
            string name = pair.Key;
        }

        foreach (string name in charNameList)
        {
        }
        */
        charNum = PlayerData.instance.usedCharacter;
        skinNum = PlayerData.instance.usedSkin;
        cameraController.InLoadout();
        Refresh();
    }

    public override void Exit(AState to)
    {
        missionPopup.gameObject.SetActive(false);
        loadout.gameObject.SetActive(false);
        loadoutBackGroundStage.gameObject.SetActive(false);
        startGround.gameObject.SetActive(true);

        if (m_CharacterName != null) m_CharacterName = null;
        if (m_Character != null) Destroy(m_Character);
        if (c1 != null) Destroy(c1);
        if (c3 != null) Destroy(c3);

        if (m_Skin != null) m_Skin = null;
        //if (s1 != null) Destroy(s1);
        //if (s3 != null) Destroy(s3);


        GameState gs = to as GameState;

        //skyMeshFilter.gameObject.SetActive(false);
        //UIGroundFilter.gameObject.SetActive(false);

        if (gs != null)
        {
            gs.currentModifier = m_CurrentModifier;

            // We reset the modifier to a default one, for next run (if a new modifier is applied, it will replace this default one before the run starts)
            m_CurrentModifier = new Modifier();
            /*
            if (m_PowerupToUse != Consumable.ConsumableType.NONE)
            {
                PlayerData.instance.Consume(m_PowerupToUse);
                Consumable inv = Instantiate(ConsumableDatabase.GetConsumbale(m_PowerupToUse));
                inv.gameObject.SetActive(false);
                gs.trackManager.characterController.inventory = inv;
            }
            */
        }
    }

    public void Refresh()
    {
        // --- Tutorial Check ---
        if (PlayerData.instance.tutorialCheck)
        {
            tutorialButton.gameObject.SetActive(true);
            tutorialToggle.isOn = true;
        }
        else
        {
            tutorialButton.gameObject.SetActive(false);
            tutorialToggle.isOn = false;
        }

        if (PlayerData.instance.tutorialCompletion)
        {
            tutorialReward.gameObject.SetActive(false);
        }
        else
        {
            tutorialReward.gameObject.SetActive(true);
        }

        //PopulatePowerup();
        if (PlayerData.instance.koreanCheck)
        {
            //koreanToggle.isOn = true;

            tutorialText.text = "튜토리얼";

        }
        else
        {
            //koreanToggle.isOn = false;

            tutorialText.text = "TUTORIAL";

        }

        StartCoroutine(PopulateCharactersWithSkins());

        //StartCoroutine(PopulateTheme());
    }

    public void ChangeLanguage()
    {
        StartCoroutine(PopulateSkins());

        if (PlayerData.instance.koreanCheck)
        {
            tutorialText.text = "튜토리얼";
        }
        else
        {
            tutorialText.text = "TUTORIAL";
        }
    }

    public override string GetName()
    {
        return "Loadout";
    }

    public override void Tick()
    {
        if (!startButton.interactable)
        {
            bool interactable = SkinDatabase.loaded; //ThemeDatabase.loaded && CharacterDatabase.loaded;
            if (interactable)
            {
                startButton.interactable = true;
                startButton.GetComponentInChildren<Text>().text = "JUMP !";
            }
        }

        if (m_Character != null)
        {
            m_Character.transform.Rotate(0, k_CharacterRotationSpeed * Time.deltaTime, 0, Space.Self);
        }

        currentStar.text = PlayerData.instance.stars.ToString();
        currentCoin.text = PlayerData.instance.coins.ToString();

        if (!GPGSManager.Instance.isAuthenticated)
        {
            loginSign.text = "Login Fail!";
        }
        else
        {
            loginSign.text = "Login Success!";
        }
        //themeSelect.gameObject.SetActive(PlayerData.instance.themes.Count > 1);
    }

    /*
    public void GoToStore()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(k_ShopSceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }
    */

    public void GoToTutorial()
    {
        loading.gameObject.SetActive(true);
        //tutorialToggle.isOn = false;
        //tutorialButton.gameObject.SetActive(false);
        PlayerData.instance.tutorialCheck = false;

        PlayerData.instance.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene(k_TutorialSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ChangeCharacter(int dir)
    {
        /*
        PlayerData.instance.usedCharacter += dir;
        if (PlayerData.instance.usedCharacter >= PlayerData.instance.characters.Count)
            PlayerData.instance.usedCharacter = 0;
        else if (PlayerData.instance.usedCharacter < 0)
            PlayerData.instance.usedCharacter = PlayerData.instance.characters.Count - 1;
            */

        charNum += dir;
        if (charNum >= SkinDatabase.charNameList.Count)
            charNum = 0;
        else if (charNum < 0)
            charNum = SkinDatabase.charNameList.Count - 1;

        StartCoroutine(PopulateCharactersWithSkins());
        //CharacterSelectUIUpdate();
        SkinSelectUIUpdate();
    }

    public void ChangeSkin(int dir)
    {
        skinNum += dir;
        if (skinNum >= SkinDatabase.skinNameList.Count)
            skinNum = 0;
        else if (skinNum < 0)
            skinNum = SkinDatabase.skinNameList.Count - 1;

        StartCoroutine(PopulateSkins());
        SkinSelectUIUpdate();
    }

    public void SelectSkin()
    {
        PlayerData.instance.usedSkin = skinNum;
        PlayerData.instance.usedCharacter = charNum;
        //CharacterSelectUIUpdate();
        SkinSelectUIUpdate();
        PlayerData.instance.Save();

    }

    public void BuySkinPopupOpen()
    {
        buySkinName.text = m_Skin.displayName;

        if (!PlayerData.instance.koreanCheck)
        {
            buySkinPopupTitle.text = "BUY SKIN !";
            buySkinAchievementTitle.text = "NEW SKIN !";

            buySkinName.text = m_Skin.displayName;
            achievementName.text = m_Skin.displayName;
        }
        else
        {
            buySkinPopupTitle.text = "스킨 구입 !";
            buySkinAchievementTitle.text = "새로운 스킨 !";

            buySkinName.text = m_Skin.korDisplayName;
            achievementName.text = m_Skin.korDisplayName;

        }

        if (PlayerData.instance.stars >= m_Skin.GetComponent<Skin>().skinStar)
        {
            buySkinByStarButton.interactable = true;
        }
        else
        {
            buySkinByStarButton.interactable = false;
        }

        if (PlayerData.instance.coins >= m_Character.GetComponent<Skin>().skinCoin)
        {
            buySkinByCoinButton.interactable = true;
        }
        else
        {
            buySkinByCoinButton.interactable = false;
        }

        buySkinStar.text = m_Skin.skinStar.ToString();
        buySkinCoin.text = m_Skin.skinCoin.ToString();


        BuySkinPopup.gameObject.SetActive(true);
    }

    public void BuySkinByStar()
    {
        PlayerData.instance.stars -= m_Skin.skinStar;
        PlayerData.instance.AddSkin(SkinDatabase.skinNameList[skinNum]);
        //BuySkinPopup.gameObject.SetActive(false);
        SkinSelectUIUpdate();
        PlayerData.instance.Save();
    }

    public void BuySkinByCoin()
    {
        PlayerData.instance.coins -= m_Skin.skinCoin;
        PlayerData.instance.AddSkin(SkinDatabase.skinNameList[skinNum]);
        //BuySkinPopup.gameObject.SetActive(false);
        SkinSelectUIUpdate();
        PlayerData.instance.Save();
    }
    /*
    public void ChangeAccessory(int dir)
    {
        m_UsedAccessory += dir;
        if (m_UsedAccessory >= m_OwnedAccesories.Count)
            m_UsedAccessory = -1;
        else if (m_UsedAccessory < -1)
            m_UsedAccessory = m_OwnedAccesories.Count - 1;

        if (m_UsedAccessory != -1)
            PlayerData.instance.usedAccessory = m_OwnedAccesories[m_UsedAccessory];
        else
            PlayerData.instance.usedAccessory = -1;

        SetupAccessory();
    }
    */

    /*
public void ChangeTheme(int dir)
{
    PlayerData.instance.usedTheme += dir;
    if (PlayerData.instance.usedTheme >= PlayerData.instance.themes.Count)
        PlayerData.instance.usedTheme = 0;
    else if (PlayerData.instance.usedTheme < 0)
        PlayerData.instance.usedTheme = PlayerData.instance.themes.Count - 1;

    StartCoroutine(PopulateTheme());
}
*/

    /*
    public IEnumerator PopulateTheme()
    {
        ThemeData t = null;

        while (t == null)
        {
            t = ThemeDatabase.GetThemeData(PlayerData.instance.themes[PlayerData.instance.usedTheme]);
            yield return null;
        }

        themeNameDisplay.text = t.themeName;
        themeIcon.sprite = t.themeIcon;

        skyMeshFilter.sharedMesh = t.skyMesh;
        UIGroundFilter.sharedMesh = t.UIGroundMesh;
    }
    */

    /*
    void CharacterSelectUIUpdate()
    {
        if (PlayerData.instance.characters.Contains(m_CharacterName))
        {
            charPriceButton.gameObject.SetActive(false);
            charSelectButton.gameObject.SetActive(true);

            if (PlayerData.instance.usedCharacter == charNum)
            {
                charSelectButton.interactable = false;
            }
            else
            {
                charSelectButton.interactable = true;
            }
        }
        else
        {
            skinPriceButton.gameObject.SetActive(false);
            charPriceButton.gameObject.SetActive(true);
            if (PlayerData.instance.stars >= m_Skin.characterStar || PlayerData.instance.coins >= m_Character.GetComponent<Skin>().characterCoin)
            {
                charPriceButton.GetComponentInChildren<Button>().interactable = true;
            }
            else
            {
                charPriceButton.GetComponentInChildren<Button>().interactable = false;
            }
            charSelectButton.gameObject.SetActive(false);
        }
    }
    */

    void SkinSelectUIUpdate()
    {
        if (PlayerData.instance.skins.Contains(SkinDatabase.skinNameList[skinNum]))
        {
            skinPriceButton.gameObject.SetActive(false);
            charSkinSelectButton.gameObject.SetActive(true);
            lockImage.gameObject.SetActive(false);

            if (PlayerData.instance.usedSkin == skinNum)
            {
                charSkinSelectButton.interactable = false;
            }
            else
            {
                charSkinSelectButton.interactable = true;
            }
        }
        else
        {
            //if(m_Skin.skinName != m_Skin.characterName + "_basic")
            //skinPriceButton.gameObject.SetActive(true);
            charSkinSelectButton.gameObject.SetActive(false);
            skinPriceButton.gameObject.SetActive(true);

            if(PlayerData.instance.skins.Contains(m_Skin.characterName + "_basic"))
            {
                lockImage.gameObject.SetActive(false);

                if (PlayerData.instance.stars >= m_Skin.GetComponent<Skin>().skinStar || PlayerData.instance.coins >= m_Character.GetComponent<Skin>().skinCoin)
                {
                    skinPriceButton.GetComponentInChildren<Button>().interactable = true;
                }
                else
                {
                    skinPriceButton.GetComponentInChildren<Button>().interactable = false;
                }
            }
            else
            {
                if(m_Skin.skinName != m_Skin.characterName + "_basic")
                {
                    lockImage.gameObject.SetActive(true);
                    skinPriceButton.GetComponentInChildren<Button>().interactable = false;
                }
                else
                {
                    lockImage.gameObject.SetActive(false);
                    if (PlayerData.instance.stars >= m_Skin.GetComponent<Skin>().skinStar || PlayerData.instance.coins >= m_Character.GetComponent<Skin>().skinCoin)
                    {
                        skinPriceButton.GetComponentInChildren<Button>().interactable = true;
                    }
                    else
                    {
                        skinPriceButton.GetComponentInChildren<Button>().interactable = false;
                    }
                }
            }


        }
    }

    public IEnumerator PopulateCharactersWithSkins()
    {
        while(!SkinDatabase.loaded)
        {
            yield return null;
        }

        if (!m_IsLoadingCharacter)
        {
            m_IsLoadingCharacter = true;

            Destroy(c1);
            Destroy(c3);

            for (int i = 0; i <= SkinDatabase.charNameList.Count - 1; i++)
            {
                string c = SkinDatabase.charNameList[i];

                if (c != null) 
                {
                    if (charNum - 1 >= 0)
                    {
                        if (charNum - 1 == i)
                        {
                            GameObject newChar = null;
                            Skin s = SkinDatabase.GetSkin(c + "_basic");

                            newChar = Instantiate(s.gameObject);
                            newChar.transform.SetParent(charPosition, false);
                            newChar.transform.rotation = k_FlippedYAxisRotation;
                            newChar.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);

                            c1 = newChar;

                            c1.transform.localPosition = Vector3.right * 1 * charInterval;
                        }
                    }

                    if (charNum == i)
                    {

                        //GameObject newChar = null;

                        //Skin s = SkinDatabase.GetSkin(SkinDatabase.skinNameList[i]);
                        //newChar = Instantiate(s.gameObject);

                        if (m_CharacterName != null)
                            m_CharacterName = null;

                        m_CharacterName = c;

                        StartCoroutine(PopulateSkins());
                    }

                    if ((SkinDatabase.charNameList.Count - 1) - charNum >= 1)
                    {
                        if (charNum + 1 == i)
                        {
                            GameObject newChar = null;

                            Skin s = SkinDatabase.GetSkin(c + "_basic");

                            newChar = Instantiate(s.gameObject);
                            newChar.transform.SetParent(charPosition, false);
                            newChar.transform.rotation = k_FlippedYAxisRotation;
                            newChar.transform.localScale = new Vector3(2.2f, 2.2f, 2.2f);

                            c3 = newChar;

                            c3.transform.localPosition = Vector3.left * 1 * charInterval;
                        }
                    }
                }
                //else
                    //yield return new WaitForSeconds(1.0f);
            }
            //CharacterSelectUIUpdate();
            SkinSelectUIUpdate();
            m_IsLoadingCharacter = false;
        }
    }

    public IEnumerator PopulateSkins()
    {

        SkinSelectUIUpdate();

        //Destroy(s1);
        //Destroy(s3);

        List<string> charSkinList = new List<string>();

        charSkinList.Clear();

        for (int i = 0; i <= SkinDatabase.skinNameList.Count - 1; i++)
        {
            Skin s = SkinDatabase.GetSkin(SkinDatabase.skinNameList[i]);
            if(s.characterName == m_CharacterName)
            {
                charSkinList.Add(s.skinName);
            }
        }

        Skin sk = SkinDatabase.GetSkin(SkinDatabase.skinNameList[skinNum]);

        if (!charSkinList.Contains(sk.skinName))
        {
            string result = charSkinList.Find(item => item == (m_CharacterName + "_basic"));
            basicIndex = SkinDatabase.skinNameList.IndexOf(result);
            skinNum = basicIndex;

        }

        for (int i = 0; i <= SkinDatabase.skinNameList.Count - 1; i++)
        {
            Skin s = SkinDatabase.GetSkin(SkinDatabase.skinNameList[i]);

            if (s.characterName == m_CharacterName)
            {
                if (s != null)
                {
                    /*
                    if (skinNum - 1 >= 0)
                    {
                        if (skinNum - 1 == i)
                        {
                            GameObject newSkin = null;

                            newSkin = Instantiate(s.gameObject);
                            newSkin.transform.SetParent(skinPosition);
                            newSkin.transform.rotation = k_FlippedYAxisRotation;
                            newSkin.transform.localScale = new Vector3(2f, 2f, 2f);

                            s1 = newSkin;

                            s1.transform.localPosition = Vector3.right * skinInterval;
                        }
                    }
                    */

                    if (skinNum == i)
                    {
                        /*
                        GameObject newSkin = null;

                        newSkin = Instantiate(s.gameObject);
                        newSkin.transform.SetParent(skinPosition);
                        newSkin.transform.rotation = k_FlippedYAxisRotation;
                        newSkin.transform.localScale = new Vector3(2f, 2f, 2f);

                        if (m_Skin != null)
                            Destroy(m_Skin);

                        m_Skin = newSkin;

                        m_Skin.transform.localPosition = Vector3.zero;
                        */

                        m_Skin = s;
                        /*
                        if(s.skinName != s.characterName + "_basic")
                            skinNameDisplay.text = s.skinName;
                        else
                            skinNameDisplay.text = null;
                            */
                        //skinStarText.text = s.skinStar.ToString();
                        //skinCoinText.text = s.skinCoin.ToString();


                        //---character populate---
                        GameObject newChar = null;
                        while (newChar == null)
                        {
                            newChar = Instantiate(s.gameObject);
                            newChar.transform.SetParent(charPosition, false);
                            newChar.transform.rotation = k_FlippedYAxisRotation;

                            if (m_Character != null)
                                Destroy(m_Character);

                            m_Character = newChar;

                            if (!PlayerData.instance.koreanCheck)
                            {
                                charNameDisplay.text = s.displayName;
                            }
                            else
                            {
                                charNameDisplay.text = s.korDisplayName;

                            }

                            skinStarText.text = s.skinStar.ToString();
                            skinCoinText.text = s.skinCoin.ToString();

                            m_Character.transform.localPosition = Vector3.right * 1000;

                            yield return new WaitForEndOfFrame();
                            m_Character.transform.localPosition = Vector3.zero;
                        }
                    }
                    /*
                    if ((SkinDatabase.skinNameList.Count - 1) - skinNum >= 1)
                    {
                        if (skinNum + 1 == i)
                        {
                            GameObject newSkin = null;

                            newSkin = Instantiate(s.gameObject);
                            newSkin.transform.SetParent(skinPosition);
                            newSkin.transform.rotation = k_FlippedYAxisRotation;
                            newSkin.transform.localScale = new Vector3(2f, 2f, 2f);

                            s3 = newSkin;

                            s3.transform.localPosition = Vector3.left * 1 * skinInterval;
                        }
                    }
                    */
                }
            }
        }
    }

    /*
    void SetupAccessory()
    {
        Character c = m_Character.GetComponent<Character>();
        c.SetupAccesory(PlayerData.instance.usedAccessory);

        if (PlayerData.instance.usedAccessory == -1)
        {
            accesoryNameDisplay.text = "None";
            accessoryIconDisplay.enabled = false;
        }
        else
        {
            accessoryIconDisplay.enabled = true;
            accesoryNameDisplay.text = c.accessories[PlayerData.instance.usedAccessory].accessoryName;
            accessoryIconDisplay.sprite = c.accessories[PlayerData.instance.usedAccessory].accessoryIcon;
        }
    }

    void PopulatePowerup()
    {
        powerupIcon.gameObject.SetActive(true);

        if (PlayerData.instance.consumables.Count > 0)
        {
            Consumable c = ConsumableDatabase.GetConsumbale(m_PowerupToUse);

            powerupSelect.gameObject.SetActive(true);
            if (c != null)
            {
                powerupIcon.sprite = c.icon;
                powerupCount.text = PlayerData.instance.consumables[m_PowerupToUse].ToString();
            }
            else
            {
                powerupIcon.sprite = noItemIcon;
                powerupCount.text = "";
            }
        }
        else
        {
            powerupSelect.gameObject.SetActive(false);
        }
    }

    public void ChangeConsumable(int dir)
    {
        bool found = false;
        do
        {
            m_UsedPowerupIndex += dir;
            if (m_UsedPowerupIndex >= (int)Consumable.ConsumableType.MAX_COUNT)
            {
                m_UsedPowerupIndex = 0;
            }
            else if (m_UsedPowerupIndex < 0)
            {
                m_UsedPowerupIndex = (int)Consumable.ConsumableType.MAX_COUNT - 1;
            }

            int count = 0;
            if (PlayerData.instance.consumables.TryGetValue((Consumable.ConsumableType)m_UsedPowerupIndex, out count) && count > 0)
            {
                found = true;
            }

        } while (m_UsedPowerupIndex != 0 && !found);

        m_PowerupToUse = (Consumable.ConsumableType)m_UsedPowerupIndex;
        PopulatePowerup();
    }
    */

    public void SetModifier(Modifier modifier)
    {
        m_CurrentModifier = modifier;
    }

    public void StartGame()
    {
        if (PlayerData.instance.ftueLevel == 1)
        {
            PlayerData.instance.ftueLevel = 2;
            PlayerData.instance.Save();
        }

        manager.SwitchState("Game");
    }

    // ----- Google Play Games 서비스 관련 -----

    public void Openleaderboard()
    {
        //GPGSManager.Instance.ShowLeaderboardUI();
        leaderboard.displayPlayer = false;
        leaderboard.forcePlayerDisplay = false;
        leaderboard.Open();
    }

    public void ShowAchievementUI()
    {
        GPGSManager.Instance.ShowAchievementsUI();
    }


}
