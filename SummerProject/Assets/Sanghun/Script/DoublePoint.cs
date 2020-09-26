using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DoublePoint : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private float DoublePointTimer = 3.0f;

    private Coroutine coroutine;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
            (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
        SelectCharacter.GetInstance.ChangeCharacterHandler += ChangeCharacter;
    }

    private void ChangeCharacter()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild
           (SelectCharacter.GetInstance.character).gameObject.GetComponent<Player>();
    }

    public void StartUpdate()
    {
        coroutine = StartCoroutine(UpdateDoublePoint());
    }

    private IEnumerator UpdateDoublePoint()
    {
        float previousTimer = 0.0f;
        while (previousTimer <= DoublePointTimer && player.isGameOver == false)
        {
            yield return new WaitForFixedUpdate();
            previousTimer += Time.fixedDeltaTime;
        }

        player.SetPlayerMode(PlayerMode.DOUBLE_POINT, PlayerMode.NORMAL);
    }
}