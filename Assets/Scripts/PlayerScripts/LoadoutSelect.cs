using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class LoadoutSelect : MonoBehaviour
{
    /*[SerializeField] Button rifle;
    [SerializeField] Button shotgun;
    [SerializeField] Button grenade;
    [SerializeField] Button flare;
    [SerializeField] Button breach; 
    [SerializeField] Button flash;*/
    [SerializeField] GameObject rifleScript;
    [SerializeField] GameObject shotgunScript;
    [SerializeField] GameObject grenadeScript;
    [SerializeField] GameObject flareScript;
    [SerializeField] GameObject breachChargeScript;
    [SerializeField] GameObject flashbangScript;
    PlayerMovement playerMovement;
    public void Start() {
        Game game = GameObject.Find("Game").GetComponent<Game>();
    }
    public void setMyPlayer(GameObject player) {
        //Debug.Log("set player called for loadout selection from " + player.GetComponent<PlayerMovement>().nickName);
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    public void onRifle() {
        playerMovement.changeWeapon(rifleScript);
        //Debug.Log(playerMovement.hasWeapon + " player has weapon?");
    }
    public void onShotgun() {
        playerMovement.changeWeapon(shotgunScript);
        //Debug.Log(playerMovement.hasWeapon + " player has weapon?");
    }
    public void onGrenade() {
        playerMovement.changeGadget(1);
        //Debug.Log(playerMovement.hasGadget + " player has gadget?");
    }
    public void onFlare() {
        playerMovement.changeGadget(2);
        //Debug.Log(playerMovement.hasGadget + " player has gadget?");
    }
    public void onBreach() {
        playerMovement.changeGadget(3);
        //Debug.Log(playerMovement.hasGadget + " player has gadget?");
    }
    public void onFlash() {
        playerMovement.changeGadget(4);
        //Debug.Log(playerMovement.hasGadget + " player has gadget?");
    }
}
