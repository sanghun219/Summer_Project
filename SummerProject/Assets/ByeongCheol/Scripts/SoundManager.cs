using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Sound
{
    public string name; // 곡 이름
    public AudioClip clip;// 곡
}

public class SoundManager : MonoBehaviour
{
    static public SoundManager instance;

    //public Button soundBtn;
    private bool bToggleSM = true;

    //싱글턴화

    #region singleton

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion singleton

    public AudioSource[] audioSourceEffects;
    public AudioSource audioSourceBgm;

    public string[] playSoundName;

    [SerializeField]
    public Sound[] effectSounds;

    public Sound[] bgmSounds;

    private void Start()
    {
        playSoundName = new string[audioSourceEffects.Length];
    }

    public void PlaySE(string _name)
    {
        if (!bToggleSM)
        {
            Debug.Log("사운드가 꺼져있습니다");
            return;
        }
        else if (bToggleSM)
        {
            for (int i = 0; i < effectSounds.Length; i++)
            {
                if (_name == effectSounds[i].name)
                {
                    playSoundName[i] = effectSounds[i].name;
                    audioSourceEffects[i].clip = effectSounds[i].clip;
                    audioSourceEffects[i].Play();
                    //for (int j = 0; j < audioSourceEffects.Length; j++)
                    //{
                    //    if (!audioSourceEffects[j].isPlaying)
                    //    {
                    //        return;
                    //    }
                    //    //Debug.Log("모든 가용 AudioSource가 사용중입니다.");
                    //    Debug.Log("사운드 실행");
                    //    return;
                    //}
                }
            }
            //Debug.Log(_name + "사운드가 SoundManager에 등록되지 않았습니다.");
        }
    }

    public void StopAllSE()
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            audioSourceEffects[i].Stop();
        }
    }

    public void StopSE(string _name)
    {
        for (int i = 0; i < audioSourceEffects.Length; i++)
        {
            if (playSoundName[i] == _name)
            {
                audioSourceEffects[i].Stop();
                break;
            }
        }
        Debug.Log("재생 중인" + _name + "사운드가 없습니다.");
    }

    public void ToggleSound()
    {
        bToggleSM = !bToggleSM;
        Debug.Log("bToggleSM : " + bToggleSM);
    }
}

/*
 *
 해당 스크립트에서 변수 선언

 [SerializedField]
 private string 변수이름;

  해당 스크립트 콜리전 등 함수에
  SoundManager.instance.PlaySE(변수이름);

  후에 인스펙터 창에서 변수 이름 칸에 오디오 소스 이름을 적어넣어준다.
     */