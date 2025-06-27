using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class MainMenuController : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Button infiniteModeButton;  //无限模式按钮
    [SerializeField] private Button timeModeButton;  //时间模式按钮
    [SerializeField] private Text coinsText;  //金币显示
    [SerializeField] private Text highScoreText;  //高分数显示
    [SerializeField] private AudioClip buttonSound;  //按钮声音
    [SerializeField] private AudioClip MainMenuMusic;  //主界面音乐
    AudioSource audioSource;

    [Header("游戏设置")]
    [SerializeField] private int timeModeDuration = 90;  //时间模式时长（秒）

    private void Start()
    {
        //显示金币和最高得分
        coinsText.text = $"{PlayerPrefs.GetInt("coins",0)}";
        highScoreText.text = $"{PlayerPrefs.GetInt("highScore",0)}";

        //设置时间按钮
        if(infiniteModeButton!=null)
        {
            infiniteModeButton.onClick.AddListener(() => StartGame(GameMode.Infinite));
        }
        if(timeModeButton!=null)
        {
            timeModeButton.onClick.AddListener(() => StartGame(GameMode.TimeLimit));
        }

        //设置时间模式时长
        GameManager.Instance.timeLimit = timeModeDuration;

        audioSource = GameManager.Instance.GetComponent<AudioSource>();
        audioSource.clip = MainMenuMusic;
        audioSource.Play();
        audioSource.loop = true;
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="mode">游戏模式</param>
    private void StartGame(GameMode mode)
    {
        //播放按钮音效
         audioSource = GameManager.Instance.GetComponent<AudioSource>();
        if(buttonSound!=null)
        {
            audioSource.PlayOneShot(buttonSound);
        }
        audioSource.Pause();
        //开始游戏
        GameManager.Instance.NewGame(mode);
    }    
}
