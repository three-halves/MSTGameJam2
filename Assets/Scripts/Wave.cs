using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Wave", menuName = "Scrimblo/Wave")]
public class Wave : ScriptableObject
{
    [SerializeField] public WaveNode[] nodes;
    [SerializeField] private float difficulty = 1f;
}

[System.Serializable]
public class WaveNode
{
    [SerializeField] public GameObject[] bulletSpawnerPrefabs;
    [SerializeField] public Vector2 pos;

    // position change per item in list
    [SerializeField] public Vector2 posDelta;

    [SerializeField] public float delay = 3f;
    [SerializeField] public float addAngle = 0f;

    [SerializeField] public bool randomPosOverride = false;
    [SerializeField] public bool randomAngleOverride = false;
    [SerializeField] public bool centerAtPlayer = false;
    // in seconds
    // [SerializeField] float delay;
}