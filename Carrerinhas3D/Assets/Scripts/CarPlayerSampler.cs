using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class SampleLap
{
    public List<CarTracePosition> PositionSamples;
    public float LapTime;
}


[System.Serializable]
public class RaceSample
{
    public List<SampleLap> LapSamples;
    public float TotalTime;
    public string TrackName;
}

/**
 * Clase encargada de realizar muestras del recorrido de un jugador durante la carrera para eventualmente
 * utilizarlas como interpolación para el recorrido en modo replay o para un fantasma de la pista.
 */

public class CarPlayerSampler : MonoBehaviour
{

    public RaceSample CurrentSamples;
    public Transform SamplingTarget;
    public Rigidbody SamplingPhysics;
    public CarManager SamplingManager;
    public List<CarTracePosition> SamplePositions;
    public float timeBetweenSamples = 0.5f;
    private float currenttimeBetweenSamples = 0.0f;
    private int CurrentLap = 0;

    void Start()
    {
        CurrentSamples = new()
        {
            LapSamples = new List<SampleLap>(),
            TotalTime = 0f
        };
        List<SampleLap> lapSamples = new List<SampleLap>();
        SamplePositions = new List<CarTracePosition>();
        CurrentLap = SamplingManager.CurrentLap;
    }

    void Update()
    {
        //Solo se va a hacer muestreo mientras el vehiculo este habilitado para correr
        if (SamplingManager.CarEnabled)
        {
            if (CurrentLap == SamplingManager.CurrentLap)
            {
                // A cada frame incrementamos el tiempo transcurrido 
                currenttimeBetweenSamples += Time.deltaTime;

                // Si el tiempo transcurrido es mayor que el tiempo de muestreo
                if (currenttimeBetweenSamples >= timeBetweenSamples)
                {
                    // Guardamos la información para el fantasma
                    AddNewData(SamplingTarget, SamplingPhysics.velocity.magnitude);
                    // Dejamos el tiempo extra entre una muestra y otra
                    currenttimeBetweenSamples -= timeBetweenSamples;
                }
            }
            else
            {
                SampleLap finishedLap = new()
                {
                    LapTime = SamplingManager.TimeLaps[CurrentLap - 1],
                    PositionSamples = SamplePositions
                };

                CurrentSamples.LapSamples.Add(finishedLap);
                CurrentSamples.TotalTime += finishedLap.LapTime;
                SamplePositions = new List<CarTracePosition>();
                CurrentLap++;
            }
        }
        if(CommonDataSingleton.instance.RaceFinished)
        {
            CommonDataSingleton.instance.PlayerRecordSample = CurrentSamples;
        }

    }

    public void AddNewData(Transform transform, float speed)
    {
        CarTracePosition newPosition = new CarTracePosition(transform.position, transform.rotation, speed);
        SamplePositions.Add(newPosition);
    }

}
