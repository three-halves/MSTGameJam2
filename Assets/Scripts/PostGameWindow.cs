using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class PostGameWindow : MonoBehaviour
{
    [SerializeField] GameObject postGameWindow;
    [SerializeField] TMP_Text scoreText;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        postGameWindow.SetActive(false);
    }

    public void SetScene(string s)
    {
        SceneManager.LoadScene(s);
    }

    public IEnumerator StartPostGame()
    {
        yield return new WaitForSeconds(1f);
        Time.timeScale = 0f;
        float timeSurvived = GameObject.Find("PlayerContainer").GetComponent<Player>().timeSurvived;
        int score = GameObject.Find("PlayerContainer").GetComponent<Player>().score;
        int waves =  GameObject.Find("WaveManager").GetComponent<WaveManager>().waveCount;
        postGameWindow.SetActive(true);
        StartCoroutine(SelectRetry());
        scoreText.text = string.Format(
            "score: {0}\ntime: {1}m {2}s\nwaves: {3}\n scorepersec: {4}\nscoreperwave: {5}",
            score,
            Mathf.Floor(timeSurvived / 60),
            Mathf.Floor(timeSurvived % 60),
            waves,
            Math.Round(score/timeSurvived, 3),
            Math.Round((float)score/waves, 3)
        );
    }

    private IEnumerator SelectRetry()
    {
        yield return new WaitForSecondsRealtime(1f);
        EventSystem.current.SetSelectedGameObject(postGameWindow.transform.GetChild(1).gameObject);
    }
}
