using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RenderShapes : MonoBehaviour
{

    public GameObject canvas;
    string choice;
    int shapesRemaining;
    public TMP_Text timer, shapesToFind, finishText;
    public float secondsLeft;
    public GameObject uiControl, gameCanvas;
    public DDAAlgorithm dDA;
    public bool finished;
    public bool control= false;

    public void DDAStartAdapter(float time) {

        secondsLeft = time;

    }


    public void StartNew() {
        SceneManager.LoadScene("ShapeGame");
     
    }
    void FinishLevel() {

        finishText.color = Color.green;
        finishText.text = "Well done";
        uiControl.SetActive(true);
        gameCanvas.SetActive(false);
      

    }
    void GameOver() {
        
        uiControl.SetActive(true);
        gameCanvas.SetActive(false);
        
    }

    void DDAFeedbackLevelEnd(bool finished) {
        Debug.Log("Finished Level with " + secondsLeft + " to spare.");
        dDA.DDAReceiveData("shapes", secondsLeft, finished);
    }


   
    void FixedUpdate()
    {
        secondsLeft -= Time.fixedDeltaTime;
        timer.text = "Time Left: " + secondsLeft + " Seconds";
     
        if (secondsLeft <= 0 && control == false)
        {
            control = true;
            DDAFeedbackLevelEnd(false);
            GameOver();
        }
    }


    public void GoMenu()
    {
        SceneManager.LoadScene("GameSelector");

    }



    void Update()
    {

        if (dDA.choiceShape == "Circle")
        {
            shapesRemaining = dDA.circleCount;
        }
        else if (dDA.choiceShape == "Rectangle")
        {
            shapesRemaining = dDA.rectCount;
        } else if (dDA.choiceShape == "Square")
        {
            shapesRemaining = dDA.sqrCount;
        }
        shapesToFind.text = dDA.choiceShape + "s to Find: " + shapesRemaining;
        if (shapesRemaining == 0 && control == false) {

            control = true;
            DDAFeedbackLevelEnd(true);
            FinishLevel();
        
        } 
    }
}
