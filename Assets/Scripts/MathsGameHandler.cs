using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Globalization;
using UnityEngine.SceneManagement;



public class MathsGameHandler : MonoBehaviour
{
    float secondsLeft;
    public TMP_Text timerText;
    double answer;
    public GameObject answerBox;
    public TMP_Text problemText;
    string problem;
    double correctAnswer;
    string correctAnswerString;
    float correctAnswerFloat;
    private TextMeshProUGUI m_TextComponent;
    public DDAAlgorithm dDA;
    public GameObject wrongPrefab;
    public GameObject canvas;
    public GameObject Gameovercanvas;
    public TMP_Text gameOverText;

    bool flag = false;

    
    void FixedUpdate() {
        
        secondsLeft -= Time.fixedDeltaTime;
        timerText.text = "Time Left: " + secondsLeft + " Seconds";
    
        if (secondsLeft <= 0 && flag==false){
            GameOver();
            flag = true;
            GameOverCanvas(false);
        }
    }

    public void DDAStartAdapter(float time, string problem, string correctString, double correctDouble) {

        secondsLeft = time;
        correctAnswer = correctDouble;
        correctAnswerString = correctString;
        problemText.text = problem;
    }

    void GameOverCanvas(bool finished)
    {
        Gameovercanvas.SetActive(true);
        canvas.SetActive(false);
        if (finished == true)
        {
            gameOverText.text = "Well Done";
            gameOverText.color = Color.green;
        }

    }


    void ScoreCalculation() {
        bool result = false;

        if (answer == correctAnswer)
        {
            FinishGame();
            GameOverCanvas(true);
        }
        else if (answer == Math.Round(correctAnswer, 2))
        {
            FinishGame();
            GameOverCanvas(true);
        }
        else if (answer == Math.Floor(correctAnswer))
        {
            FinishGame();
            GameOverCanvas(true);
        }
        else if (answer == Math.Round(correctAnswer, 1))
        {
            FinishGame();
            GameOverCanvas(true);
        }
        else
        {
            Debug.Log("Wrong answer");
            GameObject wrong = Instantiate(wrongPrefab, new Vector3(80.09127807617188f, -180.13502502441407f, 500.42999267578127f), Quaternion.identity);
            wrong.transform.parent = canvas.transform;
            dDA.IncrementDecrementIntelligenceGlobal(-0.1f);
            dDA.IncrementDecrementIntelligence("maths",-0.1f);
            Destroy(wrong, 2.0f);

        }

    }

    void FinishGame()
    {
        DDAFeedback(true);
        Debug.Log("Finished");
    }
    void GameOver()
    {
        DDAFeedback(false);
        Debug.Log("Not Finished");
    }

    void DDAFeedback(bool finished)
    {

        dDA.DDAReceiveData("maths", secondsLeft, finished);

    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("MathsGame");

    }

    public void GoMenu()
    {
        SceneManager.LoadScene("GameSelector");

    }

    public void ReceiveAnswer() {
        string answerString = answerBox.GetComponent<TMP_InputField>().text;
        answer = double.Parse(answerString, CultureInfo.InvariantCulture);
        ScoreCalculation();
    }
    
}
