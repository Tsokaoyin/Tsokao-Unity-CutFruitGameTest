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

    [Header("游戏设置")]
    [SerializeField] private int timeModeDuration = 90;  //时间模式时长（秒）

    private void Start()
    {
        //显示金币和最高得分
        coinsText.text = $"Coins:{PlayerPrefs.GetInt("coins,0")}";
        highScoreText.text = $"High Score:{PlayerPrefs.GetInt("highScore", 0)}";

        //设置时间按钮
        infiniteModeButton.onClick.AddListener(() => StartGame(GameViewRenderMode.mode));
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="mode"></param>
    private void StartGame(GameViewRenderMode mode)
    {
        //播放按钮音效
        AudioSource audioSource = GameManager.Instance.GetComponent<AudioSource>();
        if(buttonSound!=null)
        {
            audioSource.PlayOneShot(buttonSound);
        }

        //开始游戏
        GameManager.Instance.NewGame();
    }    
}
