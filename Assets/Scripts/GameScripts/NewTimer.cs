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
    [SerializeField] double timer;
    [SerializeField] double defuseTimer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bombPlantedText;
    ExitGames.Client.Photon.Hashtable CustomeValue;

    public void Start()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CustomeValue = new ExitGames.Client.Photon.Hashtable();
            startTime = PhotonNetwork.Time;
            startTimer = true;
            CustomeValue.Add("StartTime", startTime);
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomeValue);
            Debug.Log("timer started");
        }
        else
        {
            startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
            startTimer = true;
        }
    }

    public void Update()
    {

        if (!startTimer) return;


        timerIncrementValue = timer - (PhotonNetwork.Time - startTime);

        minutes = (int)Math.Floor(timerIncrementValue / 60);
        seconds = (int)Math.Floor(timerIncrementValue % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (timerIncrementValue <= 0)
        {
            Debug.Log("game over");
            startTimer = false;
        }
    }

    public void defuserPlaced()
    {
        startTime = PhotonNetwork.Time;
        timer = defuseTimer;
        bombPlantedText.SetText("bomb planted");
    }
}