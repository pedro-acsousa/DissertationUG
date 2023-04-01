using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.IsolatedStorage;
using System;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

public class ArraySort : MonoBehaviour
{
    string file_name = "Assets\\Scripts\\words.json";
    string resultAdjective;
    List<string> words;
    int x = 1;
    void Start()
    {
        

        using (var streamReader = 
            new StreamReader(file_name, Encoding.UTF8))
        {
            resultAdjective = streamReader.ReadToEnd();
            words = JArray.Parse(resultAdjective).ToObject<List<string>>();
            words.Sort((x, y) => x.Length.CompareTo(y.Length));
        }

        using (StreamWriter outputFile = 
            new StreamWriter("NewWords.json", true))
        {
            outputFile.Write("[");
            foreach (var item in words)
            {
                outputFile.Write("\"");
                outputFile.Write(item);
                outputFile.Write("\",");

            }
        

            outputFile.Write("]");
            outputFile.Flush();
            outputFile.Close();

        }


        foreach (var item in words)
        {
            
            if (!(words[x - 1].Length == words[x].Length))
            {
                Debug.Log("Length " + words[x - 1].Length + 
                    ", ends at index " + (x - 1));

            }
            x++;
        }

    }


}
