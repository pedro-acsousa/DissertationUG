using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public class DDADataPersistance : MonoBehaviour
{
    
    public static DDADataPersistance Instance;
    [JsonProperty]
    public float intelligenceLevelGlobal = 10;
    [JsonProperty]
    public float shapesIntelligence = 10;
    [JsonProperty]
    public float mathsIntelligence = 50;
    [JsonProperty]
    public float wordIntelligence = 50;
    [JsonProperty]
    public int lastGamesCounter = 0;
    public string wordMode = "";
    [JsonProperty]
    public List<bool> lastFiveGames = new List<bool>();

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



}
