using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Photon.Pun;
using UnityEditor;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviourPunCallbacks, IShootAble
{
    public float walkSpeed;
    public float sprintSpeed;
    public float moveSpeed;
    public int MaxHealth = 100;
    int health;
    public Rigidbody2D rb;
    public Camera cam;
    bool playerLocked;
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
    private GameObject currentWeapon;
    private IWeaponScript currentWeaponScript;
    float lightOuterAngle;
    float lightInnerAngle;
    //private GameObject deathScreen;
    private Game game;
    public bool isDead;
    public bool isDef;

    NewTimer gameTimer;
    [SerializeField] private GameObject flashBang;
    private GameObject reticle;
    UnityEngine.Vector3 reticlePos;
    [SerializeField] GameObject selfSprite;
    [SerializeField] GameObject shadowSprite;
    //[SerializeField] GameObject spectateCam;
    GameObject spectateCam;
    GameObject spectateCamInstance;
    private DeathScreen deathScreen;
    private WinRound roundWinScreen;
    private LoseRound roundLossScreen;
    private WinGameBG gameWinScreen;
    private LoseGameBG gameLoseScreen;
    bool temp = true;
    private bool isSpectating;
    public string nickName;
    SecondaryGadgetScript secondaryGadgetScript;
    public bool hasWeapon;
    public bool hasGadget;
    GameObject loadoutSelect;
    bool inPrepPhase;
    public void Start()
    {
        view = GetComponent<PhotonView>();
        loadoutSelect = GameObject.Find("Canvas").transform.Find("LoadoutSelect").gameObject;
        if (view.IsMine)
        {
            spotLight2D.SetActive(true);
            selfSprite.SetActive(true);
            shadowSprite.SetActive(false);
            nickName = PhotonNetwork.LocalPlayer.NickName;    
            loadoutSelect.GetComponent<LoadoutSelect>().setMyPlayer(gameObject);
        }
        secondaryGadgetScript = GetComponent<SecondaryGadgetScript>();
        //secondaryGadgetScript.setCurrentGadget(flashBang.GetComponent<FlashbangScript>());
        //currentWeaponScript = currentWeapon.GetComponent<IWeaponScript>();
        //currentWeaponScript.onStart();

        health = MaxHealth;
        isDef = gameObject.GetComponent<PlaceDefuser>() == null;
        defaultCamScript = GameObject.Find("DefaultCameras").GetComponent<DefaultCamScript>();
        healthBar = GameObject.Find("HealthBar");
        healthBarScript = healthBar.GetComponent<HealthBarScript>();
        healthBarScript.setSliderMax(health);
        // testing secondary gadgets
        lightOuterAngle = spotLight2D.GetComponent<Light2D>().pointLightOuterAngle;
        lightInnerAngle = spotLight2D.GetComponent<Light2D>().pointLightInnerAngle;
        deathScreen = GameObject.Find("Canvas").transform.Find("DeathBG").GetComponent<DeathScreen>();
        roundWinScreen = GameObject.Find("Canvas").transform.Find("WinRoundBG").GetComponent<WinRound>();
        roundLossScreen = GameObject.Find("Canvas").transform.Find("LoseRoundBG").GetComponent<LoseRound>();
        gameWinScreen = GameObject.Find("Canvas").transform.Find("WinGameBG").GetComponent<WinGameBG>();
        gameLoseScreen = GameObject.Find("Canvas").transform.Find("LoseGameBG").GetComponent<LoseGameBG>();
        game = GameObject.Find("Game").GetComponent<Game>();
        //Debug.Log((game == null) + " game = null");
        spectateCam = GameObject.Find("SpectatingCam").transform.GetChild(0).gameObject;
        reticle = GameObject.Find("reticle");
        isSpectating = false;
        gameTimer = GameObject.Find("Canvas").GetComponent<NewTimer>();
        inPrepPhase = true;
    }

    public void Update()
    {
        if (view.IsMine)
        {
            if (!isDead)
            {
                if (inPrepPhase && gameTimer.inPrep) {
                    //Debug.Log(hasGadget + " has gadget" + " from " + nickName);
                    //Debug.Log(hasWeapon + " has weapon" + " from " + nickName);
                    if (!hasGadget || !hasWeapon) {

                        loadoutSelect.SetActive(true);
                        Cursor.visible = true;
                    } else {
                        Debug.Log("has gadget and has weapon, ending prep phase");
                        loadoutSelect.SetActive(false);
                        inPrepPhase = false;
                        Cursor.visible = false;
                    }
                } else if (inPrepPhase && !(hasGadget && hasWeapon)) {
                    if (!hasGadget) {
                        Debug.Log("giving flashbang default");
                        changeGadget(flashBang.GetComponent<FlashbangScript>());
                    }
                    if (!hasWeapon) {
                        Debug.Log("giving assault rifle default");
                        changeWeapon(GameObject.Find("AssaultRifle"));
                    }
                    inPrepPhase = false;
                    loadoutSelect.SetActive(false);
                    Cursor.visible = false;
                }

                movement.x = Input.GetAxisRaw("Horizontal");
                movement.y = Input.GetAxisRaw("Vertical");
                mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
                reticlePos = new UnityEngine.Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y, cam.ScreenToWorldPoint(Input.mousePosition).z + 30);
                reticle.transform.position = reticlePos;
                //sprinting
                if (Input.GetKey(KeyCode.LeftShift)) moveSpeed = sprintSpeed;
                else moveSpeed = walkSpeed;

                if (hasWeapon) {
                    if (Input.GetButton("Fire1")) {
                        currentWeaponScript.shoot(transform.Find("FirePoint"), GetComponent<AudioSource>());
                    } if (Input.GetKeyDown("r")) {
                        currentWeaponScript.reload(GetComponent<AudioSource>());
                    } if (Input.GetButtonDown("Fire2")) {
                        currentWeaponScript.aimDownSight(spotLight2D.GetComponent<Light2D>());
                    } if (Input.GetButtonUp("Fire2")) {
                        currentWeaponScript.hipFire(spotLight2D.GetComponent<Light2D>(), lightInnerAngle, lightOuterAngle);
                    }
                }

                if (Input.GetKeyDown("5") && !isWatchingCam) {
                    defaultCamScript.openCam(cameraHolder);
                    playerLocked = true;
                    isWatchingCam = true;
                    spotLight2D.SetActive(false);
                } else if (Input.GetKeyDown("5") && isWatchingCam) {
                    defaultCamScript.exitCam(cameraHolder);
                    playerLocked = false;
                    isWatchingCam = false;
                    spotLight2D.SetActive(true);
                } else if (isWatchingCam && Input.GetKeyDown("q")) {
                    defaultCamScript.scrollCamLeft(false);
                } else if (isWatchingCam && Input.GetKeyDown("e")) {
                    defaultCamScript.scrollCamRight(false);
                } 
            }
            else //if is dead
            { 
                if (!isSpectating && deathScreen.startSpectating) {
                    startSpectating();
                    deathScreen.gameObject.SetActive(false);
                    deathScreen.resetTimer();
                    isSpectating = true;
                }
            }
            if (roundWinScreen.doneCounting || roundLossScreen.doneCounting) {
                game.createPlayerLists();
                resetPlayer();
                game.resetMap();
                roundWinScreen.resetTimer();
                roundLossScreen.resetTimer();
                roundWinScreen.gameObject.SetActive(false);
                roundLossScreen.gameObject.SetActive(false);
                game.canEndRound = true;
            } else if (temp && (gameWinScreen.doneCounting || gameLoseScreen.doneCounting)) {
                temp = false;
                //Destroy(gameObject);
                Debug.Log("game ending returning to lobby");
                PhotonNetwork.AutomaticallySyncScene = false;
                SceneManager.LoadScene (sceneName:"Lobby");
                PhotonNetwork.LeaveRoom();
                //PhotonNetwork.LoadLevel("Lobby");
                
            }
        }
    }
    public void changeWeapon(GameObject weapon) {
        currentWeapon = weapon;
        currentWeaponScript = weapon.GetComponent<IWeaponScript>();
        currentWeaponScript.onStart();
        hasWeapon = true;
        Debug.Log("changing weapon " + hasWeapon + " from " + nickName);
    }
    public void changeGadget(ISecondaryGadget gadget) {
        secondaryGadgetScript.setCurrentGadget(gadget);
        hasGadget = true;
        Debug.Log("changing gadget " + hasGadget + " from " + nickName);
    }
    public void FixedUpdate()
    {
        if (view.IsMine && !playerLocked && !isDead)
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
        //Debug.Log("recieve hit called");
        view.RPC("TakeDamage", RpcTarget.All, damage);
    }

    public void takeDmg(int damage)
    {
        view.RPC("TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    public void TakeDamage(int damage)
    {
        //Debug.Log("take damage called");
        health -= damage;
        if (view.IsMine) {
            healthBarScript.adjustSlider(health);
        } else {
            //Debug.Log(health + " health");  
        }

        if (health <= 0 && view.IsMine) {

            game.setAliveLists(gameObject);
            Cursor.visible = true;
            reticle.SetActive(false);
            if (!(game.defAlive.Count <= 0 || game.atkAlive.Count <= 0)) {
                //maybe wait a couple seconds after death screen or smth
                //Debug.Log("game not over");
                deathScreen.gameObject.SetActive(true);
                deathScreen.startTimer();
                transform.GetChild(3).gameObject.SetActive(false);
            }

            if (game.defAlive.Count <= 0) {
                Debug.Log("atk won round");
                game.endRoundHelper(false);
            } else if (game.atkAlive.Count <= 0) {
                Debug.Log("def won round");
                game.endRoundHelper(true);
            }
        }
    }

    [PunRPC]
    public void playerDied() {
        transform.GetChild(4).gameObject.SetActive(false);
        transform.GetChild(5).gameObject.SetActive(false);
        transform.GetChild(6).gameObject.SetActive(false);
        isDead = true;
        game.setAliveLists(null);
    }

    public void roundEndScreen(bool win)
    {
        deathScreen.gameObject.SetActive(false);
        Debug.Log("round end called, did win? " + win);
        if (win) {
            roundWinScreen.gameObject.SetActive(true);
            roundWinScreen.startTimer();
        } else {
            roundLossScreen.gameObject.SetActive(true);
            roundLossScreen.startTimer();
        }
    }

    public void startSpectating()
    {
        //game.prepActiveList();

        //spectateCamInstance = Instantiate(spectateCam, new UnityEngine.Vector3(-5.5f, 0.5f, -12f), quaternion.identity);
        spectateCam.SetActive(true);
        //cameraHolder.SetActive(false);

        if (isDef) {
            defaultCamScript.allLightsOn();
        }

        if (isDef) {
            //Debug.Log(game.defAlive.Count + " def alive");
            //Debug.Log(game.defPlayerList.Count + " def player list");
            foreach (GameObject player in game.defAlive) {
                //Debug.Log();
                player.transform.GetChild(3).gameObject.SetActive(true);
                player.transform.GetChild(4).gameObject.SetActive(true);
                player.transform.GetChild(5).gameObject.SetActive(false);
            }
        }
        else {
            foreach (GameObject player in game.atkAlive) {
                player.transform.GetChild(3).gameObject.SetActive(true);
                player.transform.GetChild(4).gameObject.SetActive(true);
                player.transform.GetChild(5).gameObject.SetActive(false);
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

    public void resetPlayer()
    {
        if (view.IsMine) {
            transform.GetChild(4).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
            defaultCamScript.allLightsOff();
            foreach (GameObject player in game.defPlayerList) {
                //Debug.Log("def player list count " + game.defPlayerList.Count);
                if (player != gameObject)
                {
                    player.transform.GetChild(3).gameObject.SetActive(false);
                    player.transform.GetChild(4).gameObject.SetActive(false);
                    player.transform.GetChild(5).gameObject.SetActive(true);
                    player.transform.GetChild(6).gameObject.SetActive(true);
                }
            }
            foreach (GameObject player in game.atkPlayerList) {
                //Debug.Log("atk player list count " + game.atkPlayerList.Count);
                if (player != gameObject)
                {
                    player.transform.GetChild(3).gameObject.SetActive(false);
                    player.transform.GetChild(4).gameObject.SetActive(false);
                    player.transform.GetChild(5).gameObject.SetActive(true);
                    player.transform.GetChild(6).gameObject.SetActive(true);
                }
            }
        } else {
            transform.GetChild(5).gameObject.SetActive(true);
        }
        transform.GetChild(6).gameObject.SetActive(true);

        //Debug.Log("reset player called");

        // NEED TO FIX - other player sees health as 0 when they respawn!?
        //gameObject.GetComponent<PlayerMovement>().health = gameObject.GetComponent<PlayerMovement>().MaxHealth;
        view.RPC(nameof(setHealthMax), RpcTarget.All);

        
        isDead = false;
        hasWeapon = false;
        hasGadget = false;
        inPrepPhase = true;
        spectateCam.SetActive(false);
        roundLossScreen.gameObject.SetActive(false);
        roundWinScreen.gameObject.SetActive(false);
        isSpectating = false;
        reticle.SetActive(true);
        Destroy(spectateCamInstance);
        Cursor.visible = true;

        // respawning (maybe add a timer or some other intermediate?)
        SpawnPlayers spawnPlayers = game.gameObject.GetComponent<SpawnPlayers>();
        spawnPlayers.resetUsedLists();
        transform.position = spawnPlayers.respawn(gameObject);   
        gameTimer.restartTimer();
    }

    [PunRPC]
    public void setHealthMax() {
        health = MaxHealth;
    }

    public void gameEnd(bool win) {
        deathScreen.gameObject.SetActive(false);
        Debug.Log("game end called, did win? " + win);
        if (win) {
            gameWinScreen.gameObject.SetActive(true);
            gameWinScreen.startTimer();
        } else {
            gameLoseScreen.gameObject.SetActive(true);
            gameLoseScreen.startTimer();
        }
    }
}
