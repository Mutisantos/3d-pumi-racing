using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ReplayType
{
    PLAYER, GHOST
}

/**
 * Clase que gestiona el recorrido de una carro en una pista basado en un recorrido establecido.
 * Se utiliza tanto para que el fantasma haga su recorrido como para hacer un replay de la carrera.
 */
public class ReplayController : MonoBehaviour
{
    //Shader usado para la transparencia del fantasma
    public Shader TransparentShader;
    //Muestras del recorrido para el fantasma
    public RaceSample ghostSamples;
    //Muestras del recorrido hecho por el jugador al final de la carrera
    public RaceSample replaySamples;
    //Lista de Transform con la cual la cinemachine va a apuntar al momento de hacer replay
    public List<Transform> CameraFollowAngles;
    //Valor para identificar si se trata de un Replay o de un Fantasma
    public ReplayType FollowingType = ReplayType.GHOST;
    //Lista de vueltas que va a seguir el componente al que se le aplique este comportamiento
    public List<SampleLap> lapSamples;
    //Rango de muestreo utilizado
    public float timeBetweenSamples = 0.5f;
    public int currentSampleToPlay = 0;

    //Indice de la camara que se esta usando para mostrar (CameraFollowAngles)
    private int CameraIndex = 0;
    //Referencias a la posición y rotación interpolada actual y la objetivo
    private Vector3 lastSamplePosition;
    private Vector3 nextPosition;
    private Quaternion lastSampleRotation;
    private Quaternion nextRotation;
    private float CurrenttimeBetweenPlaySamples = 0;
    private float CurrentTimeBetweenSwaps = 0;
    //Contador de vueltas para poder iterar en tantas vueltas como hayan en lapSamples
    private int lapIteration = 0;
    [SerializeField]
    //Virtual Camara asociada al Vehiculo
    private CinemachineVirtualCamera VirtualCamera;

    void Start()
    {
        lapIteration = 0;
        CurrenttimeBetweenPlaySamples = 0;
        CurrentTimeBetweenSwaps = 0;
        //Se carga las muestras del recorrido del jugador. Esto tambien ocurre cuando habilita el componente si estaba deshabilitado
        replaySamples = CommonDataSingleton.instance.PlayerRecordSample;
        VirtualCamera = this.GetComponentInChildren<CinemachineVirtualCamera>();
        if (FollowingType == ReplayType.GHOST)
        {
            ghostSamples = CommonDataSingleton.instance.LoadSamplesFile(CommonDataSingleton.instance.ChosenTrackName);
            PaintAllModelsTransparent();
            if (ghostSamples != null)
            {
                lapSamples = ghostSamples.LapSamples;
            }
            else
            {
                //Si la pista no posee fantasma, el fantasma se desactiva.
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            lapSamples = replaySamples.LapSamples;
        }
    }


    void Update()
    {
        if (!CommonDataSingleton.instance.RaceFinished)
        {
            MoveThroughWaypoint();
            SwapCameras();
        }
        else
        {
            var nextPos = lapSamples[0].PositionSamples[0];
            this.transform.position = nextPos.Position;
            this.transform.rotation = nextPos.Rotation;
        }
    }

    /**
     * Metodo para cambiar paulatinamente desde cual posicion se muestra el vehiculo en el replay
     */
    private void SwapCameras()
    {
        // Si el tiempo transcurrido es mayor que el tiempo de muestreo
        if (!CommonDataSingleton.instance.RaceFinished && FollowingType == ReplayType.PLAYER)
        {
            // A cada frame incrementamos el tiempo transcurrido 
            CurrentTimeBetweenSwaps += Time.deltaTime;
            if (CameraIndex == this.CameraFollowAngles.Count - 1)
            {
                CameraIndex = 0;
            }
            else
            {
                CameraIndex++;
            }
            if (CurrentTimeBetweenSwaps >= 5)
            {
                CurrentTimeBetweenSwaps -= 5;
                VirtualCamera.m_Follow = this.CameraFollowAngles[CameraIndex];
                VirtualCamera.m_LookAt = this.CameraFollowAngles[CameraIndex];
            }
        }
    }

    /**
     * Metodo para pintar todos los modelos del carro de manera transparente para simular ser un fantasma.
     */
    private void PaintAllModelsTransparent()
    {
        MeshRenderer[] renderers = this.gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                var color = renderers[i].material.color;
                color.a = 0.5f;
                renderers[i].material.color = color;
            }
            renderers[i].material.shader = TransparentShader;
        }

        SkinnedMeshRenderer[] skinRenderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skinRenderers.Length; i++)
        {
            var color = renderers[i].material.color;
            color.a = 0.5f;
            skinRenderers[i].material.color = color;
            skinRenderers[i].material.shader = TransparentShader;
            Debug.Log($"{skinRenderers[i].name}");
        }
    }

    /**
     *  Metodo para mover el vehiculo a través de las muestras obtenidas.
     */
    public void MoveThroughWaypoint()
    {
        //Mientras hayan vueltas por recorrerse, se procederá con el recorrido interpolado.
        if (lapIteration < lapSamples.Count)
        {
            // A cada frame incrementamos el tiempo transcurrido 
            CurrenttimeBetweenPlaySamples += Time.deltaTime;
            int sampleNumber = lapSamples[lapIteration].PositionSamples.Count;
            // Si el tiempo transcurrido es mayor que el tiempo de muestreo, significa que hay que ir por la siguiente muestra
            if (CurrenttimeBetweenPlaySamples >= timeBetweenSamples)
            {
                // Guardar la posición y rotación anterior para poder interpolar el movimiento a la próxima.
                lastSamplePosition = nextPosition;
                lastSampleRotation = nextRotation;
                //Si se exceden el numero de muestras de la vuelta actual, se pasa a la siguiente
                if (sampleNumber <= currentSampleToPlay)
                {
                    lapIteration++;
                    currentSampleToPlay = 0;
                }
                if (lapIteration == lapSamples.Count)
                {
                    return;
                }
                // Se toman los datos previamente cargados de las muestras de vuelta a utilizar
                var nextPos = lapSamples[lapIteration].PositionSamples[currentSampleToPlay];
                nextPosition = nextPos.Position;
                nextRotation = nextPos.Rotation;

                // Se reinicia el contador del muestreo
                CurrenttimeBetweenPlaySamples -= timeBetweenSamples;

                // Se incrementa el contador de muestras
                currentSampleToPlay++;
            }

            float percentageBetweenFrames = CurrenttimeBetweenPlaySamples / timeBetweenSamples;
            this.transform.position = Vector3.Slerp(lastSamplePosition, nextPosition, percentageBetweenFrames);
            this.transform.rotation = Quaternion.Slerp(lastSampleRotation, nextRotation, percentageBetweenFrames);
        }
        else
        {
            //Una vez el fantasma haya acabado la carrera, desaparecerá
            if (FollowingType == ReplayType.GHOST)
            {
                this.gameObject.SetActive(false);
                StopAllCoroutines();
            }
            //Si es el replay del jugador, se vuelve a ubicar en el primer punto de sampleo
            else
            {
                var nextPos = lapSamples[0].PositionSamples[0];
                this.transform.position = nextPos.Position;
                this.transform.rotation = nextPos.Rotation;
                lapIteration = 0;
                CurrenttimeBetweenPlaySamples = 0;
                CurrentTimeBetweenSwaps = 0;
                CommonDataSingleton.instance.FinishRace();
            }
        }

    }


}
