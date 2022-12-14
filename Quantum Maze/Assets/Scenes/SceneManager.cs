using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void Load_Menu()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Menu");

    public void Load_QuantumStepByStep()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Quantum (step by step)");

    public void Load_QuantumAuto()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Quantum (auto)");

    public void Load_ClassicalBFS()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Classical BFS");

    public void Load_ClassicalDFS()
        => UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Classical DFS");
}
