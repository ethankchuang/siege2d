using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
//using UnityEditor.Animations;

public class CameraController : MonoBehaviour
{
    public GameObject cameraHolder;
    PhotonView view;

    public void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            cameraHolder.SetActive(true);
        }
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name == "Game" && view.IsMine)
        {
            cameraHolder.transform.position = transform.position;
            //Debug.Log(cameraHolder.transform.position);
            cameraHolder.transform.rotation = Quaternion.identity;
        }
    }
}
