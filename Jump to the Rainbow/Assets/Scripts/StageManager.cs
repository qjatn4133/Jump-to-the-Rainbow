using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANALYTICS
using UnityEngine.Analytics;
using UnityEngine.Analytics.Experimental;
#endif

public class StageManager : MonoBehaviour {

    static protected StageManager m_Instance;
    static public StageManager instance { get { return m_Instance; } }

    [Header("Stages")]
    public int StageTipSize;
    int currentTipIndex;
    public Transform character;
    public GameObject[] stageTips;
    public int startTipIndex;
    public int preInstantiate;
    public int DestroyOldestStageCount;

    public List<GameObject> generatedStageList = new List<GameObject>();

    [Header("Backgrounds")]
    public int backGTipSize;
    int backGCurrentTipIndex;
    public GameObject[] backGStageTips;
    public int backGStartTipIndex;
    public int backGPreInstantiate;
    public int backGDestroyOldestStageCount;

    public List<GameObject> backGGeneratedStageList = new List<GameObject>();

    [Header("Character & Movements")]
    public PlayerController playerController;
    public Transform characterSlot;

    [Header("Other")]
    public SideTrigger sideTrigger;


    [Header("Camera")]
    public CameraController cameraController;

    protected bool m_Rerun;
    public bool isRerun { get { return m_Rerun; } set { m_Rerun = value; } }

    public int playerPosZ;

    private bool inGame = false;

    /*
    private void Awake()
    {
        GameState.OnEndGame += ClearStage;
    }
    */
    /*
    void Start ()
    {
        currentTipIndex = startTipIndex - 1;
        UpdateStage(preInstantiate);
    }
    */

    public void Begin()
    {
        // (이어하기가 아닌) 시작 시 씬 전체를 초기화(캐릭터, 스테이지, 장애물, 몬스터, 배경등)
        if (!m_Rerun)
        {
            inGame = true;

            //backGroundGenerator.Begin();
            KeepPosition keepPosition = sideTrigger.GetComponent<KeepPosition>();
            keepPosition.IsGame(true);

            playerController.gameObject.SetActive(true);
            //playerController = Instantiate(playerController);
            //characterSlot = playerController.characterSlot;

            // Spawn the player
            Skin player = Instantiate(SkinDatabase.GetSkin(SkinDatabase.skinNameList[PlayerData.instance.usedSkin]), Vector3.zero, Quaternion.identity);
            player.transform.SetParent(characterSlot, false);
            //Camera.main.transform.SetParent(characterController.transform, true);


            //player.SetupAccesory(PlayerData.instance.usedAccessory);

            if (playerController.character == null)
            {
                playerController.character = player;
            }
            playerController.stageManager = this;
            playerController.Init();
            cameraController.Begin();

            //characterController.Init();
            //characterController.CheatInvincible(invincible);

            //m_CurrentThemeData = ThemeDatabase.GetThemeData(PlayerData.instance.themes[PlayerData.instance.usedTheme]);
            //m_CurrentZone = 0;
            //m_CurrentZoneDistance = 0;

            //skyMeshFilter.sharedMesh = m_CurrentThemeData.skyMesh;
            //RenderSettings.fogColor = m_CurrentThemeData.fogColor;
            //RenderSettings.fog = true;

            gameObject.SetActive(true);

            playerController.gameObject.SetActive(true);

            playerController.Star = 0;
            playerController.Coin = 0;
            //characterController.coins = 0;
            //characterController.premium = 0;

            //m_Score = 0;
            //m_ScoreAccum = 0;

            //m_SafeSegementLeft = k_StartingSafeSegments;

            //Coin.coinPool = new Pooler(currentTheme.collectiblePrefab, k_StartingCoinPoolSize);

            PlayerData.instance.StartRunMissions(this);
            currentTipIndex = startTipIndex - 1;
            backGCurrentTipIndex = backGStartTipIndex - 1;

#if UNITY_ANALYTICS
            AnalyticsEvent.GameStart(new Dictionary<string, object>
            {
                //{ "theme", m_CurrentThemeData.themeName},
                { "character", player.characterName }
                //{ "accessory",  PlayerData.instance.usedAccessory >= 0 ? player.accessories[PlayerData.instance.usedAccessory].accessoryName : "none"}
            });
#endif
        }

        playerController.Begin();
        //gameObject.SetActive(true);
        UpdateStage(preInstantiate);
        UpdateBackGStage(backGPreInstantiate);
    }

