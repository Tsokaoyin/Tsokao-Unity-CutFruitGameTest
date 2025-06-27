using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public float sliceForce = 15.0f;
    public float minSliceVelocity = 0.01f;

    //�и���Ч 
    public AudioClip sliceSound;
    private AudioSource audioSource;

    private Camera mainCamera;
    private Collider sliceCollider;
    private TrailRenderer sliceTrail;

    public Vector3 direction { get; private set; }
    public bool slicing { get; private set; }

    private void Awake()
    {
        mainCamera = Camera.main;
        sliceCollider = GetComponent<Collider>();
        sliceTrail = GetComponentInChildren<TrailRenderer>();

        //��ʼ����Ч���
        audioSource = GetComponent<AudioSource>();
        if(audioSource==null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            StartSlice();
        }else if(Input.GetMouseButtonUp(0))
        {
            StopSlice();
        }else if(slicing)
        {
            ContinueSlice();
        }
    }

    private void OnEnable()
    {
        StartSlice();
    }

    private void OnDisable()
    {
        StopSlice();
    }

    private void StopSlice()
    {
        slicing = false;
        sliceCollider.enabled = false;
        sliceTrail.enabled = false;
    }

    private void StartSlice()
    {
        Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        transform.position = position;

        slicing = true;
        sliceCollider.enabled = true;
        sliceTrail.enabled = true;
        sliceTrail.Clear();

        //�����и���Ч
        if (sliceSound!=null&&audioSource!=null&&sliceCollider.CompareTag("Fruit"))
        {
            audioSource.PlayOneShot(sliceSound);
        }
    }

    private void ContinueSlice()
    {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPosition.z = 0;
        direction = newPosition - transform.position;

        float velocity = direction.magnitude / Time.deltaTime;
        sliceCollider.enabled = velocity > minSliceVelocity;

        transform.position = newPosition;
    }

    public void ResetPosition()
    {
        if(mainCamera!=null)
        {
            Vector3 position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            transform.position = position;
        }

        //���ù켣
        if(sliceTrail!=null)
        {
            sliceTrail.Clear();
        }
    }
}
