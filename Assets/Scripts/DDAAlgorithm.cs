using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;
using System;
using TMPro;

public class DDAAlgorithm : MonoBehaviour
{

    float timeStart;

    int cluesAvailable;
    GameObject newShape;
    float medianIntelligence;
    public int circleCount, rectCount, sqrCount;
    public string choiceShape;

    string word;
    dynamic root;
    List<string> syns;
    string resultAdjective;
    int wordLength;
    public string onlyLetters;
    List<string> synonyms;
    List<string> allwords;
    public TMP_Text problemText;

    public GameObject[] myShapePrefab;
    public List<GameObject> shapeDistraction;
    public GameObject ShapeCanvas, shapeParent;
    int choiceShapeNo;
    public RenderShapes controller;
    public WordMIRRIAM wordController;
    public MathsGameHandler mathsController;
    bool ctrl1 = false;
    bool ctrl2 = false;
    bool ctrlWrongMoves = false;
    public List<string> modelList;
    float correctAnswer;
    float timer = 0;
    int maxIndex, minIndex;
    


    void FixedUpdate()
    {
        
        if (DDADataPersistance.Instance.intelligenceLevelGlobal < 0)
        {
            DDADataPersistance.Instance.intelligenceLevelGlobal = 0;
        }
        else if (DDADataPersistance.Instance.shapesIntelligence < 0)
        {
            DDADataPersistance.Instance.shapesIntelligence = 0;
        }
        else if (DDADataPersistance.Instance.wordIntelligence < 0)
        {
            DDADataPersistance.Instance.wordIntelligence = 0;
        }
        else if (DDADataPersistance.Instance.mathsIntelligence < 0)
        {
            DDADataPersistance.Instance.mathsIntelligence = 0;
        }
        timer += Time.fixedDeltaTime;
        if (SceneManager.GetActiveScene().name == "ShapeGame" && timer >=7 && CalculateMedianInteligence("shapes")>=50)
        {
            timer = 0;
            newShape = Instantiate(myShapePrefab[choiceShapeNo], new Vector3(0, 0, -1), Quaternion.identity);
            newShape.transform.parent = shapeParent.transform;
            newShape.transform.localScale = new Vector3(0.5f, 0.5f, 1);
            newShape.transform.localPosition = new Vector3(UnityEngine.Random.Range(-500, 500), UnityEngine.Random.Range(-350, 350), -2);
            Color newColour = new Color();
            newColour = UnityEngine.Random.ColorHSV();
            newShape.GetComponent<Image>().color = newColour;
            if (newShape.transform.name == "BaseCircle(Clone)")
            {
                newShape.transform.tag = "Circle";

                circleCount++;
            }
            else if (newShape.transform.name == "BaseSquare(Clone)")
            {
                newShape.transform.tag = "Square";
                sqrCount++;
            }
            else if (newShape.transform.name == "BaseRectangle(Clone)")
            {
                newShape.transform.tag = "Rectangle";
                rectCount++;
            }


        }
           
    }

    void Awake()
    {
        

        if (SceneManager.GetActiveScene().name == "WordsGame")
        {
            CalculateWordLength();
        }

        

    }
    void Start()
    {
        


        if (SceneManager.GetActiveScene().name == "ShapeGame")
        {

            DDASendData("shapes", 90f, "");
            controller.DDAStartAdapter(CalculateTimeSend("shapes"));
        }
        if (SceneManager.GetActiveScene().name == "WordsGame")
        {
            CalculateWordLength();
            wordController.DDAStartAdapter(CalculateTimeSend("words"));
            



        }
        if (SceneManager.GetActiveScene().name == "MathsGame")
        {
           
            DDASendData("maths", 90f, "");
        

        }

    }
    public void DDAReceiveData(string game, float timeLeft,  bool finished)
    {
        CalculateMedianInteligence(game);
        DDADataPersistance.Instance.lastGamesCounter++;
        if (finished == true)
        {
            DDADataPersistance.Instance.lastFiveGames.Add(true);
        }
        else
        {
            DDADataPersistance.Instance.lastFiveGames.Add(false);

        }
        if (DDADataPersistance.Instance.lastGamesCounter > 5) {
            DDADataPersistance.Instance.lastGamesCounter = 0;
            int gamesLost = 0;
            
            foreach (var item in DDADataPersistance.Instance.lastFiveGames)
            {
                
                if (item == false) {
                    gamesLost++;
                } 
            }
            DDADataPersistance.Instance.lastFiveGames.Clear();
            if (gamesLost >= 3)
            {

                DDADataPersistance.Instance.intelligenceLevelGlobal--;
             
            }
        }

        if (timeLeft <= 0 || finished == false)
        {
            DDADataPersistance.Instance.intelligenceLevelGlobal -= 0.5f;
            IncrementDecrementIntelligence(game, -1);

        }
        else if (timeLeft >= 0 && finished == true)
        {
            
            double intelligenceAdjust;
            DDADataPersistance.Instance.intelligenceLevelGlobal += 0.5f;
            
            intelligenceAdjust = 1 + (timeLeft * 0.025);
            
            IncrementDecrementIntelligence(game, (float)intelligenceAdjust);
        }


    }

   

