using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    float h;


    public float forwardSpeed = 0;
    public float fMaxSpeed = 200;

    public float acceleration = 10;// 가속도 - 좌 우 이동시 맥스 스피드까지 도달하는 속도


    public float hCurrentSpeed;
    public float hMaxSpeed = 0;
    private float hTargetSpeed;

    private bool isRight, isLeft;


    Animator anim;

    Rigidbody rigid;

    void Start()
    {
        StartCoroutine(this.GameSpeedController());
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();

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
        forwardSpeed += Input.GetAxisRaw("Vertical") * 1f;

        h = Input.GetAxisRaw("Horizontal");
        //Debug.Log("GetAxisRaw(Horizontal) : "+Input.GetAxisRaw("Horizontal"));
        //Debug.Log("get key D : "+Input.GetKey(KeyCode.D));

        //한쪽 횡방향 입력중 다른방향 입력시 방향전환 및 횡방향 

        //오른쪽-왼쪽 은 되지만 왼쪽-오른쪽도 가능하게 코드를 짜야함 - 수정 요망



        // 좌,우 이동 버튼 입력 시 애니매이션 작동
        anim.SetBool("isRight", hCurrentSpeed > 0);
        anim.SetBool("isLeft", hCurrentSpeed < 0);

        //if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        //{
        //    anim.SetInteger("Input", (int)h);
        //}


    }

    void FixedUpdate()
    {

        //Debug.Log("Input key : " + Input.inputString);
        //Debug.Log("input d"+Input.GetKey(KeyCode.D));
        //Debug.Log("input a"+Input.GetKey(KeyCode.A));
        hTargetSpeed = h * hMaxSpeed;

        if (Input.GetKey(KeyCode.D))
        {
            //Debug.Log("왼쪽 이동중");
            if (Input.GetKey(KeyCode.A) && Input.GetAxisRaw("Horizontal") == 0)
            {
                //Debug.Log("오른쪽 진행 도중 왼쪽진행");
                hTargetSpeed = -1 * hMaxSpeed;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            //Debug.Log("오른쪽 이동중");

            if (Input.GetKey(KeyCode.D) && Input.GetAxisRaw("Horizontal") == 0)
            {
                //Debug.Log("왼쪽 진행 도중 오른쪽 진행");
                hTargetSpeed = 1 * hMaxSpeed;
            }
        }
        else
        {
            hTargetSpeed = h * hMaxSpeed;
        }

        hCurrentSpeed = IncrementSide(hCurrentSpeed, hTargetSpeed, acceleration);
        //transform.Translate(hCurrentSpeed * Time.deltaTime, 0f, forwardSpeed * Time.deltaTime, Space.World);
        rigid.MovePosition(transform.position + new Vector3(hCurrentSpeed * Time.deltaTime, 0f, forwardSpeed * Time.deltaTime));
        //rigid.AddForce(new Vector3(hCurrentSpeed * Time.deltaTime, 0f, forwardSpeed * Time.deltaTime), ForceMode.Impulse);

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
