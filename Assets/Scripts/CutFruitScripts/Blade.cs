using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    public float sliceForce = 5.0f;
    public float minSliceVelocity = 0.01f;

    //«–∏Ó“Ù–ß
    public AudioClip sliceSound;
    private AudioSource audioSource;

    private Camera mainCamera;
    private Collider sliceCollider;
    private TrailRenderer sliceTrail;

    public Vector3 direction { get; private set; }
    public bool slicing { get; private set; }

    private void Awake()
    {
        
    }
}
