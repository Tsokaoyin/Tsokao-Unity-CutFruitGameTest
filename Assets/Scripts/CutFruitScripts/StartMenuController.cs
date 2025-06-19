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
        
        if (GameManager.Instance !=null)
        {
            audioSource = GameManager.Instance.GetComponent<AudioSource>();

            if (menuMusic != null)
            {
                audioSource.clip = menuMusic;
                audioSource.Play();
            }


            startButton.onClick.AddListener
            (() =>
            {

                if (buttonSound != null)
                {
                    audioSource.PlayOneShot(buttonSound);
                }
                GameManager.Instance.ShowMainMenu();
                
            }
            );

        }

    }
}
