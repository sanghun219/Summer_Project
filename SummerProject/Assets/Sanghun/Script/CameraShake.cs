using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 originPos;

    private Follow cm;

    [SerializeField]
    private float amount;

    [SerializeField]
    private float duration;

    private Player player;

    private bool isShake = false;

    private void Start()
    {
        originPos = transform.localPosition;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        cm = gameObject.GetComponent<Follow>();
        player.GameOverEvent += Shaking;
    }

    private void Update()
    {
        if (isShake == false)
            originPos = transform.localPosition;
    }

    private void Shaking()
    {
        cm.target = null;
        isShake = true;
        StartCoroutine(Shake());
    }

    public IEnumerator Shake()
    {
        float timer = 0;
        while (timer <= duration)
        {
            transform.localPosition = (Vector3)Random.insideUnitSphere * amount + originPos;
            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originPos;
    }
}