using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour, IShootAble
{
    public float moveSpeed = 5f;
    public int health = 100;
    public Rigidbody2D rb;
    public Camera cam;

    PhotonView view;

    UnityEngine.Vector2 movement;
    UnityEngine.Vector2 mousePos;


    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
    }


    public void FixedUpdate()
    {
        if (view.IsMine)
        {
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

            UnityEngine.Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    public void RecieveHit(RaycastHit2D hit)
    {
        Debug.Log("recieved hit");
        view.RPC("TakeDamage", RpcTarget.All);
    }

    [PunRPC]
    public void TakeDamage()
    {
        Debug.Log("took damage");
        health -= 50;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
