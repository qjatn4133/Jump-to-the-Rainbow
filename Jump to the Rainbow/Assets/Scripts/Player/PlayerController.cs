using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerController : MonoBehaviour {

    public Skin character;
    public GameState gameState;
    public StageManager stageManager;
    public TutorialManager tutorialManager;
    //public CameraController cameraController;
    public InputUIController inputUIController;
    CharacterController controller;
    public Transform characterSlot;
    public Slider slider;

    public Vector3 moveDirection = Vector3.zero;
    public Vector3 lastPosition;

    public float velocity;

    [Header("Adjustment")]
    [SerializeField]
    private float velocityAdjust = 10.0f;

    [SerializeField]
    private float gravity = 9.81f;

    [SerializeField]
    private float bounceForce = 8.0f;

    [SerializeField]
    private float jumpSpeed = 2.0f;

    [SerializeField]
    private float life = 10;

    [SerializeField]
    public float waitingTime = 1.0f;

    public bool jumpEnable;

    [Header("Particle")]
    public GameObject prefep_stars;
    public GameObject prefep_dust;
    public GameObject prefep_crash;
    public GameObject prefep_spray;
    public GameObject prefep_waterFall;
    public GameObject prefep_dollar;

    [Header("Sound")]
    public AudioClip bounce;
    public AudioClip getStar;
    public AudioClip getCoin;
    public AudioClip landing;
    public AudioClip jumping;
    public AudioClip crashing;
    public AudioClip fallInWater;
    public AudioClip fallInSide;

    private int star = 0;
    private int coin = 0;
    public bool endGame;
    public bool dieFlag = false;
    public bool inTutorial = false;

    protected bool m_isMovable = true;
    public bool isMovable {get {return m_isMovable; } set { m_isMovable = value;}}

    static int s_IdleHash = Animator.StringToHash("Idle");
    static int s_DeadHash = Animator.StringToHash("Dead");
    static int s_JumpHash = Animator.StringToHash("Jump");
    static int s_HitHash = Animator.StringToHash("Hit");
    static int s_FallingHash = Animator.StringToHash("Falling");

    public static event Action EndGame = delegate { };
    public static event Action StartGame = delegate { };

    private void OnEnable()
    {

        InputUIController.OnDragEnd += Jump;

        CollidChecker.OnEnteredObject += CollidChecker_OnEnteredObject;
        CollidChecker.OnExitedObject += CollidChecker_OnExitedOBject;
    }

    private void OnDisable()
    {
        InputUIController.OnDragEnd -= Jump;

        CollidChecker.OnEnteredObject -= CollidChecker_OnEnteredObject;
        CollidChecker.OnExitedObject -= CollidChecker_OnExitedOBject;
    }

    public float Life
    {
        get
        {
            return life;
        }

        set
        {
            life = value;
            if (life >= 10)
                life = 10f;

            if (life <= 0 && dieFlag == false)
            {
                life = 0;
                dieFlag = true;
                endGame = true;
                EndGame();
                Die();
            }            
        }
    }

    public int Star
    {
        get
        {
            return star;
        }

        set
        {
            star = value;
            if (dieFlag == false && Life < 10)
            {
                Life = life + 1.5f;
            }
        }
    }

    public int Coin
    {
        get
        {
            return coin;
        }

        set
        {
            coin = value;
        }
    }

    public struct DeathEvent
    {
        public string character;
        //public string obstacleType;
        //public string themeUsed;
        public int stars;
        public int coins;
        //public int premium;
        public int score;
        //public float worldDistance;
    }

    public DeathEvent deathData { get { return m_DeathData; } }
    protected DeathEvent m_DeathData;

    //public new AudioSource audio { get { return m_Audio; } }
    protected AudioSource m_Audio;
    public static PlayerController instance = null;
    public IObject tCollidingObject;

    public void Begin()
    {
        EndGame = delegate { };
        StartGame = delegate { };

        Life = 10.0f;
        m_isMovable = true;
        if (character.animator == null)
        {
            character.animator = character.GetComponent<Animator>();
        }
        character.animator.SetBool(s_DeadHash, false);
        character.animator.Play(s_IdleHash);
        endGame = false;
        dieFlag = false;
        StartGame();
    }

    private void Die()
    {
        gameState.Die();
        //StopAllCoroutines();
        /*
        if(!jumpEnable)
        {
            transform.position = lastPosition + new Vector3(0.0f, 2.0f, 0.0f);
            character.animator.SetTrigger(s_FallingHash);
        }
        */
        //moveDirection = Vector3.zero;

        StartCoroutine(DieAnimationPlay());
        //m_isMovable = false;
        //character.animator.SetBool(s_DeadHash, true);
       // moveDirection = Vector3.zero;

        //m_Audio.PlayOneShot(character.deathSound);



        m_DeathData.character = character.characterName;
        //m_DeathData.themeUsed = controller.trackManager.currentTheme.themeName;
        //m_DeathData.obstacleType = ob.GetType().ToString();
        m_DeathData.stars = Star;
        m_DeathData.coins = Coin;
        //m_DeathData.premium = controller.premium;
        m_DeathData.score = stageManager.playerPosZ;
        //m_DeathData.worldDistance = controller.trackManager.worldDistance;
        //this.enabled = false;
        //enabled = false;

    }

    IEnumerator DieAnimationPlay()
    {
        while (!jumpEnable)
        {
            yield return null;

        }
        m_isMovable = false;
        character.animator.SetBool(s_DeadHash, true);
        m_Audio.PlayOneShot(character.deathSound);

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(gameState.WaitForGameOver());
        WaitForGameOver();
    }

    private void Awake()
    {

        //gameState = GetComponent<GameState>();
        //cameraController = GetComponent<CameraController>();

        controller = GetComponent<CharacterController>();
        m_Audio = GetComponent<AudioSource>();
    }

    private void CollidChecker_OnEnteredObject(IObject collidingObject)
    {

        tCollidingObject = collidingObject;

        //---방해물 or 장애물 충돌---
        if(collidingObject is Laplas)
        {
            character.animator.SetTrigger(s_HitHash);
            // Life 감소는 Laplas에서

            //m_isMovable = false;
            m_Audio.PlayOneShot(crashing);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Beaver)
        {
            character.animator.SetTrigger(s_HitHash);
            moveDirection = new Vector3(0, 3, -3);
            m_Audio.PlayOneShot(crashing);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if(collidingObject is Catfish)
        {
            character.animator.SetTrigger(s_HitHash);
            moveDirection = new Vector3(0, 3, -3);
            m_Audio.PlayOneShot(crashing);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if(collidingObject is SeaOtter_body)
        {
            character.animator.SetTrigger(s_HitHash);
            moveDirection = new Vector3(0, 3, -3);
            m_Audio.PlayOneShot(crashing);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Hipo)
        {
            character.animator.SetTrigger(s_HitHash);
            moveDirection = new Vector3(0, 3, -3);
            m_Audio.PlayOneShot(crashing);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Snake_body)
        {
            character.animator.SetTrigger(s_HitHash);
            moveDirection = new Vector3(0, 3, -3);
            m_Audio.PlayOneShot(crashing);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
        // ---바운스---
        else if (collidingObject is Alocasia)
        {
            m_Audio.PlayOneShot(bounce);

            if (prefep_crash != null)
            {
                Instantiate(prefep_crash, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        //---착지 가능 충돌---
        else if (collidingObject is WaterLily)
        {
            character.animator.SetBool(s_JumpHash, false);
            WaterLily lily = collidingObject as WaterLily;
            transform.eulerAngles = Vector3.zero;
            moveDirection = Vector3.zero;
            m_Audio.PlayOneShot(landing);

            if (prefep_dust != null)
            {
                Instantiate(prefep_dust, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }

            StartCoroutine(BindingPlayerPosition(lily.transform));
        }

        else if(collidingObject is SeaOtter)
        {
            character.animator.SetBool(s_JumpHash, false);
            jumpEnable = true;
            moveDirection = Vector3.zero;

            transform.eulerAngles = Vector3.zero;
            m_Audio.PlayOneShot(landing);

            // 마지막 위치 저장
            lastPosition = transform.position;

            if (prefep_dust != null)
            {
                Instantiate(prefep_dust, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Rocks)
        {
            character.animator.SetBool(s_JumpHash, false);
            jumpEnable = true;
            moveDirection = Vector3.zero;

            transform.eulerAngles = Vector3.zero;
            m_Audio.PlayOneShot(landing);

            // 마지막 위치 저장
            lastPosition = transform.position;

            if (prefep_dust != null)
            {
                Instantiate(prefep_dust, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
        //---Item 충돌---
        else if (collidingObject is Star)
        {
            Star++;
            Star objStar = collidingObject as Star;
            Destroy(objStar.gameObject);
            m_Audio.PlayOneShot(getStar);

            if (prefep_stars != null)
            {
                Instantiate(prefep_stars, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Coin)
        {
            Coin++;

            Coin objCoin = collidingObject as Coin;
            Destroy(objCoin.gameObject);
            m_Audio.PlayOneShot(getCoin);

            if (prefep_dollar != null)
            {
                Instantiate(prefep_dollar, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Apple)
        {
            Life += 10f;
            Apple objApple = collidingObject as Apple;
            Destroy(objApple.gameObject);
            m_Audio.PlayOneShot(getStar);

            if (prefep_spray != null)
            {
                Instantiate(prefep_spray, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }

        else if (collidingObject is Snake_head)
        {
            Coin++;

            m_Audio.PlayOneShot(getCoin);

            if (prefep_dollar != null)
            {
                Instantiate(prefep_dollar, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            }
        }
    }

    
    private void CollidChecker_OnExitedOBject(IObject collidingObject)
    {
        if (collidingObject is SideTrigger)
        {
            character.animator.SetTrigger(s_FallingHash);
            Life--;

            if (Math.Abs(transform.position.x) <= 13)
            {
                m_Audio.PlayOneShot(fallInWater);
                if (prefep_waterFall != null)
                {
                    if(Life >= 0)
                    Instantiate(prefep_waterFall, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
                }
            }
            else
            {
                m_Audio.PlayOneShot(fallInSide);
            }

            StartCoroutine(MovePlayer());

        }
        jumpEnable = false;
    }

    // Try Again, StageManager.Begin 시에 실행(이어하기가 아닌 초기화 후에 시작 시 실행)
    public void Init()
    {
        Life = 10.0f;
        dieFlag = false;
        endGame = false;
        StartGame();
        m_isMovable = true;
        inputUIController.DragEnd = true;
        moveDirection = Vector3.zero;
        StartCoroutine(stageManager.cameraController.Init());

        transform.eulerAngles =Vector3.zero;
        transform.position = new Vector3(0.0f, 3.0f, 0.0f);
    }

    // 사망시에 실행
    public void WaitForGameOver()
    {
        m_isMovable = false;
        inputUIController.DragEnd = true;
        moveDirection = Vector3.zero;

        //slider.value = 0;
        //transform.position = lastPosition;
    }

    public void SecondWind()
    {
        StartGame();
        Life = 10.0f;
        dieFlag = false;
        m_isMovable = true;
        transform.position = lastPosition + new Vector3(0.0f, 1.0f, 0.0f);
    }

    //Tutorial에서 실행
    public void ResetPosition()
    {
        moveDirection = Vector3.zero;
        transform.position = new Vector3(0, 5, 0);
    }

    IEnumerator MovePlayer()
    {
        
        if(!endGame)
        {
            if(inTutorial)
            {
                StartCoroutine(tutorialManager.cameraController.WaitingPlayer(waitingTime));
            }
            else
            {
                StartCoroutine(stageManager.cameraController.WaitingPlayer(waitingTime));
            }
            yield return new WaitForSeconds(waitingTime);
        }
        

        transform.position = lastPosition + new Vector3(0, 3, 0);
        transform.eulerAngles = Vector3.zero;
        moveDirection = Vector3.zero;
    }

    IEnumerator BindingPlayerPosition(Transform other)
    {
        var contactPosition = transform.position - other.position;
        jumpEnable = true;
        moveDirection = Vector3.zero;

        while (jumpEnable)
        {
            transform.position = other.position + contactPosition;
            lastPosition = other.transform.position + new Vector3(0, 1, 0);

            yield return null;
        }
    }

    public void FinishedUnmovableTime(Vector3 direction)
    {
        moveDirection = direction;
        m_isMovable = true;
    }

    void Update ()
    {
        velocity = slider.value * velocityAdjust;
                
        // 이동 기능
        MoveUpdate();

#if UNITY_EDITOR
        // test용 입력 -----------------------
        if (Input.GetKeyDown("space"))
            Life--;
        if (Input.GetKeyDown("v"))
            Life++;
        if (Input.GetKeyDown("f"))
            controller.Move(new Vector3(0f, 10.0f, 10f));
        //-------------------------------------
#endif
    }

    void MoveUpdate()
    {
        if(!m_isMovable)
        {
            moveDirection = Vector3.zero;
            return;
        }
        if(!jumpEnable)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * jumpSpeed * Time.deltaTime);

    }

    public void Jump()
    {
        moveDirection = new Vector3(0, velocity, velocity);

        if(character != null)
        {
            character.animator.SetBool(s_JumpHash, true);
        }

        if (Life >= 0)
            if(m_Audio != null)
            {
                m_Audio.PlayOneShot(jumping);
            }


        if (prefep_dust != null)
        {
            Instantiate(prefep_dust, transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        }
    }

    public void BounceFromAlocasia(float alocasiaYAngle)
    {
        transform.eulerAngles = new Vector3(0, alocasiaYAngle, 0);
        moveDirection = new Vector3(0, bounceForce, bounceForce);
    }
}
