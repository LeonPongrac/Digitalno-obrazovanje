using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartGameConfig : MonoBehaviour
{
    private TimerConfig timerConfig;
    private PuzzleConfig puzzleConfig;
    public float timeToSet;
    int strike;

    public void Start()
    {
        timerConfig = GameObject.Find("Timer").GetComponent<TimerConfig>();
        puzzleConfig = GameObject.Find("Wires_puzzle").GetComponent<PuzzleConfig>();
        timeToSet = 90f;
        strike = 0;
    }
    public void StartGame()
    {
        Debug.Log("Start game");
        Debug.Log("timeToSet: " + timeToSet);
        timerConfig.StartTimer(timeToSet);
        puzzleConfig.StartGame();
    }

    public void AddStrike()
    {
        strike++;
        if (strike > 2)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over");
    }
}
