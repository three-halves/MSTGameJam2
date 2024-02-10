using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class PostGameWindow : MonoBehaviour
{
    [SerializeField] GameObject postGameWindow;
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
        postGameWindow.SetActive(true);
        EventSystem.current.SetSelectedGameObject(postGameWindow.transform.GetChild(0).gameObject);
    }
}
