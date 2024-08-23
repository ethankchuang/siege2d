using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DefaultCamScript : MonoBehaviour
{
    private List<GameObject> camList;
    private GameObject currentCam;
    private GameObject prevCam;
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

    public void allLightsOn()
    {
        foreach (GameObject cam in camList)
        {
            cam.gameObject.SetActive(true);
            cam.transform.parent.gameObject.GetComponent<CamScript>().lightOn();
            cam.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void allLightsOff()
    {
        foreach (GameObject cam in camList)
        {
            cam.gameObject.SetActive(false);
            cam.transform.parent.gameObject.GetComponent<CamScript>().lightOff();
            cam.transform.GetChild(0).gameObject.SetActive(true);
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
}
