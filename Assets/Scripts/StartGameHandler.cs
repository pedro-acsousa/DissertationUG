using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameHandler : MonoBehaviour
{

    public void StartGameSelector()
    {
        SceneManager.LoadScene("GameSelector");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartMaths() {
        SceneManager.LoadScene("MathsGame");
    }

    public void StartShapes()
    {
        SceneManager.LoadScene("ShapeGame");
    }
    public void StartWords(string mode)
    {
        DDADataPersistance.Instance.wordMode = mode;
        SceneManager.LoadScene("WordsGame");
    }
    public void StartWordMode()
    {
    
        SceneManager.LoadScene("WordGameMode");
    }
    public void StartDifficultyManager()
    {

        SceneManager.LoadScene("DifficultyManager");
    }

    public void Back()
    {
        if (SceneManager.GetActiveScene().name == "WordGameMode")
        {
            
            SceneManager.LoadScene("GameSelector");
        }
    }
}
