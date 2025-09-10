using Cinemachine;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

/**
 * Clase que centraliza el control y gestión del Carro en escena y su 
 * interacción con otros componentes dentro de la lógica del juego.
 */
public class CarManager : MonoBehaviour
{
    public int MaxHealth;
    public float TerrainPenaltyFactor = 3;
    public float DamageThreshold;
    public bool CarEnabled = false;
    public bool IsPlayer = false;
    public AudioClip LapEffect;
    public AudioClip CollisionSound;
    //Referencia a los componentes del HUD del vehículo -TODO centralizar estos componentes en UIHelper
    public UIHelper CanvasHelper;
    public GameObject OffTrackParticlesHolder;
    //Referencia al objeto que visualice que el vehiculo ha sufrido daños por encima de un límite
    public GameObject DamageHolder;
    public Transform FrontCameraFollower;
    public Transform BackCameraFollower;
    [SerializeField]
    private int CurrentHealth;
    //Referencia a Scripts y componentes que hacen parte del prefab de vehiculo
    private Rigidbody Rigidbody;
    private CarController Controller;
    private CarUserControl InputController;
    [SerializeField]
    private CarLapManager LapManager;
    [SerializeField]
    private CinemachineVirtualCamera VirtualCamera;
    [SerializeField]
    private bool OnTrack = true;
    [SerializeField]
    private float InitialTime = 0f;
    [SerializeField]
    private float ElapsedTime = 0f;
    [SerializeField]
    private int LapNumber;
    public int CurrentLap { get { return LapNumber; } }
    [SerializeField]
    // Tiempos que ha hecho el carro, vuelta a vuelta
    private List<float> LapMarks;
    public List<float> TimeLaps  { get { return LapMarks; } }
    private float OriginalCapSpeed;

    void Awake()
    {
        LapMarks = new List<float>();
        LapManager =new CarLapManager();
        Controller = this.GetComponent<CarController>();
        InputController = this.GetComponent<CarUserControl>();
        Rigidbody = this.GetComponent<Rigidbody>();
        VirtualCamera = this.GetComponentInChildren<CinemachineVirtualCamera>();
        OffTrackParticlesHolder.SetActive(false);
        DamageHolder.SetActive(false);
        //Determina si el vehiculo es controlado por un jugador o debe usar la IA
        if (IsPlayer)
        {
            InputController.enabled = true;
        }
        ResetCarValues();
    }


    void Update()
    {
        if (CarEnabled)
        {
            ElapsedTime = Time.time - InitialTime;
            if (!OnTrack)
            {
                //Se reduce la velocidad progresivamente en cada frame en el que el auto no esté en la pista.
                Controller.ReduceLimit(Time.deltaTime * TerrainPenaltyFactor, OriginalCapSpeed * 0.3f);
            }
        }
        CanvasHelper.UpdateTimerText(ElapsedTime);
        
    }


    private void FixedUpdate()
    {
        CanvasHelper.UpdateSpeedText(Controller.CurrentSpeed);
    }


    //Visiviliza el daño producido a un vehiculo
    private void ProcessDamage()
    {
        if(CurrentHealth < DamageThreshold)
        {
            this.DamageHolder.SetActive(true);
        }
        else
        {
            this.DamageHolder.SetActive(false);
        }
    }


    public void OnCarEnabled(List<Transform> circuitWaypoints, Shortcut shortcut)
    {
        this.CarEnabled = true;
        if (IsPlayer)
        {
            this.InputController.SetCarEnabled(true);
            this.InputController.ForceStart();
        }
        if (this.LapManager.TotalWaypoints == 0)
        {
            this.LapManager.BuildCarCircuit(circuitWaypoints, shortcut);
        }
        InitialTime = Time.time;
        this.VirtualCamera.Follow = this.BackCameraFollower;
    }

    public void OnCarDisabled()
    {
        this.CarEnabled = false;
        if (IsPlayer)
        {
            this.InputController.SetCarEnabled(false); 
            this.Rigidbody.velocity = Vector3.zero;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Road"))
        {
            OnTrack = true;
            OffTrackParticlesHolder.SetActive(false);
            Controller.ResetLimit(OriginalCapSpeed);
        }
        if (other.CompareTag("Waypoint"))
        {
            ProcessWaypoint(other);
        }
        if (other.CompareTag("Shortcut"))
        {
            ProcessShortcut();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Road"))
        {
            OnTrack = false;
            OffTrackParticlesHolder.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Road"))
        {
            CurrentHealth -= 10;
            ProcessDamage();
            SoundManager.instance.PlayOnce(CollisionSound);
        }
    }

    private void ProcessShortcut()
    {
        LapManager.ApplyShortcut();
    }

    //Procesar el Waypoint, maracandolo como pasado, asi como controlar que el jugador tenga que recorrer toda la pista.
    private void ProcessWaypoint(Collider other)
    {
        int index = LapManager.PassWaypoint(other.transform);
        if (index >= 0)
        {
            Debug.Log($"Waypoint: {index} + {LapManager.TotalWaypoints}");
        }
        if (index == 0)
        {
            LapManager.ResetWaypointPassage();
            SoundManager.instance.PlayOnce(LapEffect, 1f);
            float lapTime = 0f;
            if (LapNumber == 1)
            {
                lapTime = ElapsedTime;
                CanvasHelper.UpdateFirstLapLabel(lapTime, 2);
                LapNumber = 2;
            }
            else if (LapNumber == 2)
            {
                lapTime = ElapsedTime - LapMarks[0];
                CanvasHelper.UpdateSecondLapLabel(lapTime, 3);
                LapNumber = 3;
            }
            else if (LapNumber == 3)
            {
                lapTime = ElapsedTime - LapMarks[1] - LapMarks[0];
                LapNumber = 4;
            }
            LapMarks.Add(lapTime);
            CommonDataSingleton.instance.LapMarks = LapMarks;
        }
    }

    //Reiniciar los valores del vehiculo tanto para inicio de escenas como para replays.
    public void ResetCarValues()
    {
        CurrentHealth = MaxHealth;
        ElapsedTime = 0;
        LapNumber = 1;
        OriginalCapSpeed = Controller.MaxSpeed;
        LapMarks.Clear();
    }




}
