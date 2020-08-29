using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Player target;
    public Vector3 offset;

    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void LateUpdate()
    {
        if (target.isGameOver == false)
        {
            transform.position = target.transform.position + offset;
        }
        else
        {
            transform.position = this.transform.position;
        }
    }
}