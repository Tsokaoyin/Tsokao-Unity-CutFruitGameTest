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
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject gameOverPanel;  //游戏结束面板
    [SerializeField] private GameObject revivePanel;  //游戏复活面板
    [SerializeField] private GameObject pausePanel;  //游戏暂停面板

    [Header("奖牌系统")]
    [SerializeField] private GameObject bronzeMedal;
    [SerializeField] private GameObject silverMedal;
    [SerializeField] private GameObject goldMedal;
    [SerializeField] private int bronzeThreshold = 100;
    [SerializeField] private int silverThreshold = 300;
    [SerializeField] private int goldThreshold = 500;

    //游戏状态
    public int score { get; private set; } = 0;
    private  int currentLives;
    private int initialLives=3;
    private int coins;
    private int sessionCoins = 0;  ///当前游戏获得的金币
    private float gameTime = 0f;  //游戏时间（用于时间模式）
    private float timeLimit = 90f;  //时间模式的时间限制
    private bool isPaused = false;
    private bool isRevived = false;  //是否已经复活
    private GameMode currentGameMode = GameMode.Infinite;  //默认无限时间模式

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

        //加载金币和最高分数
        coins = PlayerPrefs.GetInt("coins", 0);
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

    /// <summary>
    /// 开始游戏
    /// </summary>
    /// <param name="mode"></param>
    public void NewGame(GameMode mode)
    {
        currentGameMode = mode;
        isRevived = false;
        sessionCoins = 0;

        //重置游戏状态
        Time.timeScale = 1f;
        score = 0;
        currentLives = initialLives;
        gameTime = timeLimit;

        //更新UI
        scoreText.text = score.ToString();
        livesText.text = $"Lives:{currentLives}";
        coinsText.text = $"Coins:{coins}";
        timeText.text = $"Time:{Mathf.CeilToInt(gameTime)}";

        gameOverPanel.SetActive(false);
        revivePanel.SetActive(false);
        pausePanel.SetActive(false);
        UpdateMedalDisplay();

        //清理场景
        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        //加载游戏场景
        SceneManager.LoadScene("GameScene");
    }

    private void Update()
    {
        //时间模式倒计时
        if(currentGameMode==GameMode.TimeLimit&&Time.timeScale>0)
        {
            gameTime -= Time.deltaTime;
            timeText.text = $"Time:{Mathf.CeilToInt(gameTime)}";

            if(gameTime<=0)
            {

            }
        }
    }

    private void GameOve()
    {
        
    }

    private void ClearScene()
    {
        //使用对象池回首所有游戏
    }

    /// <summary>
    /// 增加分数
    /// </summary>
    /// <param name="points">消除对象获得的分数</param>
    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        //每100分获得1金币
        if(score%100==0)
        {
            AddCoins(1);
            sessionCoins++;
        }
    }

    public void AddCoins(int amout)
    {
        coins += amout;
        PlayerPrefs.SetInt("coins", coins);
        coinsText.text = $"Coins:{coins}";

        //播放金币音效
        if(coinSound!=null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    /// <summary>
    /// 更新奖牌显示
    /// </summary>
    private void UpdateMedalDisplay()
    {
        bronzeMedal.SetActive(score >= bronzeThreshold);
        silverMedal.SetActive(score >= silverThreshold);
        goldMedal.SetActive(score >= goldThreshold);
    }
}