    float CalculateTimeSend(string game)
    {

        timeStart = (float) (120 - (CalculateMedianInteligence(game) * 0.45));

        
        if (CalculateMedianInteligence(game) >= 50)
        {
            timeStart -= (float)((0.04 * (CalculateMedianInteligence(game) - 50)));
        }
        if (SceneManager.GetActiveScene().name == "MathsGame")
        {
            timeStart += 100f;
        }
            Debug.Log("Time Allocated: " + timeStart);
        return timeStart;
    }

    public void DDASendData(string game, float timeStart, string problem)
    {

       
        if (game == "shapes")
        {
          
            int x;
            choiceShape = ShapePicker();
            for (x = 0; x < (5  + (((1 * Math.Truncate(CalculateMedianInteligence("shapes")))) / 2)); x++)
            {
                newShape = Instantiate(myShapePrefab[choiceShapeNo], new Vector3(0, 0, -1), Quaternion.identity);
                newShape.transform.parent = shapeParent.transform;
                newShape.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                newShape.transform.localPosition = new Vector3(UnityEngine.Random.Range(-500, 500), UnityEngine.Random.Range(-350, 350), -2);
                Color newColour = new Color();
                newColour = UnityEngine.Random.ColorHSV();
                newShape.GetComponent<Image>().color = newColour;
                if (newShape.transform.name == "BaseCircle(Clone)")
                {
                    newShape.transform.tag = "Circle";

                    circleCount++;
                }
                else if (newShape.transform.name == "BaseSquare(Clone)")
                {
                    newShape.transform.tag = "Square";
                    sqrCount++;
                }
                else if (newShape.transform.name == "BaseRectangle(Clone)")
                {
                    newShape.transform.tag = "Rectangle";
                    rectCount++;
                }
            }

         

            for (x = 0; x <= ((15-1) + ((2 * Math.Truncate(CalculateMedianInteligence("shapes"))) - (5 + (((1 * Math.Round(CalculateMedianInteligence("shapes")))) / 2)))); x++)
            {




                newShape = Instantiate(shapeDistraction[UnityEngine.Random.Range(0, shapeDistraction.Count)], new Vector3(0, 0, -1), Quaternion.identity);
                newShape.transform.parent = shapeParent.transform;
                newShape.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                newShape.transform.localPosition = new Vector3(UnityEngine.Random.Range(-500, 500), UnityEngine.Random.Range(-350, 350), -2);
                Color newColour = new Color();
                newColour = UnityEngine.Random.ColorHSV();
                newShape.GetComponent<Image>().color = newColour;
                if (newShape.transform.name == "BaseCircle(Clone)")
                {
                    newShape.transform.tag = "Circle";

                    circleCount++;
                }
                else if (newShape.transform.name == "BaseSquare(Clone)")
                {
                    newShape.transform.tag = "Square";
                    sqrCount++;
                }
                else if (newShape.transform.name == "BaseRectangle(Clone)")
                {
                    newShape.transform.tag = "Rectangle";
                    rectCount++;
                }
            }
            Debug.Log("Circles: " + circleCount);
            Debug.Log("Squares: " + sqrCount);
            Debug.Log("Rectangles: " + rectCount);
            Debug.Log("Total Shape Number: " + (circleCount + sqrCount + rectCount));


        }
        else if (game == "words")

        {

            float medianIntelligence = CalculateMedianInteligence("words");
            if (medianIntelligence < 50 && medianIntelligence >= 40)
            {
                cluesAvailable = 1;
             

            }
            else if (medianIntelligence <= 40)
            {
                cluesAvailable = 2;
              

            }
            else { cluesAvailable = 0; Debug.Log("Clues Available: " + cluesAvailable); }


            if (wordController.mode == "synonym")
            {

                GetAdjectiveSynonyms();

            }
            else if (wordController.mode == "wordguess")
            {

                GetDefinitions();

            }
            Debug.Log("Clues Available: " + cluesAvailable);
        }

        else if (game == "maths")

        {
            while (true)
            {
                try
                {
                    string x = GenerateMathsProblem(CalculateMedianInteligence("maths"));
                    object correctAnswer = new System.Data.DataTable().Compute(x, "");
                    Debug.Log(x);
                    Debug.Log(correctAnswer.ToString());
                    mathsController.DDAStartAdapter(CalculateTimeSend("maths"), x, correctAnswer.ToString(), double.Parse(correctAnswer.ToString()));
                }

                catch (Exception e)
                {
                    continue;
                }


                break;
            }

        }

    }

    

