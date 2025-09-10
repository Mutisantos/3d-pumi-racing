using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
*  Una instancia única que va a servir de enlace entre otras clases así como de proveer y mantener datos y metodos de uso común. 
*/
public class CommonDataSingleton : MonoBehaviour
{


    public static CommonDataSingleton instance;
    public List<GameObject> AvailableRacerPrefabs;
    public List<string> AvailableTrackNames;
    public bool RaceFinished { get { return CurrentRaceManager.RaceFinished; } }
    public List<float> LapMarks;
    public GameObject ChosenRacer;
    public string ChosenTrackName;
    public int ChosenTrackIndex;
    //Muestras hechas por el jugador
    public RaceSample PlayerRecordSample;
    [SerializeField]
    //Muestras cargadas por el record de pista
    private RaceSample ChosenTrackRecordSample;
    [SerializeField]
    private RaceManager CurrentRaceManager;


    void Awake()
    {
        MakeSingleton();
        Time.timeScale = 1;
    }

    private void MakeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void RegisterRaceManager(RaceManager trackManager)
    {
        this.CurrentRaceManager = trackManager;
    }

    public void ReplayRace()
    {
        if (this.CurrentRaceManager != null)
        {
            this.CurrentRaceManager.RestartRace();
        }
    }

    public bool IsNewRecordAchieved()
    {
        return LapMarks.ToArray().Sum() < ChosenTrackRecordSample.TotalTime;
    }


    public void LoadRecordData()
    {
        this.ChosenTrackRecordSample = LoadSamplesFile(this.ChosenTrackName);
        if (this.ChosenTrackRecordSample == null)
        {
            ChosenTrackRecordSample = new()
            {
                LapSamples = new(),
                //Si no hay record, se toma el record como vacío
                TotalTime = int.MaxValue
            };
        }
    }

    public void ChooseRacer(int racerIndex)
    {
        this.ChosenRacer = AvailableRacerPrefabs[racerIndex];
    }


    public RaceSample LoadSamplesFile(string filename)
    {
        try
        {
#if UNITY_EDITOR
            string path = Path.Combine(Application.dataPath, "Resources", $"{filename}.json");
#else
            string path = Path.Combine(Application.persistentDataPath, "Resources", $"{filename}.json");
#endif
            var textAsset = File.ReadAllText(path);
            if (textAsset != null)
            {
                return JsonUtility.FromJson<RaceSample>(textAsset);
            }
            return null;
        }
        catch (FileNotFoundException ex)
        {
            Debug.LogWarning($"File not found for track {ex.FileName}");
            return null;
        }
        catch (DirectoryNotFoundException ex)
        {
            Debug.LogWarning($"Directory Issue: {filename} {ex.TargetSite}");
            var textAsset = Resources.Load<TextAsset>($"{filename}.json");
            if (textAsset != null)
            {
                return JsonUtility.FromJson<RaceSample>(textAsset.text);
            }
            return null;
        }
    }

    public void FinishRace()
    {
        this.CurrentRaceManager.RaceFinished = true;
    }

    public void SaveSamplesFile()
    {
        SaveSamplesFile(this.ChosenTrackName);
    }

    public void SaveSamplesFile(string filename)
    {
        PlayerRecordSample.TrackName = ChosenTrackName;
        #if UNITY_EDITOR
                string path = Path.Combine(Application.dataPath, "Resources", $"{filename}.json");
        #else
                string path = Path.Combine(Application.persistentDataPath, "Resources", $"{filename}.json");
        #endif
        var jsonContent = JsonUtility.ToJson(PlayerRecordSample);
        File.WriteAllText(path, jsonContent);
        Debug.Log($"Saved File {path}");
    }

    public string GenerateTimestampString(float time)
    {
        return string.Format("{0:00}:{1:00}:{2:0000}", (int)(time / 60), time % 60, (time * 1000) % 1000);
    }

}
