using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class RigidbodySleep : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip crashClip;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    public float soundThreshold = 2.0f;


    private int sleepCountdown =4;
    private Rigidbody rigid;
    private AudioSource audioSource;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
        // Optimize AudioSource for one-shots
        audioSource.playOnAwake = false;
    }

    void FixedUpdate()
    {
        if (sleepCountdown > 0)
        {
            rigid.Sleep();
            sleepCountdown--;
        }
    }
    // Triggered whenever something hits this block
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Don't play sounds during the initial sleep countdown
        if (sleepCountdown > 0) return;

        // 2. Check how hard the impact was
        float impactForce = collision.relativeVelocity.magnitude;

        // 3. Play the sound if the impact is above the threshold
        if (impactForce > soundThreshold)
        {
            // PlayOneShot is best so sounds can overlap without cutting off
            audioSource.PlayOneShot(crashClip, volume);
        }
    }
}
