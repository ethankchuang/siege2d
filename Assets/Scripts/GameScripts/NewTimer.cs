using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using Photon.Pun;
using System.Xml.Serialization;
using System;
using System.Threading;
using Photon.Realtime;

public class NewTimer : MonoBehaviourPunCallbacks
{
    bool startTimer = false;
    double timerIncrementValue;
    double startTime;
    public int minutes;
    public int seconds;
    [SerializeField] double RoundTimer;
    double timer;
    [SerializeField] double defuseTimer;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI bombPlantedText;
    ExitGames.Client.Photon.Hashtable CustomValue;
    bool hasUpdated;
    public bool inPrep;

    // testing
    double currentMatchTime;
    Coroutine timerCoroutine;


    // temp
    int waitCounter = 0;
    bool stopWaiting = true;
    bool bombDown;

    public void Start()
    {
        bombDown = false;
        hasUpdated = false;
        inPrep = true;
        timer = RoundTimer;
    
        CustomValue = new ExitGames.Client.Photon.Hashtable();
        startTime = PhotonNetwork.Time;
        //startTimer = true;
        CustomValue.Add("StartTime", startTime);
        CustomValue.Add("PlayersReady", 1);
            
            //Debug.Log("timer started");
            /*Debug.Log(CustomValue);
            Debug.Log(CustomValue.Keys);
            Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"]);
            Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"]);
            Debug.Log(PhotonNetwork.CurrentRoom.CustomProperties.Keys);
            Debug.Log(int.Parse(PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"].ToString()) + " master loaded, players ready");*/
        if (PhotonNetwork.LocalPlayer.IsMasterClient) {
            PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
        }
        waitCounter = 3;
        startTimer = false;
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged) {
        hasUpdated = true;
    }

    public void Update()
    {

        if (hasUpdated && waitCounter >= 0)
        {
            waitCounter --;
            if (waitCounter <= 0) {
                // slave client error here
                if (!PhotonNetwork.LocalPlayer.IsMasterClient) {
                    startTime = double.Parse(PhotonNetwork.CurrentRoom.CustomProperties["StartTime"].ToString());
                }
                var pr = (int)PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"] + 1;
                CustomValue.Remove("PlayersReady");
                CustomValue.Add("PlayersReady", pr);
                PhotonNetwork.CurrentRoom.SetCustomProperties(CustomValue);
                hasUpdated = false;
                //Debug.Log((int)PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"] + " players ready");
                startTimer = true;
            }
        }
        if (!hasUpdated || !startTimer || (int)PhotonNetwork.CurrentRoom.CustomProperties["PlayersReady"] < PhotonNetwork.CurrentRoom.PlayerCount) return;



        timerIncrementValue = timer - (PhotonNetwork.Time - startTime);

        minutes = (int)Math.Floor(timerIncrementValue / 60);
        seconds = (int)Math.Floor(timerIncrementValue % 60);
        if (inPrep && minutes < 3) {
            inPrep = false;
        }

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        if (timerIncrementValue <= 0)
        {
            Debug.Log("game over");
            Game game = GameObject.Find("Game").GetComponent<Game>();
            game.endRoundHelper(!bombDown);
            startTimer = false;
        }
    }

    // new Timer ??
    
    /*public void initializeMatchTimer() {
        currentMatchTime = RoundTimer;
        editUI();

        if (PhotonNetwork.IsMasterClient) {
            timerCoroutine = StartCoroutine(Timer());
        }
    }
    public void endGame() {
        if (timerCoroutine != null) {
            StopCoroutine(timerCoroutine);
        }
        currentMatchTime = 0;
        editUI();
    }
    private IEnumerator Timer () {
        yield return new WaitForSeconds(1f);

        currentMatchTime -= 1;

        if (currentMatchTime <= 0) {
            timerCoroutine = null;
            // end time
        } else {
            RefreshTimer_S();
            timerCoroutine - StartCoroutine(Timer());
        }
    }
    public void RefreshTimer_S() {

    }
    public void editUI() {
        minutes = (int)Math.Floor(timerIncrementValue / 60);
        seconds = (int)Math.Floor(timerIncrementValue % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }*/

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