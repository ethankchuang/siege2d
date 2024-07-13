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

    Vector3 pos;

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
            
            pos = new Vector3(transform.position.x, transform.position.y /*+ 5*/, transform.position.z);

            cameraHolder.transform.position = pos;
            //Debug.Log(cameraHolder.transform.position);
            cameraHolder.transform.rotation = Quaternion.identity;
        }
    }
}
