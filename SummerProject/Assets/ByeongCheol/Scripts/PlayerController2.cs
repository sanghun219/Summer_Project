using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{

    Animator anim;
    Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {

        // 충돌 발생 시 애니매이션 비활성화 및 쿼터뷰 카메라 정지
        anim.enabled = false;
        Follow vari = GameObject.Find("Main Camera").GetComponent<Follow>();
        vari.target = null;

        Debug.Log("Game Over");


    }

    private void OnCollisionExit(Collision collision)
    {
        gameObject.GetComponent<PlayerController2>().enabled = false;
    }


    //물리적 이동 관련은 FixedUpdate부분에 짜는게 좋다하여 하는중
    void FixedUpdate()
    {
        Vector3 vec = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        rigid.AddForce(vec, ForceMode.Impulse);
        Debug.Log(rigid.velocity);


    }
}
