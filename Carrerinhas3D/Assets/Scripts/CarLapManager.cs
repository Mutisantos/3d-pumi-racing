using System;
using System.Collections.Generic;
using UnityEngine;


/**
 * Clase que centraliza la gestión de Waypoints para controlar el recorrido de la carrera
 * Al subordinada por un CarManager, no necesita ser una clase MonoBehavior. 
 */
[Serializable]
public class CarLapManager
{
    [SerializeField]
    //Mapa que relaciona el transform con los datos del Waypoint
    public Dictionary<Transform, Waypoint> WaypointMap;
    [SerializeField]
    //Definición del atajo de la pista
    public Shortcut shortcut;
    //Referencia de la cantidad total de Waypoints que tiene la pista. 
    public int TotalWaypoints { get { return WaypointMap.Keys.Count; } }
    [SerializeField]
    //Contador de la cantidad de waypoints por los que ha pasado el vehiculo
    public int PassedWaypointCount = 0;

    // Constructor para tener un mapa vacío
    public CarLapManager()
    {
        WaypointMap = new Dictionary<Transform, Waypoint>();
    }

    // Llenado de datos de la información de la pista, para que tenga su propio listado de waypoints y atajos.
    public void BuildCarCircuit(List<Transform> waypointPositions, Shortcut shortcut)
    {
        int i = 0;
        foreach(var position in waypointPositions)
        {
            Waypoint trackWaypoint = new(position, i);
            WaypointMap.Add(position, trackWaypoint);
            i++;
        }
        this.shortcut = shortcut;
    }

    // Metodo llamado al finalizar una vuelta, para marcar todos los waypoints como si no se hubiese pasado por ellas.

    public void ResetWaypointPassage()
    {
        foreach(var tuple in WaypointMap)
        {
            tuple.Value.PassWaypoint(false);
        }
        PassedWaypointCount = 0;
    }


    // Metodo para aplicar un atajo, teniendo en cuenta el waypoint por el que sale el atajo y en donde desemboca.
    public void ApplyShortcut()
    {
        int initialIndex = WaypointMap.GetValueOrDefault(shortcut.InitialSkipWaypoint).Index;
        int finalIndex = WaypointMap.GetValueOrDefault(shortcut.FinalSkipWaypoint).Index;
        foreach (var waypoint in WaypointMap.Values)
        {
            if(waypoint.Index > initialIndex && waypoint.Index < finalIndex && !waypoint.Passed)
            {
                waypoint.PassWaypoint(true);
                PassedWaypointCount++;
            }
        }
    }

    // Metodo para marcar un waypoint como pasado usando su transform.
    // Es llamado en el momento en el que el CarManager detecta que se ha entrado en el trigger de un Waypoint.
    public int PassWaypoint(Transform passedWaypoint)
    {
        Waypoint waypoint = WaypointMap.GetValueOrDefault(passedWaypoint);
        if (waypoint.Passed)
        {
            //Si ya ha dado la vuelta completa, permite reiniciar los puntos
            if (waypoint.Index == 0 && PassedWaypointCount >= TotalWaypoints - 1) { 
                return 0;
            }
            //Impide procesar la meta si no se ha pasado por todos los waypoints
            return -1;
        }
        else
        {
            if (waypoint.Index >= PassedWaypointCount)
            {
                WaypointMap.GetValueOrDefault(passedWaypoint).PassWaypoint(true);
                PassedWaypointCount++;
                return PassedWaypointCount;
            }
            else
            {
                Debug.Log($"Backtracking detected at waypoint{waypoint.Index}, player currently at {PassedWaypointCount}");
                return -1;
            }
        }
    }


}
