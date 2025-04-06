using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

[Serializable]
public class SceneNav : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitApp()
    {
        Application.Quit();
        //UnityEditor.EditorApplication.ExitPlaymode();
    }
}