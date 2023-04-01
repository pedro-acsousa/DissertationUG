using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine.SceneManagement;
using System.Text;
using System.Security.Cryptography;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;


public class DIfficultySaving : MonoBehaviour
{

    public GameObject saveUser;
    public GameObject retrieveUser;
    public GameObject prefab;
    public GameObject canvas;
    public GameObject rewrite;





    public bool GetContentsFile(bool rewrite)
    {
        try
        {
            using (var streamReader = new StreamReader("AssetsDDAData.json"))
            {
                List<dynamic> allList = new List<dynamic>();
                while (streamReader.Peek() >= 0)
                {
                    string allBytes = streamReader.ReadLine();

                    dynamic jSONObj = JObject.Parse(allBytes).ToObject<Dictionary<string, dynamic>>();

                    allList.Add(jSONObj);
                }
                streamReader.Close();
                int index = 0;
                int indexDelete = 0;
                foreach (dynamic x in allList)
                {

                    foreach (KeyValuePair<string, dynamic> kvp in x)
                    {
                        if (kvp.Key == saveUser.GetComponent<TMP_InputField>().text)
                        {
                            if (rewrite == true)
                            {
                                indexDelete = index;
                            }


                            if (rewrite == false)
                            {
                                return true;
                            }

                        }


                    }
                    index++;

                }
                if (rewrite == true)
                {
                    if (rewrite == true)
                    {

                        allList.RemoveAt(indexDelete);
                        dynamic newEntry = new Dictionary<string, dynamic>();
                        newEntry.Add(saveUser.GetComponent<TMP_InputField>().text, DDADataPersistance.Instance);
                        allList.Add(newEntry);

                    }
                    File.Delete("AssetsDDAData.json");

                    foreach (dynamic x in allList)
                    {
                        string jsonFile = JsonConvert.SerializeObject(x, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });


                        using (StreamWriter outputFile = new StreamWriter("AssetsDDAData.json", true))
                        {
                            outputFile.WriteLine(jsonFile);
                            outputFile.Flush();
                            outputFile.Close();
                            this.rewrite.SetActive(true);
                            GameObject status = Instantiate(prefab, new Vector3(-14.259553909301758f, -4.385691165924072f, 1.8574676513671876f), Quaternion.identity);
                            status.transform.parent = canvas.transform;
                            status.GetComponent<TMP_Text>().fontSize = 18;
                            status.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                            status.GetComponent<TMP_Text>().text = "File Rewritten";
                            Destroy(status, 2.5f);
                        }
                    }
                }





                return false;

            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            GameObject status = Instantiate(prefab, new Vector3(-14.259553909301758f, -4.385691165924072f, 1.8574676513671876f), Quaternion.identity);
            status.transform.parent = canvas.transform;
            status.GetComponent<TMP_Text>().fontSize = 18;
            status.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            status.GetComponent<TMP_Text>().text = "Score File Corrupted \nDeleted File";
            Destroy(status, 2.5f);
            File.Delete("AssetsDDAData.json");
            return false;
        }
    }

