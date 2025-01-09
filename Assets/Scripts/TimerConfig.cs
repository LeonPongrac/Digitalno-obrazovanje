using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerConfig : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;

    private System.TimeSpan currentTime;
    private bool timerRunning = false;

    // Event to notify when the timer reaches 0
    public delegate void TimerEnded();
    public event TimerEnded OnTimerEnd;

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            currentTime = currentTime.Subtract(System.TimeSpan.FromSeconds(Time.deltaTime));

            if (currentTime.TotalSeconds <= 0)
            {
                currentTime = System.TimeSpan.Zero;
                StopTimer();
                OnTimerEnd?.Invoke();
            }

            //Debug.Log(currentTime.ToString());

            UpdateTimerDisplay();
        }
    }

    public void StartTimer(float timeInSeconds)
    {
        //Debug.Log(timeInSeconds);
        currentTime = System.TimeSpan.FromSeconds(timeInSeconds);
        //Debug.Log(currentTime.ToString());
        timerRunning = true;
        UpdateTimerDisplay();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void SubtractTime(float timeInSeconds)
    {
        currentTime = currentTime.Subtract(System.TimeSpan.FromSeconds(timeInSeconds));

        if (currentTime.TotalSeconds <= 0)
        {
            currentTime = System.TimeSpan.Zero;
            StopTimer();
            OnTimerEnd?.Invoke();
        }

        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        int minutes = currentTime.Minutes;
        int seconds = currentTime.Seconds;
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);

        if (currentTime.TotalSeconds < 60)
        {
            // Make the text blink red
            float t = Mathf.PingPong(Time.time, 0.5f) / 0.5f;
            timerText.color = Color.Lerp(normalColor, warningColor, t);
        }
        else
        {
            timerText.color = normalColor;
        }
    }
}
