using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class WordMIRRIAM : MonoBehaviour
{
    
    public TMP_Text problemText;
    public GameObject answerBox;
    public string mode;
    public DDAAlgorithm dDA;
    public int totalMoves, wrongMoves;
    public float secondsPassed;
    float secondsLeft;
    public TMP_Text timer;
    bool control=false;
    float timeStart;
    public TMP_Text modeText;
    public TMP_Text clueText;
    public GameObject gameOverCanvas;
    public GameObject gameCanvas;
    public GameObject wrongPrefab;
    public TMP_Text gameOverText;

    public void DDAStartAdapter(float time)
    {
        secondsLeft = time;
        timeStart = time;
    }

    void Start()
    {
        Mode(DDADataPersistance.Instance.wordMode);

    }

    void FixedUpdate()
    {
     
        secondsLeft -= Time.fixedDeltaTime;
        secondsPassed += Time.fixedDeltaTime;
        timer.text = "Time Left: " + secondsLeft + " Seconds";
        
        string clue = dDA.DDAClueAnalyseGame("words", timeStart, secondsLeft, wrongMoves, totalMoves);
        if (clue != "")
        {
            clueText.text += "\n " + clue;

        }


        if (secondsLeft <= 0 && control == false)
        {
            control = true;
      
            DDAFeedbackLevelEnd(false);
            GameOver(false);
        }


    }

    void GameOver(bool finished)
    {
        gameOverCanvas.SetActive(true);
        gameCanvas.SetActive(false);
        if (finished == true)
        {
            gameOverText.text = "Well Done";
            gameOverText.color = Color.green;
        }

    }

    void DDAFeedbackLevelEnd(bool finished)
    {

        dDA.DDAReceiveData("words", secondsLeft, finished);
    }

    public void EvaluateGame() {
        string text = answerBox.GetComponent<TMP_InputField>().text;
        if (dDA.onlyLetters.Equals(text))
        {
            problemText.text += "\n Correct!";

            totalMoves++;
            DDAFeedbackLevelEnd(true);
            GameOver(true);
        }
        else
        {

            Debug.Log("Wrong answer");
            GameObject wrong = Instantiate(wrongPrefab, new Vector3(-452.1872863769531f, -64.39324188232422f, 500.42999267578127f), Quaternion.identity);
            wrong.transform.parent = gameCanvas.transform;
            Destroy(wrong, 2.0f);
            dDA.IncrementDecrementIntelligenceGlobal(-0.1f);
            dDA.IncrementDecrementIntelligence("words", -0.1f);
            totalMoves++;
            wrongMoves++;
        }

    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("WordsGame");

    }

    public void GoMenu()
    {
        SceneManager.LoadScene("GameSelector");

    }



    public void Mode(string modes)
    {
        if (modes == "synonym")
        {
            mode = "synonym";
            modeText.text = "Playing Synonyms";
            dDA.DDASendData("words", 90f, "");
            
        }
        else
        {
            mode = "wordguess";
            modeText.text = "Playing Word Guess";
            dDA.DDASendData("words", 90f, "");
            
        }
            
    }

}
