using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using Photon.Pun;

public class BulletTrailScript : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 targetPos;
    private float progress;
    PhotonView view;

    [SerializeField] private float speed = 40f;

    void Start() {
        startPos = transform.position.WithAxis(Axis.Z, -1);        
    }

    void Update() {
        progress += Time.deltaTime * speed;
        transform.position = Vector3.Lerp(startPos, targetPos, progress);
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        targetPos = targetPosition.WithAxis(Axis.Z, -1);
        view = GetComponent<PhotonView>();
        view.RPC(nameof(setTargetPosHelper), RpcTarget.All, targetPos.x, targetPos.y, targetPos.z);

    }
    
    [PunRPC]
    public void setTargetPosHelper(float x, float y, float z) {
        targetPos = new Vector3(x, y, z);
    }
}
