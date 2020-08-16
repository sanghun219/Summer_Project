using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{

    Animator anim;
    Rigidbody rigid;

    //횡, 수직 이동 입력 변수
    float v, h;


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


    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        //플레이어 고정 속도증가 코루틴
        StartCoroutine(this.GameSpeedController());
    }

    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
    }


    // 플레이어 이동
    // 전진 이동은 rigidbody에 가속도를 더함
    // 좌 우 이동은 rigidbody의 moveposition함수 사용
    void FixedUpdate()
    {
        MoveHorizontal(hCurrentSpeed);

        //전진만 하게끔 vertical 입력 값 양수일경우에만 가속
        if (v > 0)
        {
            AccelerateForward();
        }
        //Debug.Log(rigid.velocity);


    }

    private void OnCollisionEnter(Collision collision)
    {

        // 충돌 발생 시 애니매이션 비활성화 및 쿼터뷰 카메라 정지
        anim.enabled = false;
        Follow vari = GameObject.Find("Main Camera").GetComponent<Follow>();
        vari.target = null;

        Debug.Log("Game Over");


    }
    private void OnCollisionStay(Collision collision)
    {
        //물체와 충돌시 질질끌림현상이 발생하여 콜리전 진입후에 플레이어의 콜리전 컴포넌트와 스크립트를 비활성화
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
        gameObject.GetComponent<PlayerController2>().enabled = false;
    }
    private void OnCollisionExit(Collision collision)
    {

    }





    // 게임 플레이 시작 시 일정 시간에 따라 속도 증가 < MaxSpeed
    //200을 넘어갈 경우 200에 고정하게끔 코드 수정 필요 2020.8.17
    IEnumerator GameSpeedController()
    {
        while (rigid.velocity.z <= fMaxSpeed)
        {
            rigid.AddForce(new Vector3(0, 0, forwardSpeed) * Time.deltaTime, ForceMode.Impulse);

            Debug.Log("Velocity of Z : " + rigid.velocity.z);

            yield return new WaitForSeconds(1);
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
        //transform.Translate(this.transform.position + new Vector3(c, 0, 0) * Time.deltaTime, Space.World);
        //this.rigid.velocity.Set(hCurrentSpeed, 0, 0);

        //Debug.Log("hCurrentSpeed : "+hCurrentSpeed);

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
