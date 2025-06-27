using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip buttonSound;

    AudioSource audioSource;

    private void Start()
    {
        if(GameManager.Instance != null)
        {
            audioSource = GameManager.Instance.GetComponent<AudioSource>();
        }

        if(audioSource==null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
        }

        //���Ų˵���������
            if (menuMusic != null)
            {
                audioSource.clip = menuMusic;
                audioSource.Play();
            }

            //���ÿ�ʼ��ť�¼�
            startButton.onClick.AddListener
            (() =>
              {
                  //���Ű�ť��Ч
                if (buttonSound != null)
                {
                    audioSource.PlayOneShot(buttonSound);
                }
                GameManager.Instance.ShowMainMenu();
                
               }
            );

        

    }
}
