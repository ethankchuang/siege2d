using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class SecondaryGadgetScript : MonoBehaviour
{ 
    private ISecondaryGadget currentGadgetScript;
    private float strength;
    private Light2D myLight2D;

    PhotonView view;
    
    public void setCurrentGadget(ISecondaryGadget CurrentGadgetScript)
    {
        currentGadgetScript = CurrentGadgetScript;

    }
    void Start()
    {
        view = GetComponent<PhotonView>();
        myLight2D = gameObject.transform.Find("Spot Light 2D").GetComponent<Light2D>();
    }

    void Update()
    {
        //Debug.Log(currentGadgetScript.readyToActivate());
        if (view.IsMine)
        {
            if (Input.GetKey("g"))
            {
                //Debug.Log("g key is down");
                strength += Time.deltaTime;
            }
            if (Input.GetKeyUp("g"))
            {
                //Debug.Log("strenght = " + strength);
                if (strength >= 5) {strength = 5;}

                //Debug.Log("g key is up");
                //currentGadgetScript.throwGadget(strength, gameObject);
                view.RPC(nameof(throwGadget), RpcTarget.All, strength);
                strength = 0;
            }

            //flashbang wear off 

            if (myLight2D.intensity > 1)
            {
                myLight2D.intensity -= Time.deltaTime;
                /*if (myLight2D.intensity <= 1)
                {
                    myLight2D.intensity = 1;
                    //myLight2D.pointLightOuterAngle = 180;
                    myLight2D.pointLightInnerAngle = 145;
                }*/
            }
        }
    }

    [PunRPC]
    public void throwGadget(float strength)
    {
        currentGadgetScript.throwGadget(strength, gameObject);
    } 
}
