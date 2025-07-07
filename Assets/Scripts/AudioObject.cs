using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private bool destroyOnPlay = true;
    [SerializeField] Vector2 pitchRange;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
    }

    public void Play(float pitch)
    {
        if (pitch <= 0) audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        else audioSource.pitch = pitch;
        audioSource.Play();
    }

    void Update()
    {
        if (!audioSource.isPlaying && destroyOnPlay) Destroy(gameObject);
    }
}
