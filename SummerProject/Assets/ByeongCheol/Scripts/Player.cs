using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PlayerMode
{
    NORMAL,
    PUSH,
    DOUBLE_POINT,
    SUPER,
    MAGNET,
}

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rigid;


    int score = 0;
    public Text scoreText;


    //횡, 수직 이동 입력 변수
    private float v, h;

    // 플레이어 고정 속도 증가 offset
    public float forwardSpeed = 0;

    //플레이어 고정 속도 증가 상한 offset
    public float fMaxSpeed = 0;

    // 플레이어 가속 offset
    public float AccForward = 0;

    //횡 이동 최고속도까지 도달하는 속도
    public float hAcceleration = 0;

    //횡 이동 최고속도
    public float hMaxSpeed = 0;

    //횡 이동 플레이어의 현재 속도
    public float hCurrentSpeed;

    public bool isGameOver = false;

    public delegate void GameOverHandler();

    // 다른 클래스들이 플레이어가 죽었을 때 이벤트 발생시키도록 함
    // 조건 발생시 딱 한 번만 호출하기 때문에 성능up, 코드가 깔끔해짐
    public static event GameOverHandler GameOverEvent;

    // 플레이어가 특정 아이템 먹었을 경우 상태가 바뀜
    private PlayerMode playerMode;

    // 플레이어 자식중에 플레이어 모드 오브젝트를 캐싱하기 위함
    private Transform playerModeObj;

    // switch - case문 대신 Dictionary의 value로 델리게이트(함수포인터)
    // 넣어주면 코드가 깔끔해지고 모듈 독립성이 높아진다.
    private Dictionary<PlayerMode, Action> playerModeToAction;

    private PushObj pushObj;

    private DoublePoint doublePoint;

    private SuperMode superMode;

    private Magnet magnet;

    public PlayerMode GetPlayerMode()
    {
        return playerMode;
    }

    public void SetPlayerMode(PlayerMode mode)
    {
        if (playerModeToAction.ContainsKey(mode))
        {
            playerMode = mode;
            playerModeToAction[mode]();
        }
        else
        {
            Debug.Log("해당 모드가 지정되어 있지 않군요..");
        }
    }

    private void InitPlayerMode()
    {
        playerModeToAction = new Dictionary<PlayerMode, Action>();
        pushObj = playerModeObj.transform.Find("PushObjects").GetComponent<PushObj>();
        superMode = playerModeObj.transform.Find("SuperMode").GetComponent<SuperMode>();
        doublePoint = playerModeObj.transform.Find("DoublePoint").GetComponent<DoublePoint>();
        magnet = playerModeObj.transform.Find("Magnet").GetComponent<Magnet>();

        playerModeToAction[PlayerMode.PUSH] = () =>
        {
            if (pushObj.gameObject.activeSelf == true) return;

            pushObj.gameObject.SetActive(true);
            pushObj.StartUpdate();
        };

        playerModeToAction[PlayerMode.NORMAL] = () =>
        {
            int childCount = playerModeObj.childCount;
            for (int i = 0; i < childCount; i++)
            {
                if (playerModeObj.GetChild(i).gameObject.activeSelf == true)
                {
                    playerModeObj.GetChild(i).gameObject.SetActive(false);
                }
            }
        };

        playerModeToAction[PlayerMode.SUPER] = () =>
        {
            if (superMode.gameObject.activeSelf == true) return;
            superMode.gameObject.SetActive(true);
            superMode.StartUpdate();
        };

        playerModeToAction[PlayerMode.MAGNET] = () =>
        {
            if (magnet.gameObject.activeSelf == true) return;
            Debug.Log("ㅁㄴㅇㅁㄴㅇ;");
            magnet.gameObject.SetActive(true);
            magnet.StartUpdate();
        };

        playerModeToAction[PlayerMode.DOUBLE_POINT] = () =>
        {
            if (doublePoint.gameObject.activeSelf == true) return;
            doublePoint.gameObject.SetActive(true);
            doublePoint.StartUpdate();
        };
    }

    // GameOverEvent를 통해 나중에 플레이어가 게임오버 됐을때 다른 클래스들이
    // 콜백체인을 통해 각 기능을 바로 동작시킬 수 있음
    public void GameOver()
    {
        if (GameOverEvent != null)
        {
            isGameOver = true;
            anim.enabled = false;

            Debug.Log("Game Over");
            GameOverEvent();
        }
    }

    public float VelocityZ
    {
        get { return rigid.velocity.z; }
        set
        {
            rigid.velocity = new Vector3(0, 0, value);
        }
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    public void PlayerFixedUpdate()
    {
        MoveHorizontal(hCurrentSpeed);

        //스코어 증가
        //score += (int)rigid.velocity.z;
        //Debug.Log(score);
        //scoreText.text = score.ToString();

        //전진만 하게끔 vertical 입력 값 양수일경우에만 가속
        if (v > 0)
        {
            AccelerateForward();
        }

        anim.SetBool("isRight", hCurrentSpeed > 0);
        anim.SetBool("isLeft", hCurrentSpeed < 0);
    }

    public void PlayerUpdate()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }

    private void Start()
    {
        //플레이어 고정 속도증가 코루틴
        StartCoroutine(this.GameSpeedController());
        playerMode = PlayerMode.NORMAL;
        playerModeObj = gameObject.transform.Find("PlayerMode");
        InitPlayerMode();
    }

    // 플레이어 이동
    // 전진 이동은 rigidbody에 가속도를 더함
    // 좌 우 이동은 rigidbody의 moveposition함수 사용

    private void OnCollisionStay(Collision collision)
    {
        //물체와 충돌시 질질끌림현상이 발생하여 콜리전 진입후에 플레이어의 콜리전 컴포넌트와 스크립트를 비활성화
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
            gameObject.GetComponent<Player>().enabled = false;

            GameOver();
        }
    }

    // 게임 플레이 시작 시 일정 시간에 따라 속도 증가 < MaxSpeed
    //200을 넘어갈 경우 200에 고정하게끔 코드 수정 필요 2020.8.17
    private IEnumerator GameSpeedController()
    {
        while (rigid.velocity.z <= fMaxSpeed)
        {
            rigid.AddForce(new Vector3(0, 0, forwardSpeed), ForceMode.Impulse);
            yield return null;
        }
    }

    //플레이어 가속 함수
    private void AccelerateForward()
    {
        rigid.AddForce(new Vector3(0, 0, AccForward));

        //Debug.Log("Acc : Velocity of Z : " + rigid.velocity.z);
    }

    //좌우로 이동을 수행해주는 함수
    private void MoveHorizontal(float c)
    {
        //현재 속도를 키 입력이 눌릴때만 증가하고 누르지 않을경우 천천히 0으로 돌아오게끔 한다.
        hCurrentSpeed = IncrementSide(hCurrentSpeed, h * hMaxSpeed, hAcceleration);

        //rigidbody 의 movepoint와 transform의 translate 비교중
        this.rigid.MovePosition(this.transform.position + new Vector3(c, 0, 0) * Time.deltaTime);
    }

    // 좌우 이동 시 가속도 적용 함수
    private float IncrementSide(float n, float target, float a)
    {
        if (n == target)
        {
            return n;
        }
        else
        {
            //방향 Sign -> 음수 면 - 1 양수거나 0이면 1 반환
            float dir = Mathf.Sign(target - n); // must n be increased or decreased to get closer to target
            n += a * Time.deltaTime * dir;
            return (dir == Mathf.Sign(target - n)) ? n : target; // if n has now passed target then return target, otherwise return n
        }
    }

    //메인 UI의 시작버튼을 누르면 Player의 스크립트 활성화
    public void OnGameStart()
    {
        gameObject.GetComponent<Player>().enabled = true;
    }

    //게임오버화면에서 화면 클릭 시 씬 리로드
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



}