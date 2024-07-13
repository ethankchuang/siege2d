using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CamScript : MonoBehaviour, IShootAble
{
    PhotonView view;
    bool isActive;
    [SerializeField] private GameObject redLight;

    public void Start()
    {
        isActive = true;
        view = GetComponent<PhotonView>();
    }

    // flash red light while being watched
    public void lightOn()
    {
        redLight.SetActive(true);
        Debug.Log("light on");
    }
    public void lightOff()
    {
        redLight.SetActive(false);
        Debug.Log("light off");
    }
    public bool getIsActive()
    {
        return isActive;
    }
      
    public void RecieveHit(RaycastHit2D hit, int damage)
    {
        Debug.Log(hit + " hit");
        Debug.Log(damage + " damage");
        Debug.Log(view + " view");

        view.RPC(nameof(TakeDamage), RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        gameObject.SetActive(false);
        isActive = false;
    }
}
