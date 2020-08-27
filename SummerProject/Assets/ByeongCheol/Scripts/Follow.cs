using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private void LateUpdate()
    {
        if (this.target != null)
        {
            transform.position = target.position + offset;
        }
        else
        {
            transform.position = this.transform.position;
        }
    }
}