using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using Photon.Pun;
using System.Xml.Serialization;
using System;

public class NewTimer : MonoBehaviour
{
    bool startTimer = false;
    double timerIncrementValue;
    double startTime;
    int minutes;
    int seconds;
    [SerializeField] double RoundTimer;
    double timer;
    [SerializeField] double defuseTimer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bombPlantedText;
    ExitGames.Client.Photon.Hashtable CustomeValue;

    // temp
    int waitCounter = 0;
    bool stopWaiting = true;
    bool bombDown;

    public void Start()
    {
        bombDown = false;
        timer = RoundTimer;
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            //Debug.Log("timer started");
        }
        else
        {
            waitCounter = 3;
            stopWaiting = false;
        }
    }

    public void Update()
    {
        
        if (!stopWaiting && waitCounter >= 0)
        {
            waitCounter --;
            if (waitCounter <= 0)
            {
                // slave client error here
                startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                startTimer = true;
            }
        }

        if (!startTimer) return;



        timerIncrementValue = timer - (PhotonNetwork.Time - startTime);

        minutes = (int)Math.Floor(timerIncrementValue / 60);
        seconds = (int)Math.Floor(timerIncrementValue % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (timerIncrementValue <= 0)
        {
            Debug.Log("game over");
            Game game = GameObject.Find("Game").GetComponent<Game>();
            game.endRound(!bombDown);
            startTimer = false;
        }
    }

    public void defuserPlaced()
    {
        startTime = PhotonNetwork.Time;
        timer = defuseTimer;
        bombPlantedText.SetText("bomb planted");
        bombDown = true;
    }

    public void restartTimer() {
        Debug.Log("restartTimer called");
        startTime = PhotonNetwork.Time;
        timer = RoundTimer;
        bombPlantedText.SetText("Plant the Bomb");
        bombDown = false;
    }
}