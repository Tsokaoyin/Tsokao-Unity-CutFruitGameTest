using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("��Ϸ�������")]
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

    [Header("���������")]
    [SerializeField]private Text mainMenuCoinsText;
    [SerializeField] private Text mainMenuHighScoreText;

    [Header("����ѡ��")]
    [SerializeField] private Button coinReviveButton;
    [SerializeField] private Button adReviveButton;
    [SerializeField] private Button noReviveButton;

    private void Start()
    {
        //���ø��ť�¼�
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
    /// ������ϷUI
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
    /// ���½�����ʾ
    /// </summary>
    /// <param name="score">����</param>
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
    /// �������˵�UI
    /// </summary>
    /// <param name="coins">���˵����</param>
    /// <param name="highScore">���˵���ߵ÷�</param>
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
    /// ��ʾ/������Ϸ�������
    /// </summary>
    /// <param name="active">�ж��Ƿ�����</param>
    /// <param name="score">�÷�</param>
    /// <param name="coinsEarned">��õĽ��</param>
    public void SetGameOverPanelActive(bool active,int score,int coinsEarned)
    {
        if(gameOverPanel!=null)
        {
            gameOverPanel.SetActive(active);
            if(active)
            {
                //������Ϸ�����������һ��Text���
                Text gameOverText = gameOverPanel.GetComponentInChildren<Text>();
                if(gameOverText!=null)
                {
                    gameOverText.text = $"Game Over\nScore:{score}\nCoins Earned:{coinsEarned}";
                }
            }
        }
    }

    /// <summary>
    /// ��ʾ/���ظ������
    /// </summary>
    /// <param name="active">�Ƿ񼤻�</param>
    /// <param name="coinsEarned">������Ϸ��ý������</param>
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
    /// ��ʾ/������ͣ���
    /// </summary>
    /// <param name="active">�Ƿ񼤻����</param>
    public void SetPausePanelActive(bool active)
    {
        if(pausePanel!=null)
        {
            pausePanel.SetActive(active); 
        }
    }
}