    string GenerateMathsProblem(float averageIntelligence)
    {
        string file_name = "";
        if (averageIntelligence >= 0 && averageIntelligence <= 35)
        {
            file_name = "Assets\\Scripts\\MathsModelEasy.json";
            Debug.Log("Used the Easy Mathematical Problem Models");

        }
        else if (averageIntelligence > 35 && averageIntelligence <= 60)
        {
            file_name = "Assets\\Scripts\\MathsModelMedium.json";
            Debug.Log("Used the Medium Mathematical Problem Models");

        }
        else if (averageIntelligence > 60)
        {
            file_name = "Assets\\Scripts\\MathsModelHard.json";
            Debug.Log("Used the Hard Mathematical Problem Models");

        }


        using (var streamReader = new StreamReader(file_name, Encoding.UTF8))
        {
            string resultFile = streamReader.ReadToEnd();
            string randomModel;
            modelList = JArray.Parse(resultFile).ToObject<List<string>>();

            randomModel = modelList[UnityEngine.Random.Range(0, modelList.Count())];
            Debug.Log(randomModel);

            string filterOne = randomModel.Replace("n1", (string)(UnityEngine.Random.Range(0, (int)Math.Floor(averageIntelligence))).ToString());
            string filterTwo = filterOne.Replace("n2", (string)(UnityEngine.Random.Range(0, (int)Math.Floor(averageIntelligence))).ToString());
            string filterThree = filterTwo.Replace("n3", (string)(UnityEngine.Random.Range(0, (int)Math.Floor(averageIntelligence))).ToString());
            string filterFour = filterThree.Replace("n4", (string)(UnityEngine.Random.Range(0, (int)Math.Floor(averageIntelligence))).ToString());
            Debug.Log("Can replace with numbers between 0 and " + Math.Floor(averageIntelligence));
            return filterFour;
        }
    }



    public string DDAClueAnalyseGame(string game, float timeStart, float timeLeftNow, int wrongMoves, int totalMoves)
    {


        
        if (wrongMoves == 12 && ctrlWrongMoves == false)
        {
            char[] letters = onlyLetters.ToArray();
            int letterPos = UnityEngine.Random.Range(0, onlyLetters.Length);
            ctrlWrongMoves = true;
            return "Letter: " + letters[letterPos] + ", at Pos " + (letterPos+1);
        }
        if (((timeStart - timeLeftNow)>=(timeStart/2)) && ctrl1 == false)
        {
            ctrl1 = true;
            char[] letters = onlyLetters.ToArray();
            int letterPos = UnityEngine.Random.Range(0, onlyLetters.Length);
            ctrlWrongMoves = true;
            return "Letter: " + letters[letterPos] + ", at Pos " + (letterPos+1);
        }
        if (((timeStart - timeLeftNow) >= (timeStart * (0.25))) && ctrl2 == false && (cluesAvailable==2))
        {
            ctrl2 = true;

            return "Total Letters: " + (onlyLetters.Length); 
        }

        return "";


    }
   
 

    public void IncrementDecrementIntelligence(string game, float amount)
    {
        if (game == "shapes")
        {
            DDADataPersistance.Instance.shapesIntelligence += amount;
        }
        else if (game == "words")
        {
            DDADataPersistance.Instance.wordIntelligence += amount;
        }
        else if (game == "maths")
        {
            DDADataPersistance.Instance.mathsIntelligence += amount;
        }

    }

    public void IncrementDecrementIntelligenceGlobal(float amount)
    {
        DDADataPersistance.Instance.intelligenceLevelGlobal += amount;
    }

    float CalculateMedianInteligence(string game)
    {

        if (game == "shapes")
        {
            return (float) ((DDADataPersistance.Instance.intelligenceLevelGlobal + DDADataPersistance.Instance.shapesIntelligence) / 2);
        }
        else if (game == "words")
        {
            return (float) ((DDADataPersistance.Instance.intelligenceLevelGlobal + DDADataPersistance.Instance.wordIntelligence) / 2);
        }
        else if (game == "maths")
        {
            return (float) ((DDADataPersistance.Instance.intelligenceLevelGlobal + DDADataPersistance.Instance.mathsIntelligence) / 2);
        }
        return 0f;

        
        
    }

