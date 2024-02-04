using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private string m_GameSceneName = "Game";

    public void PlayPressed()
    {
        SceneManager.LoadScene(m_GameSceneName);
    }
}