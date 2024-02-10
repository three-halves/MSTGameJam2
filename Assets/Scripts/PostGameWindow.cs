using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;


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

    public void StartPostGame()
    {
        Time.timeScale = 0f;
        float timeSurvived = GameObject.Find("PlayerContainer").GetComponent<Player>().timeSurvived;
        int score =  GameObject.Find("PlayerContainer").GetComponent<Player>().score;
        postGameWindow.SetActive(true);
        EventSystem.current.SetSelectedGameObject(postGameWindow.transform.GetChild(1).gameObject);
        scoreText.text = string.Format("score: {0} \n time: {1}m {2}s \n scoretime efficiency: {3} \n", score, Mathf.Floor(timeSurvived / 60), Mathf.Floor(timeSurvived % 60), score/timeSurvived);
    }
}
