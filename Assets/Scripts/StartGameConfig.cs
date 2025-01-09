using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartGameConfig : MonoBehaviour
{
    private TimerConfig timerConfig;
    private PuzzleConfig puzzleConfig;
    public float timeToSet;
    public int puzzleSolved;

    public void Start()
    {
        timerConfig = GameObject.Find("Timer").GetComponent<TimerConfig>();
        puzzleConfig = GameObject.Find("Wires_puzzle").GetComponent<PuzzleConfig>();
        timeToSet = 120f;
        puzzleSolved = 0;
        if (timerConfig != null)
        {
            timerConfig.OnTimerEnd += HandleTimerEnd; // event that trigers when timer runs out
        }
        if (puzzleConfig != null)
        {
            puzzleConfig.OnGameEnd += HandleGameEnd; // event that trigers when player solves the wires puzzle
        }
    }
    public void StartGame()
    {
        Debug.Log("Start game");
        Debug.Log("timeToSet: " + timeToSet);
        timerConfig.StartTimer(timeToSet);
        puzzleConfig.StartGame();
    }

    void HandleTimerEnd()
    {
        Debug.Log("Game Over");
        GameEnd();
    }
    void HandleGameEnd()
    {
        puzzleSolved++;
        if (puzzleSolved >= 2) // imamo samo dva enventa pa je zato 2
        {
            Debug.Log("Game Won");
            GameEnd();
        }
    }

    public void AddStrike()
    {
        timerConfig.SubtractTime(15f);
    }

    public void GameEnd()
    {
        //Things needed to end the game
    }

    private void OnDestroy()
    {
        if (timerConfig != null)
        {
            timerConfig.OnTimerEnd -= HandleTimerEnd; // Unsubscribe to avoid memory leaks
        }
        if (puzzleConfig != null)
        {
            puzzleConfig.OnGameEnd -= HandleGameEnd; // Unsubscribe to avoid memory leaks
        }
    }

}
