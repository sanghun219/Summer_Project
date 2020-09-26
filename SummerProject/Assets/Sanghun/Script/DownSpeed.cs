using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownSpeed : MonoBehaviour
{
    private Player player;

    [SerializeField]
    private float DownSpeedTimer;

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
        coroutine = StartCoroutine(UpdateDownSpeed());
    }

    private IEnumerator UpdateDownSpeed()
    {
        float previousTimer = 0.0f;

        while (previousTimer <= DownSpeedTimer &&
            (player.GetPlayerMode() & PlayerMode.DOWN_SPEED) != 0 && player.isGameOver == false)
        {
            yield return new WaitForFixedUpdate();
            previousTimer += Time.deltaTime;
            player.VelocityZ = Mathf.Lerp(player.VelocityZ, player.forwardSpeed, Time.deltaTime);
        }
        player.SetPlayerMode(PlayerMode.DOWN_SPEED, PlayerMode.NORMAL);
    }
}