    public void End()
    {
        ClearStage();
        ClearBackGStage();
        Destroy(playerController.character.gameObject);
        playerController.character = null;
        playerController.gameObject.SetActive(false);
        //backGroundGenerator.End();

        inGame = false;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(inGame)
        {
            // 캐릭터의 위치에서 현재 스테이지 팁의 인댁스를 계산
            playerPosZ = (int)character.position.z;
            int charPositionIndex = (playerPosZ / StageTipSize);
            // 다음의 스테이지 팁에 들어가면 스테이지의 업데이트 처리를 실시
            if (charPositionIndex + preInstantiate > currentTipIndex)
            {
                UpdateStage(charPositionIndex + preInstantiate);
            }

            int backGcharPositionIndex = (playerPosZ / backGTipSize);

            if (backGcharPositionIndex + backGPreInstantiate > backGCurrentTipIndex)
            {
                UpdateBackGStage(backGcharPositionIndex + backGPreInstantiate);
            }

            PlayerData.instance.UpdateMissions(this);
            //backGround.transform.position = new Vector3(backGround.transform.position.x, backGround.transform.position.y, characterSlot.transform.position.z);
        }
    }

    // ------- StageTips 함수 --------

    // 저장 Index까지의 스테이지 팁을 생성하여 관리해둔다
    void UpdateStage (int toTipIndex)
    {
        if(toTipIndex <= currentTipIndex)
        {
            return;
        }

        // 지정한 스테이지 팁까지를 작성
        for(int i = currentTipIndex + 1; i <= toTipIndex; i++)
        {
            GameObject stageObject = GenerateStage(i);

            // 생성한 스테이지를 관리 리스트에 추가
            generatedStageList.Add(stageObject);
            //Debug.Log(generatedStageList);
        }
         
        // 스테이지 보유 한도를 초과했다면 예전 스테이지를 삭제
        while(generatedStageList.Count > preInstantiate + DestroyOldestStageCount)
        {
            DestroyOldestStage();
        }
        
        currentTipIndex = toTipIndex;
    }

    // 지정 인덱스 위치에 Stage 오브젝트를 임의로 작성
    GameObject GenerateStage(int tipIndex)
    {
        int nextStageTip = Random.Range(0, stageTips.Length);

        GameObject stageObject = Instantiate(stageTips[nextStageTip], new Vector3(0, 0, tipIndex * StageTipSize), Quaternion.identity);

        return stageObject;
    }
    
    // 가장 오래된 스테이지를 삭제
    void DestroyOldestStage()
    {
        GameObject oldStage = generatedStageList[0];
        generatedStageList.RemoveAt(0);
        Destroy(oldStage);
    }

    // 모든 스테이지 삭제
    void ClearStage()
    {
        int n = generatedStageList.Count;
        for(int i = 0; i < n; i++)
        {
            GameObject oldStage = generatedStageList[0];
            generatedStageList.RemoveAt(0);
            Destroy(oldStage);
        }
        currentTipIndex = startTipIndex - 1;
        //UpdateStage(preInstantiate);

    }

    // -------------- BackGroundStage 함수 -----------------

    // 저장 Index까지의 스테이지 팁을 생성하여 관리해둔다
    void UpdateBackGStage(int toTipIndex)
    {
        if (toTipIndex <= backGCurrentTipIndex)
        {
            return;
        }

        // 지정한 스테이지 팁까지를 작성
        for (int i = backGCurrentTipIndex + 1; i <= toTipIndex; i++)
        {
            GameObject stageObject = GenerateBackGStage(i);

            // 생성한 스테이지를 관리 리스트에 추가
            backGGeneratedStageList.Add(stageObject);
            //Debug.Log(generatedStageList);
        }

        // 스테이지 보유 한도를 초과했다면 예전 스테이지를 삭제
        while (backGGeneratedStageList.Count > backGPreInstantiate + backGDestroyOldestStageCount)
        {
            DestroyOldestBackGStage();
        }

        backGCurrentTipIndex = toTipIndex;
    }

    // 지정 인덱스 위치에 Stage 오브젝트를 임의로 작성
    GameObject GenerateBackGStage(int tipIndex)
    {
        int nextStageTip = Random.Range(0, backGStageTips.Length);

        GameObject stageObject = Instantiate(backGStageTips[nextStageTip], new Vector3(0, 0, tipIndex * backGTipSize), Quaternion.identity);

        return stageObject;
    }

    // 가장 오래된 스테이지를 삭제
    void DestroyOldestBackGStage()
    {
        GameObject oldStage = backGGeneratedStageList[0];
        backGGeneratedStageList.RemoveAt(0);
        Destroy(oldStage);
    }

    // 모든 스테이지 삭제
    void ClearBackGStage()
    {
        int n = backGGeneratedStageList.Count;
        for (int i = 0; i < n; i++)
        {
            //Debug.Log(generatedStageList.Count);
            GameObject oldStage = backGGeneratedStageList[0];
            backGGeneratedStageList.RemoveAt(0);
            Destroy(oldStage);
        }

    }
}
