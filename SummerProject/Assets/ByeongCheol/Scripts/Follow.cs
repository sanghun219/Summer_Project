using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Player target;
    public Vector3 offset;
    private Vector3 originRotation;

    private void Start()
    {
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangeCharacter;
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild
          (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        // target = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Player>();
        originRotation = transform.rotation.eulerAngles;
    }

    private void ChangeCharacter()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform.GetChild
            (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        originRotation = transform.rotation.eulerAngles;
    }

    private void LateUpdate()
    {
        if (target.isGameOver == false)
        {
            transform.position = target.transform.position + offset;
            if (target.isIdleState() == false)
            {
                if (target.isSuperState() == true)
                {
                    Quaternion tarQuaternion = Quaternion.Euler(originRotation);
                    transform.rotation = Quaternion.Slerp(transform.rotation, tarQuaternion, Time.deltaTime);
                }
                else
                    transform.rotation = Quaternion.Slerp(transform.rotation, target.transform.rotation, Time.deltaTime);
            }
            else
            {
                Quaternion tarQuaternion = Quaternion.Euler(originRotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, tarQuaternion, Time.deltaTime);
            }
        }
        else
        {
            Quaternion tarQuaternion = Quaternion.Euler(originRotation);
            transform.position = this.transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, tarQuaternion, Time.deltaTime);
        }
    }
}