    public string ShapePicker()
    {
        choiceShapeNo = UnityEngine.Random.Range(0, myShapePrefab.Length);
       
        if (choiceShapeNo == 0)
        {
            shapeDistraction.Add(myShapePrefab[1]);
            shapeDistraction.Add(myShapePrefab[2]);
            return "Circle";
            
        }
        else if (choiceShapeNo == 1)
        {
            shapeDistraction.Add(myShapePrefab[0]);
            shapeDistraction.Add(myShapePrefab[2]);
            return "Rectangle";
            

        }
        else if (choiceShapeNo == 2)
        {
            shapeDistraction.Add(myShapePrefab[0]);
            shapeDistraction.Add(myShapePrefab[1]);
            return "Square";
            

        }
        return "";
    }


    void GetAdjectiveSynonyms()
    {


        while (true)
        {
            try
            {
                string file_name = "Assets\\Scripts\\allwords.json";

                using (var streamReader = new StreamReader(file_name, Encoding.UTF8))
                {
                    resultAdjective = streamReader.ReadToEnd();
                    List<string> words = JArray.Parse(resultAdjective).ToObject<List<string>>();
                   
                        onlyLetters = words[UnityEngine.Random.Range(minIndex, maxIndex)];
                        Debug.Log(onlyLetters);
               
                }




                var url = "https://dictionaryapi.com/api/v3/references/thesaurus/json/" + onlyLetters + "?key=f5df10f1-6fc7-4e58-ad01-4530901353c1";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var root = JArray.Parse(result).ToObject<List<JObject>>();
                    var syns = root[0]["meta"]["syns"][0].ToString();
                    Debug.Log(syns);

                    synonyms = JArray.Parse(syns).ToObject<List<string>>();

                    Debug.Log(synonyms);
                    problemText.text = "Synonyms: " + String.Join(", ", synonyms.Take(20));
                    break;
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
                continue;
            }
                
        }



        Debug.Log("Word Length: " + onlyLetters.Length);
        
    }

    void GetDefinitions()
    {
        
        string file_name = "Assets\\Scripts\\allwords.json";

        using (var streamReader = new StreamReader(file_name, Encoding.UTF8))
        {
            resultAdjective = streamReader.ReadToEnd();
            allwords = JArray.Parse(resultAdjective).ToObject<List<string>>();
           
                onlyLetters = allwords[UnityEngine.Random.Range(minIndex, maxIndex)];
                Debug.Log(onlyLetters);

  
        }

        Boolean success = false;

        while (!success)
        {
            try
            {
                var url = "https://dictionaryapi.com/api/v3/references/thesaurus/json/" + onlyLetters + "?key=f5df10f1-6fc7-4e58-ad01-4530901353c1";
                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {


                    var result = streamReader.ReadToEnd();
                    var root = JArray.Parse(result).ToObject<List<dynamic>>();
                    var def = root[0]["shortdef"][0].ToString();
                    Debug.Log(syns);


                    Debug.Log(def);
                    problemText.text = "Guess Word Definition: " + def;
                    success = true;
           
                }
            }
            catch (Exception e)
            {
               
                    onlyLetters = allwords[UnityEngine.Random.Range(minIndex, maxIndex)];
                    Debug.Log(onlyLetters);

            }

        }
        Debug.Log("Word Length: " + onlyLetters.Length);
        
    }

    void CalculateWordLength()
    {
        
        if (CalculateMedianInteligence("words") >= 0 && CalculateMedianInteligence("words") < 10)
        {
            wordLength = 3;
            maxIndex = 1101;
            minIndex = 0;
        }
        else if (CalculateMedianInteligence("words") >= 10 && CalculateMedianInteligence("words") < 20)
        {
            wordLength = 4;
            maxIndex = 5096;
            minIndex = 1102;
        }
        else if (CalculateMedianInteligence("words") >= 20 && CalculateMedianInteligence("words") < 30)
        {
            wordLength = 5;
            maxIndex = 13981;
            minIndex = 5097;
        }
        else if (CalculateMedianInteligence("words") >= 30 && CalculateMedianInteligence("words") < 40)
        {
            wordLength = 6;
            maxIndex = 29701;
            minIndex = 13982;
        }
        else if (CalculateMedianInteligence("words") >= 40 && CalculateMedianInteligence("words") < 50)
        {
            wordLength = 7;
            maxIndex = 53651;
            minIndex = 29702;
        }
        else if (CalculateMedianInteligence("words") >= 50)
        {
            wordLength = 20;
            maxIndex = 178187;
            minIndex = 53652;
        }
    }
}
