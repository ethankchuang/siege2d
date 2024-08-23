using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class SecondaryGadgetScript : MonoBehaviour
{ 
    [SerializeField] GameObject flashbangScript;
    [SerializeField] GameObject grenadeScript;
    [SerializeField] GameObject breachChargeScript;
    [SerializeField] GameObject flareScript;
    public int maxGrenades;
    private ISecondaryGadget currentGadgetScript;
    private float strength;
    private Light2D myLight2D;
    public GrenadeHUD hud;
    private UnityEngine.UI.Image flashOverlay;
    private GameObject flashOverlayGO;

    //private Light2D targetLight;
    PhotonView view;
    Game game;
    

    public void setCurrentGadget(int num)
    {
        view.RPC(nameof(setCurrentGadgetHelper), RpcTarget.All, num);
    }
    [PunRPC]
    public void setCurrentGadgetHelper(int scriptNum) {
        if (scriptNum == 1) {
            currentGadgetScript = grenadeScript.GetComponent<GrenadeScript>();
            maxGrenades = 4;
        } else if (scriptNum == 2) {
            currentGadgetScript = flareScript.GetComponent<FlareScript>();
            maxGrenades = 4;
        } else if (scriptNum == 3) {
            currentGadgetScript = breachChargeScript.GetComponent<BreachChargeScript>();
            maxGrenades = 3;
        } else if (scriptNum == 4) {
            currentGadgetScript = flashbangScript.GetComponent<FlashbangScript>();
            maxGrenades = 3;
        }
        if (view.IsMine) {
            hud.setCurrentGrenades(maxGrenades);
            hud.setSprite(currentGadgetScript.GetSprite());
        }
    }
    void Start()
    {
        view = GetComponent<PhotonView>();
        game = GameObject.Find("Game").GetComponent<Game>();
        hud = GameObject.Find("Canvas").GetComponent<GrenadeHUD>();
        flashOverlay = GameObject.Find("Canvas").transform.Find("Flash").GetComponent<UnityEngine.UI.Image>();
        flashOverlayGO = GameObject.Find("Canvas").transform.Find("Flash").gameObject;
        //Debug.Log((flashOverlay == null) + " flash overlay null??? poopy butthole fart");

    }

    void Update()
    {
        //Debug.Log(currentGadgetScript.readyToActivate());
        if (view.IsMine)
        {
            if (gameObject.GetComponent<PlayerMovement>().hasGadget && hud.currentGrenades > 0) {
                if (Input.GetKey("g"))
                {
                    //Debug.Log("g key is down");
                    strength += Time.deltaTime;
                }
                if (Input.GetKeyUp("g"))
                {
                    if (strength >= 5) {strength = 5;}
                    
                    var grenadeInstance = PhotonNetwork.Instantiate(currentGadgetScript.getName(), transform.GetChild(2).Find("FirePoint").position, transform.GetChild(2).Find("FirePoint").rotation);
                    grenadeInstance.transform.GetChild(0).gameObject.SetActive(true);
                    grenadeInstance.GetComponent<ISecondaryGadget>().throwGadget(strength, gameObject);
                    
                    strength = 0;
                    hud.decrementGrenades();
                }
            }
            //Debug.Log((flashOverlay == null) + " flash overlay null???");
            if (flashOverlay.color.a > 0) {
                //Debug.Log("changing overlay");
                var tempColor = flashOverlay.color;
                tempColor.a -= (float)(Time.fixedDeltaTime * 0.07);
                flashOverlay.color = tempColor;
                //Debug.Log("reducing flash coloring " + flashOverlay.color.a);
                /*if (myLight2D.intensity <= 1)
                {
                    myLight2D.intensity = 1;
                    //myLight2D.pointLightOuterAngle = 180;
                    myLight2D.pointLightInnerAngle = 145;
                }*/
            } else {
                flashOverlayGO.SetActive(false);
            }
        }
    }
    [PunRPC] 
    public void setLight(float percent, float maxBrightness, int id) {
        game.setAliveLists(null);

        foreach (GameObject player in game.atkAlive) {
            if (player.GetComponent<PlayerMovement>().myID == id) {
                //player.transform.GetChild(2).Find("Spot Light 2D").GetComponent<Light2D>().intensity = (maxBrightness * percent) + 1;
                if (player.GetComponent<PlayerMovement>().myID == PhotonNetwork.LocalPlayer.ActorNumber) {
                    flashOverlayGO.SetActive(true);
                    var tempColor = flashOverlay.color;
                    tempColor.a = maxBrightness * percent;
                    flashOverlay.color = tempColor;
                    //Debug.Log("was flashed " + flashOverlay.color.a);
                }
                return;
            }
        }
        foreach (GameObject player in game.defAlive) {
            if (player.GetComponent<PlayerMovement>().myID == id) {
                //player.transform.GetChild(2).Find("Spot Light 2D").GetComponent<Light2D>().intensity = (maxBrightness * percent) + 1;
                //return;
                if (player.GetComponent<PlayerMovement>().myID == PhotonNetwork.LocalPlayer.ActorNumber) {
                    flashOverlayGO.SetActive(true);
                    var tempColor = flashOverlay.color;
                    tempColor.a = maxBrightness * percent;
                    flashOverlay.color = tempColor;
                    //Debug.Log("was flashed " + flashOverlay.color.a);
                }
                return;
            }
        }
    }
    public void setLightHelper(float percent, float maxBrightness, int id) {
        view.RPC(nameof(setLight), RpcTarget.All, percent, maxBrightness, id);
    }
    [PunRPC]
    public void dealDamage(float dmgPercent, int maxDamage, int id) {
        game.setAliveLists(null);
         foreach (GameObject player in game.atkAlive) {
            if (player.GetComponent<PlayerMovement>().myID == id) {
                player.GetComponent<PlayerMovement>().takeDmg((int)(maxDamage * dmgPercent));
                return;
            }
        }
        foreach (GameObject player in game.defAlive) {
            if (player.GetComponent<PlayerMovement>().myID == id) {
                player.GetComponent<PlayerMovement>().takeDmg((int)(maxDamage * dmgPercent));
                return;
            }
        }
        
    }
    public void dealDamageHelper(float dmgPercent, int maxDamage, int id) {
        view.RPC(nameof(dealDamage), RpcTarget.All, dmgPercent, maxDamage, id);
    }

    /*[PunRPC]
    public void throwGadget(float strength)
    {
        currentGadgetScript.throwGadget(strength, gameObject);
    } */
}
