using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConnectToServer : MonoBehaviourPunCallbacks 
{
    public InputField inputField;
    public Text buttonText;

    public void onClickConnect()
    {
        if (inputField.text.Length >= 1)
        {
            PhotonNetwork.NickName = inputField.text;
            buttonText.text = "Connecting...";
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
        }
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
