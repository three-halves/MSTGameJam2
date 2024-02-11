using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioObject : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] Vector2 pitchRange;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
    }

    void Update()
    {
        if (!audioSource.isPlaying) Destroy(gameObject);
    }
}
