using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour {

    void Start()
    {
        StartCoroutine(LoadYourAsyncScene());
    }

    IEnumerator LoadYourAsyncScene()
    {
        // 애플리케이션 은 현재 장면 과 동시에 배경에 장면 을 로드합니다 .
        // 이것은 로딩 화면을 만드는 데 특히 유용합니다. build // number로 장면을로드 할 수도 있습니다.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");

        // 마지막 작업이 완전히로드 될 때까지 기다렸다가 아무것도 반환하지 않습니다.
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
