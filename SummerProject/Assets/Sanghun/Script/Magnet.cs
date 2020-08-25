using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Magnet : MonoBehaviour
{
    private Coroutine coroutine;

    [SerializeField]
    private float MagnetTimer = 3.0f;

    private Player player;

    private bool isTriggerOn = false;

    private float previousTimer = 0.0f;

    private SphereCollider collide;

    private bool test = true;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        collide = gameObject.GetComponent<SphereCollider>();
        collide.enabled = false;
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(UpdateMagnet());
    }

    private IEnumerator UpdateMagnet()
    {
        if (test == false) yield return null;
        previousTimer = 0.0f;
        ObjectManager.GetInstance.GlobalItem_opt |= SHOOT_OPT.MAGNET;

        collide.enabled = true;
        while (previousTimer <= MagnetTimer && test == true)
        {
            // TODO : 파티클이나 사운드 빠방하게 넣을수 있음!
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
        }
        ObjectManager.GetInstance.GlobalItem_opt &= ~SHOOT_OPT.MAGNET;
        Debug.Log(previousTimer + "asd");
        collide.enabled = false;
        player.SetPlayerMode(PlayerMode.NORMAL);
        float testprevious = 3.0f;
        test = false;
        while (testprevious <= 0.0f) { testprevious -= Time.deltaTime; }
        test = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Item") && collide.gameObject.activeSelf)
    //    {
    //        other.gameObject.GetComponent<Item>().shootOpt |= SHOOT_OPT.MAGNET;
    //    }
    //}
}