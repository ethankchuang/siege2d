using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;

public class PlayerMovement : MonoBehaviour, IShootAble
{
    public float walkSpeed;
    public float sprintSpeed;
    public float moveSpeed;
    public int health = 100;
    public Rigidbody2D rb;
    public Camera cam;
    Boolean playerLocked;
    PhotonView view;
    UnityEngine.Vector2 movement;
    UnityEngine.Vector2 mousePos;
    UnityEngine.Vector2 lookDir;
    public GameObject spotLight2D;
    private DefaultCamScript defaultCamScript;
    private bool isWatchingCam = false;
    public GameObject cameraHolder;
    private GameObject healthBar;
    private HealthBarScript healthBarScript;
    bool isDisarming = false;

    // weapon serialize field for testing
    [SerializeField] private GameObject currentWeapon;
    private IWeaponScript currentWeaponScript;
    float lightOuterAngle;
    float lightInnerAngle;
    private GameObject deathScreen;
    private Game game;
    private bool isDead;
    private bool isDef;

    // testing secondary gadgets
    [SerializeField] private GameObject flashBang;


    public void Start()
    {
        view = GetComponent<PhotonView>();
        if (view.IsMine)
        {
            spotLight2D.SetActive(true);
        }

        isDef = gameObject.GetComponent<PlaceDefuser>() == null;

        defaultCamScript = GameObject.Find("DefaultCameras").GetComponent<DefaultCamScript>();
        
        healthBar = GameObject.Find("HealthBar");
        healthBarScript = healthBar.GetComponent<HealthBarScript>();
        healthBarScript.setSliderMax(health);

        // testing secondary gadgets
        SecondaryGadgetScript secondaryGadgetScript = GetComponent<SecondaryGadgetScript>();
        secondaryGadgetScript.setCurrentGadget(flashBang.GetComponent<FlashbangScript>());

        lightOuterAngle = spotLight2D.GetComponent<Light2D>().pointLightOuterAngle;
        lightInnerAngle = spotLight2D.GetComponent<Light2D>().pointLightInnerAngle;

        currentWeaponScript = currentWeapon.GetComponent<ShotgunScript>();
        currentWeaponScript.onStart();

        deathScreen = GameObject.Find("Canvas").transform.Find("DeathBG").gameObject;
        game = GameObject.Find("Game").GetComponent<Game>();
    }

    public void Update()
    {
        if (view.IsMine)
        {

            if (!isDead)
            {
                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");

                mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

                //sprinting
                if (Input.GetKey(KeyCode.LeftShift)) moveSpeed = sprintSpeed;
                else moveSpeed = walkSpeed;

                if (Input.GetButton("Fire1"))
                {
                    //Debug.Log("fire1 down");
                    currentWeaponScript.shoot(transform.Find("FirePoint"));
                }      
                if (Input.GetKeyDown("r"))
                {
                    //Debug.Log("r down");
                    currentWeaponScript.reload();
                }
                if (Input.GetButtonDown("Fire2"))
                {
                    //Debug.Log("fire2 down");
                    currentWeaponScript.aimDownSight(spotLight2D.GetComponent<Light2D>());
                }
                if (Input.GetButtonUp("Fire2"))
                {
                    //Debug.Log("fire2 up");
                    currentWeaponScript.hipFire(spotLight2D.GetComponent<Light2D>(), lightInnerAngle, lightOuterAngle);
                }

            }

            //DEFAULT CAM SCRIPT
            if ((Input.GetKeyDown("5") && !isWatchingCam) || (Input.GetKeyDown("5") && isDead && defaultCamScript.isSpectating))
            {
                defaultCamScript.openCam(cameraHolder);
                playerLocked = true;
                isWatchingCam = true;
            }
            else if (Input.GetKeyDown("5") && isWatchingCam)
            {
                defaultCamScript.exitCam(cameraHolder);
                playerLocked = false;
                isWatchingCam = false;
            }
            else if (isWatchingCam && Input.GetKeyDown("q"))
            {
                defaultCamScript.scrollCamLeft(false);
            }
            else if (isWatchingCam && Input.GetKeyDown("e"))
            {
                defaultCamScript.scrollCamRight(false);
            }

            //SPECTATING SCRIPT
            else if (Input.GetKeyDown("5") && isDead && !defaultCamScript.isSpectating)
            {
                defaultCamScript.startSpectating(gameObject, isDef);
                //playerLocked = true;
                //isWatchingCam = true;
            }
            else if (Input.GetKeyDown("5") && isDead && defaultCamScript.isSpectating)
            {
                defaultCamScript.switchToCams();
            }
            else if (defaultCamScript.isSpectating && isDead && Input.GetKeyDown("q"))
            {
                defaultCamScript.spectateLeft(isDef);
            }
            else if (defaultCamScript.isSpectating && isDead && Input.GetKeyDown("e"))
            {
                defaultCamScript.spectateRight(isDef);
            } 
        }
    }


    public void FixedUpdate()
    {
        if (view.IsMine && !playerLocked)
        {
            if (movement.x != 0 && movement.y != 0) rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime / math.sqrt(2));
            else rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

            lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;
        }
    }

    public void RecieveHit(RaycastHit2D hit, int damage)
    {
        Debug.Log("recieved hit");
        view.RPC("TakeDamage", RpcTarget.All, damage);
    }

    public void takeDmg(int damage)
    {
        Debug.Log("take dmg called");
        view.RPC("TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        if (view.IsMine)
        {
            Debug.Log("took damage");
            health -= damage;

            healthBarScript.adjustSlider(health);

            if (health <= 0)
            {
                //deathScreen.SetActive(true);
                isDead = true;
                Debug.Log("player has died and game is over? " + !game.removeFromList(gameObject, isDef));
                if (!game.removeFromList(gameObject, isDef))
                {
                    //maybe wait a couple seconds after death screen or smth
                    defaultCamScript.startSpectating(gameObject, isDef);
                }
                // TODO follow teammates cam after set seconds
            }
        }
    }

    public void setPlayerLocker(Boolean value)
    {
        playerLocked = value;
    }

    public void setPlayerDisarming(bool value)
    {
        playerLocked = value;
        isDisarming = value;
    }

    public bool isPlayerDisarming()
    {
        return isDisarming;
    }

    public UnityEngine.Vector2 getPlayerRotation()
    {
        return lookDir;
    }
}
