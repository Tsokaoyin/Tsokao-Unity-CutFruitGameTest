using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("音效")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip newLifeSound;
    [SerializeField] private AudioClip coinSound;
    private AudioSource audioSource;

    [Header("游戏组件")]
    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Image fadeImage;

    [Header("UI组件")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text timeText;  //时间模式倒计时
    [SerializeField] private Text coinText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private GameObject pausePanel;

    [Header("奖牌系统")]
    [SerializeField] private GameObject bronzeMedal;
    [SerializeField] private GameObject silverMedal;
    [SerializeField] private GameObject goldMedal;
    [SerializeField] private int bronzeThreshold = 100;
    [SerializeField] private int silverThreshold = 300;
    [SerializeField] private int goldThreshold = 500;


    private void Awake()
    {
        //单例模式
        if (Instance!=null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            //初始化音效组件
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            DontDestroyOnLoad(this.gameObject);
        }

       
    }

    private void OnDestroy()
    {
        if(Instance==this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        //播放背景音乐
        //if (bgm != null)
        //{
        //    audioSource.clip = bgm;
        //    audioSource.Play();
        //}

        //显示开始界面
        ShowStartMenu();
    }

    /// <summary>
    /// 显示开始界面
    /// </summary>
    private void ShowStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
        //Time.timeScale = 0f;
    }

    /// <summary>
    /// 显示主界面
    /// </summary>
    public void ShowMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    public void NewGame(GameMode mode)
    {
        currentGameMode = mode;
    }
}
