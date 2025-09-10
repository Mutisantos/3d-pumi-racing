using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

/**
 * Clase que mantiene el GameLoop de una carrera en curso, controlando y pasando información
 * sobre el recorrido, los corredores y sus puestos de salida y determinando cuando una 
 * carrera ha iniciado, hacer el conteo regresivo y finalizar la carrera.
 */
public class RaceManager : MonoBehaviour
{
    //Numero de vueltas de la carrera
    public int LapAmount;
    public float StartDelay = 5f;
    public float CountDownSpeed = 0.75f;
    public float EndDelay = 3f;
    //Lista de waypoints que definen el circuito
    public List<Transform> TrackWaypoints;
    //Definición del atajo del circuito
    public Shortcut Shortcut;
    //Definición de los jugadores activos en la carrera
    public List<CarManager> Racers;
    public AudioClip CountDownAudio;
    public AudioClip TrackBGM;
    public AudioClip VictoryBGM;
    public Material StartLightsShader;
    //Referencia para el contador regresivo en el HUD
    public TextMeshProUGUI TimeText;
    //Puestos de salida para inicializar a los corredores
    public List<Transform> SpawnPoints;
    public GameObject GhostReference;
    //Referencia al gestor de la UI. 
    public UIHelper CanvasHelper;
    public bool RaceFinished = true;
    private float CountdownTime;
    private WaitForSeconds StartWait;
    private WaitForSeconds EndWait;

    void Awake()
    {
        CountdownTime = StartDelay;
        GameObject carInstance = Instantiate(CommonDataSingleton.instance.ChosenRacer, SpawnPoints[0].position, SpawnPoints[0].rotation);

        Racers = new List<CarManager>
        {
            carInstance.GetComponent<CarManager>()
        };
        foreach (var racer in Racers)
        {
            racer.CanvasHelper = this.CanvasHelper;
        }
        GhostReference.transform.SetPositionAndRotation(SpawnPoints[0].position, SpawnPoints[0].rotation);
    }

    void Start()
    {
        StartWait = new WaitForSeconds(0.5f);
        CommonDataSingleton.instance.LoadRecordData();
        CommonDataSingleton.instance.RegisterRaceManager(this);
        StartCoroutine(GameLoop());
        ChangeLightColor((int)CountdownTime);

    }

    void Update()
    {
        if (CountdownTime > 0)
        {
            ChangeLightColor((int)CountdownTime);
            CountdownTime = CanvasHelper.PerformCountdown(CountdownTime, CountDownAudio, CountDownSpeed);
        }
        else
        {
            ChangeLightColor(0);
            CheckIfRacersFinished();
        }
    }

    public void RestartRace()
    {
        StopAllCoroutines();
        foreach (var racer in Racers)
        {
            racer.ResetCarValues();
        }
        var playerRacer = Racers[0];
        ReplayController replayController = playerRacer.GetComponent<ReplayController>();
        replayController.FollowingType = ReplayType.PLAYER;
        replayController.enabled = true;
        playerRacer.GetComponent<CarPlayerSampler>().enabled = false;
        RaceFinished = false;
        StartCoroutine(GameLoop());
        CountdownTime = StartDelay;
        ChangeLightColor((int)CountdownTime);

    }


    private void CheckIfRacersFinished()
    {
        if (!RaceFinished)
        {
            bool raceFinished = true;
            foreach (var racer in Racers)
            {
                if (racer.CurrentLap <= LapAmount)
                {
                    raceFinished = false;
                }
            }
            this.RaceFinished = raceFinished;
        }
    }

    // Cambiar el color del semaforo en el conteo regresivo.
    private void ChangeLightColor(int seconds)
    {
        switch (seconds)
        {
            case 3: StartLightsShader.color = Color.blue; break;
            case 2: StartLightsShader.color = Color.red; break;
            case 1: StartLightsShader.color = Color.yellow; break;
            case 0: StartLightsShader.color = Color.green; break;
            default: break;
        }
    }

    public int CountTrackWaypoints()
    {
        return TrackWaypoints.Count;
    }
    

    //Controlar el flujo de juego en esta subrutina
    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RaceStarting());
        yield return StartCoroutine(RacePlaying());
        yield return StartCoroutine(RaceEnding());
        if (RaceFinished)
        {
            DisableRacers();
            CanvasHelper.ShowFinalResults();
            CanvasHelper.EnableRecordOverwrite();
            CanvasHelper.EnableResultUI();
            SoundManager.instance.PlayBGM(VictoryBGM);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }



    private IEnumerator RaceStarting()
    {
        GhostReference.SetActive(true);
        SoundManager.instance.StopBackgroundMusic();
        CanvasHelper.EnableCarHud();
        DisableRacers();
        for (int i = 0; i < Racers.Count; i++)
        {
            Racers[i].transform.position = SpawnPoints[i].position;
            Racers[i].transform.rotation = SpawnPoints[i].rotation;
        }

        while (CountdownTime > 0)
        {
            yield return null;
        }
        SoundManager.instance.PlayOnce(CountDownAudio, 1.5f);
        StartFadeout();
    }


    private IEnumerator RacePlaying()
    {
        RaceFinished = false;
        EnableRacers();
        SoundManager.instance.StartBackgroundMusic(TrackBGM);
        //En paralelo, el juego se mantendrá activo hasta que todos los corredores acaben.
        while (!RaceFinished)
        {
            yield return null;
        }
    }


    private IEnumerator RaceEnding()
    {
        DisableRacers();
        yield return EndWait;
    }


    private void StartFadeout()
    {
        CanvasHelper.FinishCountdown(StartWait);
    }

    private void EnableRacers()
    {
        foreach (var racer in Racers)
        {
            if (!racer.CarEnabled)
            {
                racer.OnCarEnabled(TrackWaypoints, Shortcut);
            }
        }
    }


    private void DisableRacers()
    {
        foreach (var racer in Racers)
        {
            racer.OnCarDisabled();
        }
    }
}
