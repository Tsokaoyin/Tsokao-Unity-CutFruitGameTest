using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("游戏界面组件")]
    [SerializeField] private Text scoreText;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text coinsText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject revivePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject bronzeMedal;
    [SerializeField] private GameObject silverMedal;
    [SerializeField] private GameObject goldMedal;

    [Header("主界面组件")]
    [SerializeField]private Text mainMenuCoinsText;
    [SerializeField] private Text mainMenuHighScoreText;

    [Header("复活选项")]
    [SerializeField] private Button coinReviveButton;
    [SerializeField] private Button adReviveButton;
    [SerializeField] private Button noReviveButton;

    private void Start()
    {
        //设置复活按钮事件
        if(coinReviveButton!=null)
        {
            coinReviveButton.onClick.AddListener(() => GameManager.Instance.Revive(true));
        }
        if (adReviveButton!=null)
        {
            adReviveButton.onClick.AddListener(() => GameManager.Instance.Revive(false));
        }
        if(noReviveButton!=null)
        {
            noReviveButton.onClick.AddListener(() => GameManager.Instance.ShowGameOver());
        }
    }

    /// <summary>
    /// 更新游戏UI
    /// </summary>
    public  void UpdateGameUI(int score,int lives,int coins,float time)
    {
        if(scoreText!=null)
        {
            scoreText.text = scoreText.ToString();
        }
        if(livesText!=null)
        {
            livesText.text = $"Lives:{lives}";
        }
        if(coinsText!=null)
        {
            coinsText.text = $"Coins:{coins}";
        }
        if(timeText!=null&&time>=0)
        {
            timeText.text = $"Time:{Mathf.CeilToInt(time)}";
        }
        UpdateMedals(score);
    }

    /// <summary>
    /// 更新奖牌显示
    /// </summary>
    /// <param name="score">分数</param>
    public void UpdateMedals(int score)
    {
        if(bronzeMedal!=null)
        {
            bronzeMedal.SetActive(score >= GameManager.Instance.bronzeThreshold);
        }
        if(silverMedal!=null)
        {
            silverMedal.SetActive(score >= GameManager.Instance.silverThreshold);
        }
        if(goldMedal!=null)
        {
            goldMedal.SetActive(score > GameManager.Instance.goldThreshold);
        }
    }

    /// <summary>
    /// 更新主菜单UI
    /// </summary>
    /// <param name="coins">主菜单金币</param>
    /// <param name="highScore">主菜单最高得分</param>
    public void UpdateMainMenuUI(int coins,int highScore)
    {
        if(mainMenuCoinsText!=null)
        {
            mainMenuCoinsText.text = $"Coins:{coins}";
        }
        if(mainMenuHighScoreText!=null)
        {
            mainMenuHighScoreText.text = $"High Score:{highScore}";
        }
    }

    /// <summary>
    /// 显示/隐藏游戏结束面板
    /// </summary>
    /// <param name="active">判断是否隐藏</param>
    /// <param name="score">得分</param>
    /// <param name="coinsEarned">获得的金币</param>
    public void SetGameOverPanelActive(bool active,int score,int coinsEarned)
    {
        if(gameOverPanel!=null)
        {
            gameOverPanel.SetActive(active);
            if(active)
            {
                //假设游戏结束面板上有一个Text组件
                Text gameOverText = gameOverPanel.GetComponentInChildren<Text>();
                if(gameOverText!=null)
                {
                    gameOverText.text = $"Game Over\nScore:{score}\nCoins Earned:{coinsEarned}";
                }
            }
        }
    }

    /// <summary>
    /// 显示/隐藏复活面板
    /// </summary>
    /// <param name="active">是否激活</param>
    /// <param name="coinsEarned">本局游戏获得金币数量</param>
    public void SetRevivePanelActive(bool active,int coinsEarned)
    {
        if(revivePanel!=null)
        {
            revivePanel.SetActive(active);
            if(active)
            {
                Text reviveText = revivePanel.GetComponentInChildren<Text>();
                if(reviveText!=null)
                {
                    reviveText.text = $"Earned{coinsEarned}coins!\nRevive?";
                }
            }
        }
    }

    /// <summary>
    /// 显示/隐藏暂停面板
    /// </summary>
    /// <param name="active">是否激活面板</param>
    public void SetPausePanelActive(bool active)
    {
        if(pausePanel!=null)
        {
            pausePanel.SetActive(active); 
        }
    }
}
