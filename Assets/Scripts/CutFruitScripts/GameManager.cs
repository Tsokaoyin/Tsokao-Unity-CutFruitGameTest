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
    [SerializeField] public int bronzeThreshold = 100;
    [SerializeField] public int silverThreshold = 300;
    [SerializeField] public int goldThreshold = 500;

    //游戏状态
    public int score { get; private set; } = 0;
    private  int currentLives;
    private int initialLives=3;
    private int coins=0;
    private int sessionCoins = 0;  ///当前游戏获得的金币
    private float gameTime = 0f;  //游戏时间（用于时间模式）
    [SerializeField]public  float timeLimit = 90f;  //时间模式的时间限制
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

        //如果还没有初始化，加载游戏初始场景
        if(SceneManager.GetActiveScene().name!="StartMenu")
        {
            SceneManager.LoadScene("StartMenu");
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
        if (bgm != null)
        {
            audioSource.clip = bgm;
            audioSource.Play();
        }

        //显示开始界面
       // ShowStartMenu();
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
        livesText.text = $"{currentLives}";
        coinsText.text = $"{coins}";
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
        audioSource.clip = bgm;
        audioSource.Play();
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
                GameOver();
            }

            //暂停游戏
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// 暂停/继续游戏
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    private void GameOver()
    {
       blade.enabled = false;
        spawner.enabled = false;

        if(gameOverSound!=null)
        {
            AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position);

        }

        int coinsEarned = CalculateCoinsEarned();
        sessionCoins += coinsEarned;
        AddCoins(coinsEarned);

        if(!isRevived)
        {
            revivePanel.SetActive(true);
            if(revivePanel.GetComponentInChildren<Text>()!=null)
            {
                revivePanel.GetComponentInChildren<Text>().text = $"Earned{sessionCoins}coins!\nRevive?";
            }
            else
            {
                ShowGameOver();
            }
        }
    }

    /// <summary>
    /// 计算金币奖励
    /// </summary>
    /// <returns></returns>
    private int CalculateCoinsEarned()
    {
        int baseCoins = score / 100;
        int timeBonus = 0;
        int medalBouns = 0;

        if(currentGameMode==GameMode.TimeLimit)
        {
            timeBonus = Mathf.CeilToInt(gameTime) / 10;
        }

        if (score >= goldThreshold)
        {
            medalBouns = 5;
        }
        else if (score >= silverThreshold) medalBouns = 3;
        else if (score >= bronzeThreshold) medalBouns = 1;

        return baseCoins + timeBonus + medalBouns;
    }

    /// <summary>
    /// 清理场景
    /// </summary>
    private void ClearScene()
    {
        //使用对象池回收所有游戏对象
        if(ObjectPool.Instance!=null)
        {
            ObjectPool.Instance.ReturnAllToPool();
        }
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

    /// <summary>
    /// 增加金币
    /// </summary>
    /// <param name="amout">金币数</param>
    public void AddCoins(int amout)
    {
        coins += amout;
        PlayerPrefs.SetInt("coins", coins);
        coinsText.text = $"{coins}";

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

    /// <summary>
    /// 执行炸弹伤害逻辑
    /// </summary>
    public void BombExploded()
    {
        currentLives--;
        livesText.text = $"Lives:{currentLives}";

        if(currentLives<=0)
        {
            GameOver();
        }
    }

    /// <summary>
    /// 显示游戏结束面板
    /// </summary>
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        if(gameOverPanel.GetComponentInChildren<Text>()!=null)
        {
            /*
             * 字符串插值：$"Game Over\nScore: {score}\nCoins Earned: {sessionCoins}"
               使用 C# 的字符串插值语法，动态拼接文本内容，例如：
               Game Over
               Score: 100
               Coins Earned: 50
              {score} 和 {sessionCoins} 是变量占位符，会被实际的变量值替换。
              \n 是换行符，用于在文本中创建多行效果。
             */
            gameOverPanel.GetComponentInChildren<Text>().text =
                $"Game Over\nScore:{score}\nCoins Earned:{sessionCoins}";

        }
    }

    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="useCoins">是否使用金币复活</param>
    public void Revive(bool useCoins)
    {
        if(useCoins)
        {
            int reviveCost = 50;
            if(coins>=reviveCost)
            {
                coins -= reviveCost;
                PlayerPrefs.SetInt("coins", coins);
                coinsText.text = $"Coins:{coins}";
                DoRevive();
            }
        }
        else
        {
            StartCoroutine(WatchAdAndRevive());
        }
    }

    /// <summary>
    /// 观看广告复活协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator WatchAdAndRevive()
    {
        Text reviveText = revivePanel.GetComponentInChildren<Text>();
        if(reviveText!=null)
        {
            reviveText.text = "Watching Ad...";
        }
        yield return new WaitForSecondsRealtime(3f);
        DoRevive();
    }

    /// <summary>
    /// 执行复活
    /// </summary>
    private void DoRevive()
    {
        isRevived = true;
        revivePanel.SetActive(false);
        currentLives = 1;
        livesText.text = $"Lives:{currentLives}";

        if (blade != null) blade.ResetPosition();
        blade.enabled = true;
        spawner.enabled = true;

        if (newLifeSound != null) audioSource.PlayOneShot(newLifeSound);
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void BackToMainMenu()
    {
        ClearScene();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    
}
