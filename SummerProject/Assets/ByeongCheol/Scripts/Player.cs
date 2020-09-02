using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor;
using UnityEngine;

[Flags]
public enum PlayerMode
{
    NORMAL = 0x00000001,
    PUSH = 0x00000002,
    DOUBLE_POINT = 0x00000004,
    SUPER = 0x00000008,
    MAGNET = 0x00000010,
    DOWN_SPEED = 0x00000020,
}

public class Player : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rigid;

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
    public float hCurrentSpeed { get; set; }

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

    private DownSpeed downSpeed;

    // Restart용 변수
    private float re_forwardSpeed;

    private Coroutine SpeedControllerCoroutine;

    public void ReStart()
    {
        gameObject.GetComponent<Rigidbody>().gameObject.SetActive(false);

        transform.position = new Vector3(0, 0, 0);
        transform.rotation = Quaternion.identity;
        playerMode = PlayerMode.NORMAL;
        isGameOver = false;
        anim.enabled = true;
        forwardSpeed = re_forwardSpeed;
        hCurrentSpeed = 0;
        h = 0;
        VelocityZ = 0;
        gameObject.GetComponent<Collider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().gameObject.SetActive(true);
        rigid.velocity = new Vector3(0, 0, 0);
        rigid.angularVelocity = new Vector3(0, 0, 0);
        gameObject.GetComponent<Player>().enabled = true;
        SpeedControllerCoroutine = StartCoroutine(GameSpeedController());
    }

    public PlayerMode GetPlayerMode()
    {
        return playerMode;
    }

    public bool isIdleState()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return true;
        return false;
    }

    public bool isSuperState()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("SuperMode")) return true;
        return false;
    }

    public void SetPlayerMode(PlayerMode prevMode, PlayerMode nextMode)
    {
        if (playerModeToAction.ContainsKey(nextMode))
        {
            playerMode &= ~prevMode;
            playerModeToAction[nextMode]();
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
        downSpeed = playerModeObj.transform.Find("DownSpeed").GetComponent<DownSpeed>();

        playerModeToAction[PlayerMode.PUSH] = () =>
        {
            if (pushObj.gameObject.activeSelf == true && (playerMode & PlayerMode.PUSH) != 0) return;
            playerMode |= PlayerMode.PUSH;
            pushObj.gameObject.SetActive(true);
            pushObj.StartUpdate();
        };

        playerModeToAction[PlayerMode.NORMAL] = () =>
        {
            if ((playerMode & PlayerMode.DOUBLE_POINT) == 0)
            {
                doublePoint.gameObject.SetActive(false);
            }
            if ((playerMode & PlayerMode.DOWN_SPEED) == 0)
            {
                downSpeed.gameObject.SetActive(false);
            }
            if ((playerMode & PlayerMode.MAGNET) == 0)
            {
                magnet.gameObject.SetActive(false);
            }
            if ((playerMode & PlayerMode.PUSH) == 0)
            {
                pushObj.gameObject.SetActive(false);
            }
            if ((playerMode & PlayerMode.SUPER) == 0)
            {
                superMode.gameObject.SetActive(false);
                anim.SetBool("superMode", false);
            }
        };

        playerModeToAction[PlayerMode.SUPER] = () =>
        {
            if (superMode.gameObject.activeSelf == true && (playerMode & PlayerMode.SUPER) != 0) return;
            playerMode &= ~PlayerMode.MAGNET;
            playerMode |= PlayerMode.SUPER;
            if ((playerMode & PlayerMode.DOWN_SPEED) != 0)
                playerMode &= ~PlayerMode.DOWN_SPEED;
            anim.SetBool("superMode", true);
            superMode.gameObject.SetActive(true);
            superMode.StartUpdate();
        };

        playerModeToAction[PlayerMode.MAGNET] = () =>
        {
            if (magnet.gameObject.activeSelf && (playerMode & PlayerMode.MAGNET) != 0
            && (playerMode & PlayerMode.SUPER) == 0) return;
            playerMode |= PlayerMode.MAGNET;
            magnet.gameObject.SetActive(true);
            magnet.StartUpdate();
        };

        playerModeToAction[PlayerMode.DOUBLE_POINT] = () =>
        {
            if (doublePoint.gameObject.activeSelf == true && (playerMode & PlayerMode.DOUBLE_POINT) != 0) return;
            playerMode |= PlayerMode.DOUBLE_POINT;
            doublePoint.gameObject.SetActive(true);
            doublePoint.StartUpdate();
        };

        playerModeToAction[PlayerMode.DOWN_SPEED] = () =>
        {
            if (downSpeed.gameObject.activeSelf == true && (playerMode & PlayerMode.DOWN_SPEED) != 0
            || (playerMode & PlayerMode.SUPER) != 0) return;

            playerMode |= PlayerMode.DOWN_SPEED;
            downSpeed.gameObject.SetActive(true);
            downSpeed.StartUpdate();
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
            gameObject.GetComponent<Collider>().enabled = false;
            Debug.Log("Game Over");
            StopCoroutine(SpeedControllerCoroutine);
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

    public void AwakePlayer()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerMode = PlayerMode.NORMAL;
        re_forwardSpeed = forwardSpeed;
    }

    public void PlayerFixedUpdate()
    {
        MoveHorizontal(hCurrentSpeed);

        //전진만 하게끔 vertical 입력 값 양수일경우에만 가속
        if (v > 0)
        {
            AccelerateForward();
        }
    }

    public void PlayerUpdate()
    {
        // h = Input.GetAxisRaw("Horizontal");
        // v = Input.GetAxisRaw("Vertical");
    }

    public void StartPlayer()
    {
        //플레이어 고정 속도증가 코루틴

        SpeedControllerCoroutine = StartCoroutine(GameSpeedController());
        playerMode = PlayerMode.NORMAL;
        playerModeObj = gameObject.transform.Find("PlayerMode");
        InitPlayerMode();
    }

    // 게임 플레이 시작 시 일정 시간에 따라 속도 증가 < MaxSpeed
    //200을 넘어갈 경우 200에 고정하게끔 코드 수정 필요 2020.8.17
    private IEnumerator GameSpeedController()
    {
        while (true)
        {
            yield return null;

            if (rigid.velocity.z <= fMaxSpeed && (playerMode & PlayerMode.DOWN_SPEED) == 0)
                rigid.AddForce(new Vector3(0, 0, forwardSpeed), ForceMode.Force);
        }
    }

    //플레이어 가속 함수
    private void AccelerateForward()
    {
        if ((playerMode & PlayerMode.DOWN_SPEED) != 0)
            rigid.AddForce(new Vector3(0, 0, AccForward));

        //Debug.Log("Acc : Velocity of Z : " + rigid.velocity.z);
    }

    //좌우로 이동을 수행해주는 함수
    private void MoveHorizontal(float c)
    {
        //현재 속도를 키 입력이 눌릴때만 증가하고 누르지 않을경우 천천히 0으로 돌아오게끔 한다.
        hCurrentSpeed = IncrementSide(hCurrentSpeed, h * hMaxSpeed, hAcceleration);
        this.rigid.MovePosition(this.transform.position + new Vector3(c, 0, 0) * Time.deltaTime);
        if (playerMode == PlayerMode.SUPER)
        {
            if (anim.GetBool("isLeft") || anim.GetBool("isRight"))
            {
                anim.StopPlayback();
                anim.SetBool("isLeft", false);
                anim.SetBool("isRight", false);
            }
            return;
        }

        //if (Application.platform == RuntimePlatform.WindowsEditor)
        //{
        //    if (!Input.anyKey || h == 0)
        //    {
        //        anim.SetBool("isLeft", false);
        //        anim.SetBool("isRight", false);
        //    }
        //    else if (Input.GetKey(KeyCode.LeftArrow) || h < 0)
        //    {
        //        anim.SetBool("isLeft", true);
        //        anim.SetBool("isRight", false);
        //    }
        //    else if (Input.GetKey(KeyCode.RightArrow) || h > 0)
        //    {
        //        anim.SetBool("isRight", true);
        //        anim.SetBool("isLeft", false);
        //    }
        //    if ((Input.GetKeyUp(KeyCode.LeftArrow)) || h > 0)
        //    {
        //        anim.SetBool("isLeft", false);
        //        h = 0;
        //    }
        //    if (Input.GetKeyUp(KeyCode.RightArrow) || h < 0)
        //    {
        //        anim.SetBool("isRight", false);
        //        h = 0;
        //    }
        //}

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 playerPos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 touchPos = new Vector2(touch.position.x, touch.position.y);

            if (touch.phase == TouchPhase.Began)
            {
                if (touchPos.x < playerPos.x)
                {
                    h = -1;
                    anim.SetBool("isLeft", true);
                    anim.SetBool("isRight", false);
                }
                else if (touchPos.x > playerPos.x)
                {
                    h = 1;
                    anim.SetBool("isRight", true);
                    anim.SetBool("isLeft", false);
                }
                else
                {
                    h = 0;
                }
            }
            else if (touch.phase == TouchPhase.Stationary
                || touch.phase == TouchPhase.Moved)
            {
            }
        }
        else
        {
            anim.SetBool("isLeft", false);
            anim.SetBool("isRight", false);
            h = 0;
        }
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
}