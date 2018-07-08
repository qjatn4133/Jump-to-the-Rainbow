using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundGenerator : MonoBehaviour
{
    public StageManager stageManager;

    [Header("Stages")]
    public int StageTipSize;
    int currentTipIndex;
    public Transform character;
    private PlayerController playerController;
    public GameObject[] stageTips;
    public int startTipIndex;
    public int preInstantiate;
    public int DestroyOldestStageCount;
    public int playerPosZ;

    private bool inLoadout;

    public List<GameObject> generatedStageList = new List<GameObject>();
    /*
    private void Awake()
    {
        GameState.OnEndGame += ClearStage;
    }
    */

    public void Begin()
    {
        // 초기화

        character = stageManager.playerController.characterSlot;

        inLoadout = false;
        currentTipIndex = startTipIndex - 1;
    }

    public void End()
    {
        inLoadout = true;
        ClearStage();
        currentTipIndex = startTipIndex - 1;
        //UpdateStage(preInstantiate);

        //Begin();
        //gameObject.SetActive(false);
    }

    private void Update()
    {
        if(!inLoadout)
        {
            playerPosZ = (int)character.position.z;

            // 캐릭터의 위치에서 현재 스테이지 팁의 인댁스를 계산
            int charaPositionIndex = (playerPosZ / StageTipSize);
            // 다음의 스테이지 팁에 들어가면 스테이지의 업데이트 처리를 실시
            if (charaPositionIndex + preInstantiate > currentTipIndex)
            {
                UpdateStage(charaPositionIndex + preInstantiate);
            }
        }


    }

    // 저장 Index까지의 스테이지 팁을 생성하여 관리해둔다
    void UpdateStage(int toTipIndex)
    {
        if (toTipIndex <= currentTipIndex)
        {
            return;
        }

        // 지정한 스테이지 팁까지를 작성
        for (int i = currentTipIndex + 1; i <= toTipIndex; i++)
        {
            GameObject stageObject = GenerateStage(i);

            // 생성한 스테이지를 관리 리스트에 추가
            generatedStageList.Add(stageObject);
            //Debug.Log(generatedStageList);
        }

        // 스테이지 보유 한도를 초과했다면 예전 스테이지를 삭제
        while (generatedStageList.Count > preInstantiate + DestroyOldestStageCount)
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
        for (int i = 0; i < n; i++)
        {
            //Debug.Log(generatedStageList.Count);
            GameObject oldStage = generatedStageList[0];
            generatedStageList.RemoveAt(0);
            Destroy(oldStage);
        }
        //currentTipIndex = startTipIndex - 1;
        //UpdateStage(preInstantiate);

    }
}