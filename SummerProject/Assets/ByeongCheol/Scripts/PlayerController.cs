using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float forwardSpeed = 0;
    public float maxSpeed = 200;

    public float acceleration = 10;//속도
    public float currentSpeed;


    public float moveSpeed = 0;

    private bool isRight, isLeft;

    private float targetSpeed;

    Animator anim;


    private Coroutine inputKeyRoutine;
    private float inputLate = 0.03f;

    void Start()
    {
        StartCoroutine(GameSpeedController());
        StartCoroutine(this.SimultaniousInput());
    }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌을 시작했을 때
    private void OnCollisionEnter(Collision collision)
    {
        anim.enabled = false;
        Debug.Log("충돌 시작!");
    }

    // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌중일 때
    private void OnCollisionStay(Collision collision)
    {
        anim.enabled = false;
        Debug.Log("충돌 중!");
    }

    // Collider 컴포넌트의 is Trigger가 false인 상태로 충돌이 끝났을 때
    private void OnCollisionExit(Collision collision)
    {
        RecoverRotation();
        anim.enabled = true;
        Debug.Log("충돌 끝!");
    }

    private void RecoverRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, 0), 0.1f);
    }


    void Update()
    {

        forwardSpeed += Input.GetAxisRaw("Vertical") * 0.5f;
        targetSpeed = Input.GetAxisRaw("Horizontal") * moveSpeed;
        //Debug.Log(h);

        currentSpeed = IncrementSide(currentSpeed, targetSpeed, acceleration);
        
        // 좌우 입력이 들어오면 월드 기준 좌우 이동 아니면 셀프.. 수정요망.. 
        transform.Translate(currentSpeed * Time.deltaTime, 0f, forwardSpeed * Time.deltaTime, (Input.GetAxisRaw("Horizontal")!=0 ? Space.World:Space.Self));
        //transform.localPosition = Vector3.Lerp(transform.position, new Vector3(currentSpeed * Time.deltaTime, 0f, forwardSpeed + Time.deltaTime), Time.deltaTime);


        anim.SetBool("isRight", isRight);
        anim.SetBool("isLeft", isLeft);

    }


    private IEnumerator SimultaniousInput()
    {
        while (true)
        {
            switch (Input.inputString)
            {
                case "AD":
                case "ad":
                    //Debug.Log("AD");
                    break;
                case "A":
                case "a":
                    if (this.inputKeyRoutine == null)
                    {
                        this.inputKeyRoutine = StartCoroutine(this.InputA());
                    }
                    break;

                case "D":
                case "d":
                    if (this.inputKeyRoutine == null)
                    {
                        this.inputKeyRoutine = StartCoroutine(this.InputB());
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

        if (d == true)
        {
            //Debug.Log("A+D");
        }
        else
        {
            Debug.Log("A");
            isLeft = true;
            isRight = false;
        }
        this.inputKeyRoutine = null;
    }

    private IEnumerator InputB()
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

        if (a == true)
        {
            //Debug.Log("D+A");
        }
        else
        {
            Debug.Log("D");
            isRight = true;
            isLeft = false;

        }
        this.inputKeyRoutine = null;
    }


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



    IEnumerator GameSpeedController()
    {
        while (forwardSpeed <= maxSpeed)
        {
            forwardSpeed += 15.0f;
            //Debug.Log(dummySpeed);
            yield return new WaitForSeconds(1);
        }
    }

}
