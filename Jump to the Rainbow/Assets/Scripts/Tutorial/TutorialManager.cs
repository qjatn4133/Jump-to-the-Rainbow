using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public AudioClip gameTheme;

    [Header("Prefab")]
    public GameObject buffPrefab;
    public GameObject starPrefab;
    public GameObject applePrefab;
    public GameObject coinPrefab;
    public GameObject landingPrefab;
    public GameObject crashPrefab;
    public GameObject bouncePrefab;
    public GameObject seaOtterPrefab;
    public GameObject laplasPrefab;
    public GameObject snakePrefab;

    [Header("PrefabPosition")]
    public Transform prefabPosition;
    public GameObject prefabAllow;

    [Header("TutorialUI")]
    public GameObject intro;
    public GameObject life;
    public GameObject jump1;
    public GameObject jump2;
    public GameObject star;
    public GameObject apple;
    public GameObject coin;
    public GameObject sea;
    public GameObject landing;
    public GameObject crash;
    public GameObject bounce;
    public GameObject seaOtter;
    public GameObject laplas;
    public GameObject snake;

    public GameObject good;
    public GameObject goToLoadout;
    public GameObject loading;


    [Header("Korean Language")]
    public Text goodText;
    public Text rewardTitle;
    public Text introTitle;
    public Text lifeDesc;
    public Text jump1Desc;
    public Text jump2Desc;
    public Text starDesc;
    public Text appleEdsc;
    public Text coinDesc;
    public Text seaDesc;
    public Text landingDesc;
    public Text crashDesc;
    public Text bounceDesc;
    public Text seaotterDesc;
    public Text laplasDesc;
    public Text snakeDesc;
    public Text gotoLoadoutDesc;

    [Header("GameUI")]
    public Text starCount;
    public Text coinCount;
    public GameObject pauseButton;
    public GameObject pauseMenu;
    public GameObject wholeUI;
    public GameObject reward;

    public PlayerController playerController;
    //public TutorialInputUI tutorialinputUI;
    public CameraController cameraController;
    public HealthBar healthBar;

    protected const string k_LoadoutSceneName = "Main";
    public Transform characterSlot;
    public Skin character;

    // Use this for initialization
    void Start () {

        if (MusicPlayer.instance.GetStem(0) != gameTheme)
        {
            MusicPlayer.instance.SetStem(0, gameTheme);
            CoroutineHandler.StartStaticCoroutine(MusicPlayer.instance.RestartAllStems());
        }

        StartGame();
    }

    void StartGame()
    {
        if (!PlayerData.instance.koreanCheck)
        {
            goodText.text = "GOOD !";
            rewardTitle.text = "COIN!";
            introTitle.text = "TUTORIAL";
            lifeDesc.text = "When the game begins,\r\nThe passion of the character\r\nIt's shrinking little by little.";
            jump1Desc.text = "Press the circle\r\nunder the character,\r\nPull down";
            jump2Desc.text = "Set the direction\r\nand jump!\r\nwhen you put your finger.";
            starDesc.text = "passion + 15%\r\nStar + 1";
            appleEdsc.text = "passion = 100%";
            coinDesc.text = "Coin + 1";
            seaDesc.text = "If you fall into the sea,\r\ncharacter's passion is reduced by 10%.\r\nGet in the water once!";
            landingDesc.text = "You can land on\r\nwater lilies and rocks.";
            crashDesc.text = "These guys\r\nget in the way.";
            bounceDesc.text = "Alocasia bounces\r\nwhen hit";
            seaotterDesc.text = "If you get on the belly of a sea otter,\r\nit will bounce off after 3 seconds.\r\nLook at the redness of the stomach!";
            laplasDesc.text = "Rafflesia swallows\r\nand spits out\r\nhere and there!";
            snakeDesc.text = "The snake hinders your way\r\nBut when you hit the snake's head,\r\nthe snake runs away!";
            gotoLoadoutDesc.text = "END ~ !\r\nTake an adventure\r\ntowards the rainbow!";
}
        else
        {
            goodText.text = "좋아요 !";
            rewardTitle.text = "코인!";
            introTitle.text = "튜토리얼";
            lifeDesc.text = "게임이 시작되면\r\n캐릭터의 열정이\r\n조금씩 줄어듭니다.";
            jump1Desc.text = "캐릭터 밑에\r\n원을 누르고\r\n밑으로 당기세요.";
            jump2Desc.text = "방향을 정하고\r\n손가락을 때면\r\n점프 !";
            starDesc.text = "열정 + 15%\r\n별 + 1";
            appleEdsc.text = "열정 = 100%";
            coinDesc.text = "코인 + 1";
            seaDesc.text = "바다에 빠지면\r\n열정이 10 % 줄어듭니다.\r\n한 번 빠져보세요!";
            landingDesc.text = "수련과 바위에는\r\n착지가 가능합니다.";
            crashDesc.text = "이 친구들은\r\n가는 길을 방해합니다.";
            bounceDesc.text = "알로카시아는\r\n충돌하면\r\n튕겨냅니다.";
            seaotterDesc.text = "해달은 배 위에 올라타면\r\n3초 뒤에 튕겨냅니다.\r\n배가 빨갛게 변하는 걸 보세요!";
            laplasDesc.text = "라플라스는 삼키고\r\n여기저기로 뱉어냅니다!";
            snakeDesc.text = "뱀은 가는 길을 방해합니다.\r\n하지만 머리를 들이받으면\r\n도망갑니다!";
            gotoLoadoutDesc.text = "끝 ~ !\r\n무지개를 향해\r\n모험을 떠나세요!";
        }
        // --- Language Setting ---

        goToLoadout.gameObject.SetActive(false);
        loading.gameObject.SetActive(false);
        playerController.gameObject.SetActive(true);
        playerController.inTutorial = true;

        Skin player = Instantiate(character, Vector3.zero, Quaternion.identity);
        player.transform.SetParent(characterSlot, false);

        if (playerController.character == null)
        {
            playerController.character = player;
        }


        playerController.Begin();
        cameraController.TutorialBegin();
        healthBar.inTutorial = true;
    }

    public void IntroClose()
    {
        intro.gameObject.SetActive(false);
        life.gameObject.SetActive(true);
    }

    public void LifeClose()
    {
        life.gameObject.SetActive(false);
        StartCoroutine(Jump1());
    }

    IEnumerator Jump1()
    {
        jump1.gameObject.SetActive(true);

        while(0.3 > playerController.inputUIController.slider.value)
        {
            yield return null;
        }

        jump1.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);
        StartCoroutine(Jump2());
    }

    IEnumerator Jump2()
    {
        jump2.gameObject.SetActive(true);

        while (playerController.jumpEnable)
        {
            yield return null;
        }

        jump2.gameObject.SetActive(false);


        yield return new WaitForSeconds(1f);


        good.gameObject.SetActive(true);

        while (!playerController.jumpEnable)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);

        StartCoroutine(Star());
    }

    IEnumerator Star()
    {
        buffPrefab.gameObject.SetActive(true);
        star.gameObject.SetActive(true);
        prefabAllow.gameObject.SetActive(true);

        ResetPosition();

        Instantiate(starPrefab, prefabPosition);

        while (0 == playerController.Star)
        {
            if(0 < playerController.transform.position.z && playerController.jumpEnable)
            {
                yield return new WaitForSeconds(1f);

                ResetPosition();
            }
            yield return null;
        }

        starCount.text = "X " + playerController.Star.ToString();

        star.gameObject.SetActive(false);
        prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);
        StartCoroutine(Apple());
    }

    IEnumerator Apple()
    {
        apple.gameObject.SetActive(true);
        prefabAllow.gameObject.SetActive(true);

        ResetPosition();

        Instantiate(applePrefab, prefabPosition);

        while (!(playerController.tCollidingObject is Apple))
        {
            if (0 < playerController.transform.position.z && playerController.jumpEnable)
            {
                yield return new WaitForSeconds(1f);

                ResetPosition();
            }
            yield return null;
        }

        apple.gameObject.SetActive(false);
        prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);
        StartCoroutine(Coin());
    }

    IEnumerator Coin()
    {
        coin.gameObject.SetActive(true);
        prefabAllow.gameObject.SetActive(true);

        ResetPosition();

        Instantiate(coinPrefab, prefabPosition);

        while (!(playerController.tCollidingObject is Coin))
        {
            if (0 < playerController.transform.position.z && playerController.jumpEnable)
            {
                yield return new WaitForSeconds(1f);

                ResetPosition();
            }
            yield return null;
        }
        coinCount.text = "X " + playerController.Coin.ToString();
        coin.gameObject.SetActive(false);
        prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        buffPrefab.gameObject.SetActive(false);

        good.gameObject.SetActive(false);
        StartCoroutine(Sea());
    }

    IEnumerator Sea()
    {
        sea.gameObject.SetActive(true);
        //prefabAllow.gameObject.SetActive(true);

        ResetPosition();

        //Instantiate(coinPrefab, prefabPosition);

        while (!(playerController.tCollidingObject is SideTrigger))
        {
            yield return null;
        }
        
        sea.gameObject.SetActive(false);
        //prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);
        Landing();
    }

    private void Landing()
    {
        landing.gameObject.SetActive(true);
        landingPrefab.gameObject.SetActive(true);

        //ResetPosition();
    }

    public void LandingClose()
    {

        landing.gameObject.SetActive(false);

        landingPrefab.gameObject.SetActive(false);

        playerController.jumpEnable = false;
        ResetPosition();

        crash.gameObject.SetActive(true);
        crashPrefab.gameObject.SetActive(true);
    }

    public void CrashClose()
    {
        crash.gameObject.SetActive(false);

        crashPrefab.gameObject.SetActive(false);

        StartCoroutine(Alocasia());
        //ResetPosition();

        //bounce.gameObject.SetActive(true);
        //bouncePrefab.gameObject.SetActive(true);
    }

    IEnumerator Alocasia()
    {
        bounce.gameObject.SetActive(true);
        //prefabAllow.gameObject.SetActive(true);
        bouncePrefab.gameObject.SetActive(true);

        ResetPosition();

        //Instantiate(coinPrefab, prefabPosition);

        while (!(playerController.tCollidingObject is Alocasia))
        {
            yield return null;
        }
        while (!(playerController.tCollidingObject is SideTrigger))
        {
            yield return null;
        }

        bounce.gameObject.SetActive(false);
        bouncePrefab.gameObject.SetActive(false);

        //prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);

        StartCoroutine(SeaOtter());

        //seaOtter.gameObject.SetActive(true);
        //seaOtterPrefab.gameObject.SetActive(true);
    }

    IEnumerator SeaOtter()
    {
        seaOtter.gameObject.SetActive(true);
        //prefabAllow.gameObject.SetActive(true);
        seaOtterPrefab.gameObject.SetActive(true);

        //ResetPosition();

        //Instantiate(coinPrefab, prefabPosition);

        while (!(playerController.tCollidingObject is SeaOtter))
        {
            yield return null;
        }

        //playerController.lastPosition = new Vector3(0, 3, 0);

        while (!(playerController.tCollidingObject is SideTrigger))
        {
            playerController.lastPosition = new Vector3(0, 3, 0);

            yield return null;
        }

        //playerController.lastPosition = new Vector3(0, 3, 0);

        seaOtter.gameObject.SetActive(false);
        seaOtterPrefab.gameObject.SetActive(false);

        //prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);


        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);

        StartCoroutine(Laplas());

        //snake.gameObject.SetActive(true);
        //snakePrefab.gameObject.SetActive(true);
    }
    /*
    public void BounceClose()
    {
        bounce.gameObject.SetActive(false);

        bouncePrefab.gameObject.SetActive(false);

        ResetPosition();

        seaOtter.gameObject.SetActive(true);
        seaOtterPrefab.gameObject.SetActive(true);
    }
    */

    /*
public void SeaOtterClose()
{
    seaOtter.gameObject.SetActive(false);

    seaOtterPrefab.gameObject.SetActive(false);

    StartCoroutine(Laplas());
    //ResetPosition();

    //laplas.gameObject.SetActive(true);
    //laplasPrefab.gameObject.SetActive(true);
}
*/

    IEnumerator Laplas()
    {
        laplas.gameObject.SetActive(true);
        //prefabAllow.gameObject.SetActive(true);
        laplasPrefab.gameObject.SetActive(true);

        //ResetPosition();

        //Instantiate(coinPrefab, prefabPosition);

        while (!(playerController.tCollidingObject is Laplas))
        {
            yield return null;
        }
        while(!(playerController.tCollidingObject is SideTrigger))
        {
            yield return null;
        }

        laplas.gameObject.SetActive(false);
        laplasPrefab.gameObject.SetActive(false);

        //prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);


        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);
        StartCoroutine(Snake());

        //snake.gameObject.SetActive(true);
        //snakePrefab.gameObject.SetActive(true);
    }

    /*
    public void LaplasClose()
    {
        laplas.gameObject.SetActive(false);

        laplasPrefab.gameObject.SetActive(false);

        ResetPosition();

        snake.gameObject.SetActive(true);
        snakePrefab.gameObject.SetActive(true);
    }
    */

    IEnumerator Snake()
    {
        snake.gameObject.SetActive(true);
        //prefabAllow.gameObject.SetActive(true);
        snakePrefab.gameObject.SetActive(true);

        //ResetPosition();

        //Instantiate(coinPrefab, prefabPosition);

        while (!(playerController.tCollidingObject is Snake_head))
        {
            yield return null;
        }
        while (!(playerController.tCollidingObject is SideTrigger))
        {
            yield return null;
        }

        snake.gameObject.SetActive(false);
        snakePrefab.gameObject.SetActive(false);

        //prefabAllow.gameObject.SetActive(false);

        good.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        good.gameObject.SetActive(false);

        //ResetPosition();

        goToLoadout.gameObject.SetActive(true);

        if (!PlayerData.instance.tutorialCompletion)
        {
            reward.gameObject.SetActive(true);
            PlayerData.instance.coins += 10;

        }
        //snake.gameObject.SetActive(false);
        //snakePrefab.gameObject.SetActive(false);
    }
    /*
    public void SnakeClose()
    {
        snake.gameObject.SetActive(false);

        snakePrefab.gameObject.SetActive(false);

        ResetPosition();

        goToLoadout.gameObject.SetActive(true);
    }
    */

    public void Pause()
    {

        AudioListener.pause = true;
        Time.timeScale = 0;

        pauseButton.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);
        wholeUI.gameObject.SetActive(false);
        //m_WasMoving = trackManager.isMoving;
        //trackManager.StopMove();
    }

    public void Resume()
    {
        Time.timeScale = 1.0f;
        pauseButton.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);
        wholeUI.gameObject.SetActive(true);

        AudioListener.pause = false;
    }

    public void GoToLoadout()
    {
        reward.gameObject.SetActive(false);
        loading.gameObject.SetActive(true);

        Destroy(playerController.character.gameObject);
        playerController.character = null;
        playerController.inTutorial = false;
        playerController.gameObject.SetActive(false);

        AudioListener.pause = false;

        Time.timeScale = 1;

        goToLoadout.gameObject.SetActive(false);

        PlayerData.instance.tutorialCompletion = true;
        PlayerData.instance.tutorialCheck = false;

        PlayerData.instance.Save();

        UnityEngine.SceneManagement.SceneManager.LoadScene(k_LoadoutSceneName, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    private void ResetPosition()
    {
        playerController.ResetPosition();
        cameraController.Init();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
