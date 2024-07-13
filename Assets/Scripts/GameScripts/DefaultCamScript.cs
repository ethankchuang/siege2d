using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultCamScript : MonoBehaviour
{
    private List<GameObject> camList;
    private List<GameObject> atkPlayerList;
    private List<GameObject> defPlayerList;
    private GameObject currentCam;
    private GameObject prevCam;
    private GameObject currentPlayer;
    public bool isSpectating;
    private Game game;
    //private GameObject defaultCams;


    // NEED TO WRITE SMTH FOR WHEN HOST DIES


    public void Start()
    {
        camList = new List<GameObject>();

        for (int i = 0; i < gameObject.transform.childCount; i ++)
        {
            camList.Add(gameObject.transform.GetChild(i).GetChild(0).gameObject);
        }
        currentCam = camList[0];

        isSpectating = false;
        game = GameObject.Find("Game").GetComponent<Game>();

    }

    public void openCam(GameObject playerCam)
    {
        if (currentCam.transform.parent.gameObject.GetComponent<CamScript>().getIsActive())
        {
            currentCam.SetActive(true);
            playerCam.SetActive(false); 
            currentCam.transform.parent.gameObject.GetComponent<CamScript>().lightOn();  
        }
        else if (camList.IndexOf(currentCam) == camList.Count)
        {
            currentCam = camList[0];
            openCam(playerCam);
        }
        else
        {
            currentCam = camList[camList.IndexOf(currentCam) + 1];
            openCam(playerCam);
        }  
    }

    public void exitCam(GameObject playerCam)
    {
        currentCam.transform.parent.gameObject.GetComponent<CamScript>().lightOff();
        currentCam.SetActive(false);  
        playerCam.SetActive(true);
    }

    public void scrollCamLeft(bool isRecursion)
    {  
        if (!isRecursion)
        {
            prevCam = currentCam;
        }

        if (camList.IndexOf(currentCam) == 0)
        {
            currentCam = camList[camList.Count - 1];
        }
        else
        {
            currentCam = camList[camList.IndexOf(currentCam) - 1];
        }


        if (!currentCam.transform.parent.gameObject.GetComponent<CamScript>().getIsActive())
        {
            scrollCamLeft(true);
        }
        else
        {
            prevCam.transform.parent.gameObject.GetComponent<CamScript>().lightOff();
            currentCam.SetActive(true);
            prevCam.SetActive(false);
            currentCam.transform.parent.gameObject.GetComponent<CamScript>().lightOn();
        }
    }
    public void scrollCamRight(bool isRecursion)
    { 
        if (!isRecursion)
        {
            prevCam = currentCam;
        }


        if (camList.IndexOf(currentCam) == (camList.Count - 1))
        {
            currentCam = camList[0];
        }
        else
        {
            currentCam = camList[camList.IndexOf(currentCam) + 1];
        }

        if (!currentCam.transform.parent.gameObject.GetComponent<CamScript>().getIsActive())
        {
            scrollCamRight(true);
        }
        else
        {
            prevCam.transform.parent.gameObject.GetComponent<CamScript>().lightOff();
            currentCam.SetActive(true);
            prevCam.SetActive(false);
            currentCam.transform.parent.gameObject.GetComponent<CamScript>().lightOn();
        }
    }

    public void startSpectating(GameObject  player, bool isDef)
    {        
        Debug.Log("start spectating called ");
        atkPlayerList = game.atkAlive;
        defPlayerList = game.defAlive;
        player.transform.Find("CameraHolder").gameObject.SetActive(false);
        player.transform.Find("Spot Light 2D").gameObject.SetActive(false);

        isSpectating = true;
        // need to set active later

        if (isDef)
        {
            currentPlayer = defPlayerList[0];
        }
        else
        {
            currentPlayer = atkPlayerList[0];
        }
        currentPlayer.transform.Find("CameraHolder").gameObject.SetActive(true);
        currentPlayer.transform.Find("Spot Light 2D").gameObject.SetActive(true);

    }

    public void spectateLeft(bool isDef)
    {
        atkPlayerList = game.atkAlive;
        defPlayerList = game.defAlive;
        currentPlayer.transform.Find("CameraHolder").gameObject.SetActive(false);
        currentPlayer.transform.Find("Spot Light 2D").gameObject.SetActive(false);

        if (isDef)
        {
            if (defPlayerList.IndexOf(currentPlayer) == 0)
            {
                currentPlayer = defPlayerList[defPlayerList.Count - 1];
            }
            else 
            {
                currentPlayer = defPlayerList[defPlayerList.IndexOf(currentPlayer) - 1];
            }
        }
        else
        {
            if (atkPlayerList.IndexOf(currentPlayer) == 0)
            {
                currentPlayer = atkPlayerList[atkPlayerList.Count - 1];
            }
            else 
            {
                currentPlayer = atkPlayerList[atkPlayerList.IndexOf(currentPlayer) - 1];
            }
        }

        currentPlayer.transform.Find("CameraHolder").gameObject.SetActive(true);
        currentPlayer.transform.Find("Spot Light 2D").gameObject.SetActive(true);
    }
    
    public void spectateRight(bool isDef)
    {
        atkPlayerList = game.atkAlive;
        defPlayerList = game.defAlive;
        currentPlayer.transform.Find("CameraHolder").gameObject.SetActive(false);
        currentPlayer.transform.Find("Spot Light 2D").gameObject.SetActive(false);

        if (isDef)
        {
            if (defPlayerList.IndexOf(currentPlayer) == (defPlayerList.Count - 1))
            {
                currentPlayer = defPlayerList[0];
            }
            else 
            {
                currentPlayer = defPlayerList[defPlayerList.IndexOf(currentPlayer) + 1];
            }
        }
        else
        {
            if (atkPlayerList.IndexOf(currentPlayer) == (atkPlayerList.Count - 1))
            {
                currentPlayer = atkPlayerList[0];
            }
            else 
            {
                currentPlayer = atkPlayerList[atkPlayerList.IndexOf(currentPlayer) + 1];
            }
        }
        currentPlayer.transform.Find("CameraHolder").gameObject.SetActive(true);
        currentPlayer.transform.Find("Spot Light 2D").gameObject.SetActive(true);
    }

    public void switchToCams()
    {
        isSpectating = false;
        currentPlayer.transform.Find("CameraHolder").gameObject.SetActive(false);
        currentPlayer.transform.Find("Spot Light 2D").gameObject.SetActive(false);
        openCam(currentPlayer.transform.Find("CameraHolder").gameObject);
    }

    public List<GameObject> getCamList()
    {
        return camList;
    }
}