    public void SaveDifficulty()
    {

        if (GetContentsFile(false) == false)
        {
            if (DDADataPersistance.Instance != null || saveUser.GetComponent<TMP_InputField>().text != "")
            {
                string userName = saveUser.GetComponent<TMP_InputField>().text;
                Dictionary<string, dynamic> jsonData = new Dictionary<string, dynamic>();

                jsonData.Add(userName, DDADataPersistance.Instance);



                string jsonFile = JsonConvert.SerializeObject(jsonData, Formatting.None,
                            new JsonSerializerSettings()
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });
                Debug.Log(jsonFile);


                string filePath = Application.dataPath + "DDAData.json";

                GameObject status = Instantiate(prefab, new Vector3(-14.259553909301758f, -4.385691165924072f, 1.8574676513671876f), Quaternion.identity);
                status.transform.parent = canvas.transform;
                status.GetComponent<TMP_Text>().fontSize = 18;
                status.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                status.GetComponent<TMP_Text>().text = "Success, saved \n Retrieve data with username next time";
                Destroy(status, 2.5f);
                //PREFEB NOT DISPLAYING
                using (StreamWriter outputFile = new StreamWriter(filePath, true))
                {
                    outputFile.WriteLine(jsonFile);
                    outputFile.Flush();
                    outputFile.Close();
                }
            }

        }
        else
        {
            GameObject status = Instantiate(prefab, new Vector3(-14.259553909301758f, -4.385691165924072f, 1.8574676513671876f), Quaternion.identity);
            status.transform.parent = canvas.transform;
            status.GetComponent<TMP_Text>().fontSize = 18;
            status.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            status.GetComponent<TMP_Text>().text = "Not saved \nUsername already exists, Rewrite?";
            rewrite.SetActive(true);

            Destroy(status, 2.5f);
        }
           
            
    }

    public void RecoverPoints()
    {
        using (var streamReader = new StreamReader("AssetsDDAData.json"))
        {
            List<dynamic> allList = new List<dynamic>();
            while (streamReader.Peek() >= 0)
            {
                string allBytes = streamReader.ReadLine();

                dynamic jSONObj = JObject.Parse(allBytes).ToObject<Dictionary<string, dynamic>>();

                allList.Add(jSONObj);
            }
        
            streamReader.Close();

            bool flag = false;
            foreach (dynamic x in allList)
            {

                foreach (KeyValuePair<string, dynamic> kvp in x)
                {
                    if (kvp.Key == retrieveUser.GetComponent<TMP_InputField>().text)
                    {
                        flag = true;
                        DDADataPersistance.Instance.intelligenceLevelGlobal = (float)kvp.Value["intelligenceLevelGlobal"];
                        DDADataPersistance.Instance.shapesIntelligence = (float)kvp.Value["shapesIntelligence"];
                        DDADataPersistance.Instance.mathsIntelligence = (float)kvp.Value["mathsIntelligence"];
                        DDADataPersistance.Instance.wordIntelligence = (float)kvp.Value["wordIntelligence"];
                        DDADataPersistance.Instance.lastGamesCounter = (int)kvp.Value["lastGamesCounter"];
                        DDADataPersistance.Instance.lastFiveGames = (List<bool>)kvp.Value["lastFiveGames"].ToObject<List<bool>>();


                        GameObject status = Instantiate(prefab, new Vector3(-14.259553909301758f, -4.385691165924072f, 1.8574676513671876f), Quaternion.identity);
                        status.transform.parent = canvas.transform;
                        status.GetComponent<TMP_Text>().fontSize = 18;
                        status.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        status.GetComponent<TMP_Text>().text = "Difficulty Score Retrieved";
                        Destroy(status, 2.5f);
                    }
                     

                }

            }
            if (flag == false)
            {
                GameObject status = Instantiate(prefab, new Vector3(-14.259553909301758f, -4.385691165924072f, 1.8574676513671876f), Quaternion.identity);
                status.transform.parent = canvas.transform;
                status.GetComponent<TMP_Text>().fontSize = 18;
                status.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                status.GetComponent<TMP_Text>().text = "No Such Username";
                Destroy(status, 2.5f);
            }
        }

    }

    public void GoBack()
    {
        SceneManager.LoadScene("GameSelector");
    }

    public void ReWrite()
    {
        GetContentsFile(true);
        rewrite.SetActive(false);
    }

    string EncryptFile(string bytes)
    {

        string result="";
        if (!string.IsNullOrEmpty(bytes))
        {
            byte[] plaintextBytes = StringToByteArray(bytes);

            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key = new byte[8] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                }

                result = ByteArrayToString(memoryStream.ToArray());
            }
        }
        

        return result;

    }

    string DecryptFile(string bytes)
    {
        string result="";
        if (!string.IsNullOrEmpty(bytes))
        {
            
            byte[] encryptedBytes = StringToByteArray(bytes);

            SymmetricAlgorithm symmetricAlgorithm = DES.Create();
            symmetricAlgorithm.Key = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, symmetricAlgorithm.CreateDecryptor(), CryptoStreamMode.Read))
                {
                    byte[] decryptedBytes = new byte[encryptedBytes.Length];
                    cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                    result = ByteArrayToString(decryptedBytes);
                }
            }
        }

        return result;

    }

    public static string ByteArrayToString(byte[] ba)
    {
        StringBuilder hex = new StringBuilder(ba.Length * 2);
        foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }



    public static byte[] StringToByteArray(String hex)
    {
        var outputLength = hex.Length / 2;
        var output = new byte[outputLength];
        for (var i = 0; i < outputLength; i++)
            output[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return output;
    }
}
