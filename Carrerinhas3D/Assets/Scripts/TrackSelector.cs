using TMPro;
using UnityEngine;
using UnityEngine.UI;

/** Script para seleccionar la pista y mostrar el record actual en cada una
 * Esteban.Hernandez
 */
public class TrackSelector : MonoBehaviour {

    public Button NextButtonSelector;
    public TextMeshProUGUI FirstLap;
    public TextMeshProUGUI SecondLap;
    public TextMeshProUGUI ThirdLap;
    public TextMeshProUGUI TotalTime;

    private void Start()
    {
        //Solo se habilita el boton para pasar a la siguiente escena si se ha seleccionado algo antes.
        NextButtonSelector.enabled = false;
    }

    public void SelectTrack(int index)
    {
        string trackName = CommonDataSingleton.instance.AvailableTrackNames[index];
        CommonDataSingleton.instance.ChosenTrackName = trackName;
        CommonDataSingleton.instance.ChosenTrackIndex = index;
        RaceSample record = CommonDataSingleton.instance.LoadSamplesFile(trackName);
        if (record != null && record.LapSamples.Count > 0)
        {
            FirstLap.text = $"V1:{CommonDataSingleton.instance.GenerateTimestampString(record.LapSamples[0].LapTime)}";
            SecondLap.text = $"V2:{CommonDataSingleton.instance.GenerateTimestampString(record.LapSamples[1].LapTime)}";
            ThirdLap.text = $"V3:{CommonDataSingleton.instance.GenerateTimestampString(record.LapSamples[2].LapTime)}";
            TotalTime.text = $"TOTAL:{CommonDataSingleton.instance.GenerateTimestampString(record.TotalTime)}";
        }
        else
        {
            FirstLap.text = $"NO DATA";
            SecondLap.text = $"NO DATA";
            ThirdLap.text = $"NO DATA";
            TotalTime.text = $"NO DATA";
        }
        NextButtonSelector.enabled = true;
    }
}
