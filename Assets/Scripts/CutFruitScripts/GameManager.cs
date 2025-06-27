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

    [Header("��Ч")]
    [SerializeField] private AudioClip bgm;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private AudioClip newLifeSound;
    [SerializeField] private AudioClip coinSound;
    private AudioSource audioSource;

    [Header("��Ϸ���")]
    [SerializeField] private Blade blade;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Image fadeImage;

    [Header("UI���")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text timeText;  //ʱ��ģʽ����ʱ
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject gameOverPanel;  //��Ϸ�������
    [SerializeField] private GameObject revivePanel;  //��Ϸ�������
    [SerializeField] private GameObject pausePanel;  //��Ϸ��ͣ���

    [Header("����ϵͳ")]
    [SerializeField] private GameObject bronzeMedal;
    [SerializeField] private GameObject silverMedal;
    [SerializeField] private GameObject goldMedal;
    [SerializeField] public int bronzeThreshold = 100;
    [SerializeField] public int silverThreshold = 300;
    [SerializeField] public int goldThreshold = 500;

    //��Ϸ״̬
    public int score { get; private set; } = 0;
    private  int currentLives;
    private int initialLives=3;
    private int coins=0;
    private int sessionCoins = 0;  ///��ǰ��Ϸ��õĽ��
    private float gameTime = 0f;  //��Ϸʱ�䣨����ʱ��ģʽ��
    [SerializeField]public  float timeLimit = 90f;  //ʱ��ģʽ��ʱ������
    private bool isPaused = false;
    private bool isRevived = false;  //�Ƿ��Ѿ�����
    private GameMode currentGameMode = GameMode.Infinite;  //Ĭ������ʱ��ģʽ

    private void Awake()
    {
        //����ģʽ
        if (Instance!=null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            //��ʼ����Ч���
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            DontDestroyOnLoad(this.gameObject);
        }

        //���ؽ�Һ���߷���
        coins = PlayerPrefs.GetInt("coins", 0);

        //�����û�г�ʼ����������Ϸ��ʼ����
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
        //���ű�������
        if (bgm != null)
        {
            audioSource.clip = bgm;
            audioSource.Play();
        }

        //��ʾ��ʼ����
       // ShowStartMenu();
    }

    /// <summary>
    /// ��ʾ��ʼ����
    /// </summary>
    private void ShowStartMenu()
    {
        SceneManager.LoadScene("StartMenu");
        //Time.timeScale = 0f;
    }

    /// <summary>
    /// ��ʾ������
    /// </summary>
    public void ShowMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    /// <summary>
    /// ��ʼ��Ϸ
    /// </summary>
    /// <param name="mode"></param>
    public void NewGame(GameMode mode)
    {
        currentGameMode = mode;
        isRevived = false;
        sessionCoins = 0;

        //������Ϸ״̬
        Time.timeScale = 1f;
        score = 0;
        currentLives = initialLives;
        gameTime = timeLimit;

        //����UI
        scoreText.text = score.ToString();
        livesText.text = $"{currentLives}";
        coinsText.text = $"{coins}";
        timeText.text = $"Time:{Mathf.CeilToInt(gameTime)}";

        gameOverPanel.SetActive(false);
        revivePanel.SetActive(false);
        pausePanel.SetActive(false);
        UpdateMedalDisplay();

        //������
        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        //������Ϸ����
        SceneManager.LoadScene("GameScene");
        audioSource.clip = bgm;
        audioSource.Play();
    }

    private void Update()
    {
        //ʱ��ģʽ����ʱ
        if(currentGameMode==GameMode.TimeLimit&&Time.timeScale>0)
        {
            gameTime -= Time.deltaTime;
            timeText.text = $"Time:{Mathf.CeilToInt(gameTime)}";

            if(gameTime<=0)
            {
                GameOver();
            }

            //��ͣ��Ϸ
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// ��ͣ/������Ϸ
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
    /// �����ҽ���
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
    /// ������
    /// </summary>
    private void ClearScene()
    {
        //ʹ�ö���ػ���������Ϸ����
        if(ObjectPool.Instance!=null)
        {
            ObjectPool.Instance.ReturnAllToPool();
        }
    }

    /// <summary>
    /// ���ӷ���
    /// </summary>
    /// <param name="points">���������õķ���</param>
    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();

        //ÿ100�ֻ��1���
        if(score%100==0)
        {
            AddCoins(1);
            sessionCoins++;
        }
    }

    /// <summary>
    /// ���ӽ��
    /// </summary>
    /// <param name="amout">�����</param>
    public void AddCoins(int amout)
    {
        coins += amout;
        PlayerPrefs.SetInt("coins", coins);
        coinsText.text = $"{coins}";

        //���Ž����Ч
        if(coinSound!=null)
        {
            audioSource.PlayOneShot(coinSound);
        }
    }

    /// <summary>
    /// ���½�����ʾ
    /// </summary>
    private void UpdateMedalDisplay()
    {
        bronzeMedal.SetActive(score >= bronzeThreshold);
        silverMedal.SetActive(score >= silverThreshold);
        goldMedal.SetActive(score >= goldThreshold);
    }

    /// <summary>
    /// ִ��ը���˺��߼�
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
    /// ��ʾ��Ϸ�������
    /// </summary>
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        if(gameOverPanel.GetComponentInChildren<Text>()!=null)
        {
            /*
             * �ַ�����ֵ��$"Game Over\nScore: {score}\nCoins Earned: {sessionCoins}"
               ʹ�� C# ���ַ�����ֵ�﷨����̬ƴ���ı����ݣ����磺
               Game Over
               Score: 100
               Coins Earned: 50
              {score} �� {sessionCoins} �Ǳ���ռλ�����ᱻʵ�ʵı���ֵ�滻��
              \n �ǻ��з����������ı��д�������Ч����
             */
            gameOverPanel.GetComponentInChildren<Text>().text =
                $"Game Over\nScore:{score}\nCoins Earned:{sessionCoins}";

        }
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="useCoins">�Ƿ�ʹ�ý�Ҹ���</param>
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
    /// �ۿ���渴��Э��
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
    /// ִ�и���
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
    /// �������˵�
    /// </summary>
    public void BackToMainMenu()
    {
        ClearScene();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0f;
    }

    
}
