using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void LoadMenu()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Menu");

    public void LoadMain()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Main");

    public void LoadSmoothMain()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/MainSmooth");
}
