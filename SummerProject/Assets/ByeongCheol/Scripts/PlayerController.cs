using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float forwardSpeed = 0;
    public float fMaxSpeed = 200;

    public float acceleration = 10;// 가속도 - 좌 우 이동시 맥스 스피드까지 도달하는 속도


    public float hCurrentSpeed;
    public float hMaxSpeed = 0;
    private float hTargetSpeed;

    private bool isRight, isLeft;


    Animator anim;


    private Coroutine inputKeyRoutine;
    private float inputLate = 0.03f;

    void Start()
    {
        StartCoroutine(this.GameSpeedController());

        //좌우 동시입력문제 해결 - 사용x 2020.8.14
        //StartCoroutine(this.SimultaniousInput());
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Player가 오브젝트와 충돌 시 에니메이터 종료
    private void OnCollisionEnter(Collision collision)
    {

        // 충돌 발생 시 애니매이션 비활성화 및 쿼터뷰 카메라 정지
        // floor 오브
        anim.enabled = false;
        Follow vari = GameObject.Find("Main Camera").GetComponent<Follow>();
        vari.target = null;

        Debug.Log("Game Over");


    }

    private void OnCollisionExit(Collision collision)
    {
        //충돌 발생 시 입력 비활성화를 위해 작성 -  수정요망
        gameObject.GetComponent<PlayerController>().enabled = false;
    }




    void Update()
    {
        // 전진 키 입력 속도증가
        forwardSpeed += Input.GetAxisRaw("Vertical") * 0.5f;


        //Debug.Log("GetAxisRaw(Horizontal) : "+Input.GetAxisRaw("Horizontal"));
        //Debug.Log("get key D : "+Input.GetKey(KeyCode.D));

        //한쪽 횡방향 입력중 다른방향 입력시 방향전환 및 횡방향 방향전환
        if (Input.GetKey(KeyCode.D) && Input.GetAxisRaw("Horizontal") == 0)
        {
            Debug.Log("오른쪽 진행 도중 왼쪽진행");
            hTargetSpeed = -1 * hMaxSpeed;
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetAxisRaw("Horizontal") == 0)
        {
            Debug.Log("왼쪽 진행 도중 오른쪽 진행");
            hTargetSpeed = 1 * hMaxSpeed;
        }
        else
        {
            hTargetSpeed = Input.GetAxisRaw("Horizontal") * hMaxSpeed;
        }


        hCurrentSpeed = IncrementSide(hCurrentSpeed, hTargetSpeed, acceleration);


        // 좌우 입력이 들어오면 월드 기준 좌우 이동 아니면 셀프.. 수정요망.. 
        transform.Translate(hCurrentSpeed * Time.deltaTime, 0f, forwardSpeed * Time.deltaTime, Space.World);
        //transform.localPosition = Vector3.Lerp(transform.position, new Vector3(hCurrentSpeed * Time.deltaTime, 0f, forwardSpeed + Time.deltaTime), Time.deltaTime);

        // 좌,우 이동 버튼 입력 시 애니매이션 작동
        anim.SetBool("isRight", hCurrentSpeed > 0);
        anim.SetBool("isLeft", hCurrentSpeed < 0);

    }




    // 좌(우) 키 입력 도중 우(좌)입력 시 입력 값이 0(좌, 우 이동 둘다 안됨)을 하지않게 하는 함수
    /*
    private IEnumerator SimultaniousInput()
    {
        while (true)
        {
            switch (Input.inputString)
            {
                //case "AD":
                //case "ad":
                //    //Debug.Log("AD");
                //    break;


                case "A":
                case "a":
                    if (this.inputKeyRoutine == null)
                    {
                        //Debug.Log("A 입력 함수 실행");
                        this.inputKeyRoutine = StartCoroutine(this.InputA());
                    }
                    break;

                case "D":
                case "d":
                    if (this.inputKeyRoutine == null)
                    {
                        //Debug.Log("D 입력 함수 실행");
                        this.inputKeyRoutine = StartCoroutine(this.InputD());
                    }
                    break;



            }
            yield return null;
        }
    }
    private IEnumerator InputA()
    {
        float i = this.inputLate;
        bool d = false;
        while (i > 0)
        {
            if (Input.inputString == "D" || Input.inputString == "d")
            {
                //Debug.Log("추가 입력!");
                d = true;
            }

            i -= Time.deltaTime;
            yield return null;
        }

        if(d == false)
        {
            //Debug.Log("A");
            isLeft = true;
            isRight = false;
        }
        this.inputKeyRoutine = null;
    }
    private IEnumerator InputD()
    {
        float i = this.inputLate;
        bool a = false;
        while (i > 0)
        {
            if (Input.inputString == "A" || Input.inputString == "a")
            {
                //Debug.Log("추가 입력!");
                a = true;
            }

            i -= Time.deltaTime;
            yield return null;
        }

        //if (a == true)
        //{
        //    //Debug.Log("D+A");
        //}
        //else
        if(a==false)
        {
            //Debug.Log("D");
            isRight = true;
            isLeft = false;

        }
        this.inputKeyRoutine = null;
    }
    */


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


    // 게임 플레이 시작 시 일정 시간에 따라 속도 증가 < MaxSpeed
    IEnumerator GameSpeedController()
    {
        while (forwardSpeed <= fMaxSpeed)
        {
            forwardSpeed += 15.0f;
            yield return new WaitForSeconds(1);
        }
    }

}
