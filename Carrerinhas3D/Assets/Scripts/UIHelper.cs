using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHelper : MonoBehaviour
{

    public GameObject CarHUDHolder;
    public GameObject RaceResultUIHolder;
    public TextMeshProUGUI CountdownText;
    public TextMeshProUGUI TitleText;
    public TextMeshProUGUI TimeText;
    public TextMeshProUGUI CurrentLapLabel;
    public TextMeshProUGUI FirstLap;
    public TextMeshProUGUI SecondLap;
    public TextMeshProUGUI SpeedLabel;
    public List<TextMeshProUGUI> LapTexts;
    public TextMeshProUGUI TotalTimeText;
    public Button SaveButton;
    public string CompletedMessage = "CARRERA COMPLETADA";
    public string RecordMessage = "NUEVO RECORD";
    private float CountdownFontSize;

    private void Awake()
    {
        CountdownFontSize = CountdownText.fontSize;
        FirstLap.gameObject.SetActive(false);
        SecondLap.gameObject.SetActive(false);
    }


    public float PerformCountdown(float countdownTime, AudioClip clip, float countdownSpeed)
    {
        int roofSeconds = (int)(countdownTime - Time.deltaTime * countdownSpeed);

        if (roofSeconds < (int)countdownTime)
        {
            CountdownText.text = $"{roofSeconds}";
            CountdownText.fontSize = CountdownFontSize;
            SoundManager.instance.PlayOnce(clip, 1f);
        }
        else
        {
            CountdownText.fontSize -= Time.deltaTime * 50;
        }
        if (roofSeconds <= 0)
        {
            countdownTime = 0;
        }
        countdownTime -= Time.deltaTime * countdownSpeed;
        return countdownTime;
    }

    public void FinishCountdown(WaitForSeconds StartWait)
    {
        StartCoroutine(StartFadeout(StartWait));
    }

    private IEnumerator StartFadeout(WaitForSeconds StartWait)
    {
        CountdownText.fontSize = CountdownFontSize;
        CountdownText.text = "GO";
        yield return StartWait;
        CountdownText.gameObject.SetActive(false);
    }

    public void EnableRecordOverwrite()
    {
        bool enabled = CommonDataSingleton.instance.IsNewRecordAchieved();
        SaveButton.enabled = enabled;
        TitleText.text = enabled ? RecordMessage : CompletedMessage;
    }

    public void ShowFinalResults()
    {
        List<float> marks = CommonDataSingleton.instance.LapMarks;
        // Start is called before the first frame update
        for(int i =0; i<marks.Count(); i++)
        {
            LapTexts[i].text = CommonDataSingleton.instance.GenerateTimestampString(marks[i]);
        }
        TotalTimeText.text = CommonDataSingleton.instance.GenerateTimestampString(marks.ToArray().Sum());
    }

    public void OverwriteRecord()
    {
        CommonDataSingleton.instance.SaveSamplesFile();
    }

    public void UpdateTimerText(float ElapsedTime)
    {
        TimeText.text = CommonDataSingleton.instance.GenerateTimestampString(ElapsedTime);
    }

    public void UpdateSpeedText(float CurrentSpeed)
    {
        SpeedLabel.text = $"{(int)CurrentSpeed} KM/H";
    }
    public void UpdateFirstLapLabel(float lapTime, int lapNumber)
    {
        FirstLap.text = CommonDataSingleton.instance.GenerateTimestampString(lapTime);
        FirstLap.gameObject.SetActive(true);
        CurrentLapLabel.text = $"{lapNumber}";
    }
    public void UpdateSecondLapLabel(float lapTime, int lapNumber)
    {
        SecondLap.text = CommonDataSingleton.instance.GenerateTimestampString(lapTime);
        SecondLap.gameObject.SetActive(true);
        CurrentLapLabel.text = $"{lapNumber}";
    }

    public void EnableCarHud()
    {
        FirstLap.text = "";
        SecondLap.text = "";
        RaceResultUIHolder.SetActive(false);
        CarHUDHolder.SetActive(true);
    }
    public void EnableResultUI()
    {
        RaceResultUIHolder.SetActive(true);
        CarHUDHolder.SetActive(false);
    }

    public void TriggerRaceReplay()
    {
        CommonDataSingleton.instance.ReplayRace();
    }


}
