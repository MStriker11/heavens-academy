using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class playerController : MonoBehaviourPunCallbacks, IDamageable, IPunObservable
{
    #region IPunObservable implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // we are owner
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else if (stream.IsReading) // other players
        {
            currentHealth = (float)stream.ReceiveNext();
        }
    }
    #endregion


    [SerializeField] GameObject cameraHolder;
    Camera POV;

    // dash zoom
    float zoomSpeed = 30, normalFOV = 60, zoomedFOV = 45;
    float minNormalAmount = 0, maxZoomedAmount = 1, lerpAmount = 0;
    public Volume normal, zoomed;

    // player health
    [SerializeField] Image healthBarImg;
    public GameObject[] ui;
    [SerializeField] Image worldHealthBarImg;

    // default: 3 6 3 250 0.15
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] GameObject BulletPrefab;

    [Header("Dash stats")]
    [SerializeField] float dashForce = 10;
    [SerializeField] AbilityInfo dashAbilityInfo;
    [SerializeField] Image dashIconBG, dashIcon, dashSilence;
    [SerializeField] Image litDashIconBG, litDashIcon;

    [Header("player stats")]
    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    private float deathHeight = -10f;

    float verticalLookRotation;
    bool grounded;
    Vector3 smoothMoveVelocity, moveAmount;

    //movement
    private float horizInput;
    private float vertInput;
    private bool isDashing;
    Vector3 moveDir;

    //flight
    private float flightHt = 6f;
    private float flightSpeed = 6f;
    private bool isFlying;

    Rigidbody rb;

    PhotonView PV;

    playerManager playerManager;

    //TODO: class loadout (make a diff playerController for each class)
    // [SerializeField] AbilityInfo[] abilityList;
    [Header("Basic Attack")]
    public BasicAttack basicAttack;
    [Header("Basic Ability")]
    public BasicSpell[] spellList;

    // spells logistic
    bool canUseSpell = true, canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        // photoview.find returns gameobject w/ given id
        // casting to int b/c instantiationData is a gameobject
        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<playerManager>();

        //
        POV = GetComponentInChildren<Camera>();

        dashAbilityInfo.currentCD = 0f;

        // dash icon
        litDashIconBG.enabled = true;
        litDashIcon.enabled = true;
        dashSilence.enabled = false;

        dashIconBG.fillAmount = 0;
        dashIcon.fillAmount = 0;

        // spell logic
        canUseSpell = true;

        DontDestroyOnLoad(litDashIconBG);
        DontDestroyOnLoad(litDashIcon);
        DontDestroyOnLoad(dashSilence);
        DontDestroyOnLoad(dashIconBG);
        DontDestroyOnLoad(dashIcon);

    }

    private void Start()
    {
        // if not local player get rid of unnecessary physics/obj
        if (!PV.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject); //get rid of camera and audio listener
            Destroy(rb);
            // if not local player destroy canvas
            //for (int i = 0; i < ui.Length; i++)
            //{
            //    Destroy(ui[i]);
            //}
        }
        isFlying = false;
    }

    private void Update()
    {
        // WASD
        moveDir = new Vector3(horizInput, 0, vertInput).normalized;
        horizInput = Input.GetAxisRaw("Horizontal");
        vertInput = Input.GetAxisRaw("Vertical");
        if (!PV.IsMine)
        {
            return;
        }
        //worldHealthBarImg.fillAmount = currentHealth / maxHealth;

        // flight is binded to z
        if (Input.GetKeyDown("z"))
        {
            isFlying = true;
            rb.useGravity = false;
            transform.position = new Vector3(transform.position.x, flightHt, transform.position.z);
        }

        Look();
        if (isFlying)
        {
            Fly();
            // TODO: for testing purposes - disable flying by pressing m
            if (Input.GetKeyDown("m"))
            {
                isFlying = false;
                DisableFlight();
            }
        }
        else if (canMove)
        {
            dashAbilityInfo.unSilenceIcon();
            //dashSilence.enabled = false;
            Dash();
            Move();
            Jump();
        }
        else
        {
            dashAbilityInfo.silenceIcon();
            dashSilence.enabled = true;
        }
        

        // respawn
        if (transform.position.y < deathHeight)
        {
            Die();
        }

        // basic ability
        if (canUseSpell)
        {
            for (int i = 0; i < spellList.Length; i++)
            {
                spellList[i].abilityInfo.unSilenceIcon();
                spellList[i].Activate();

            }
        }
        else
        {
            for (int i = 0; i < spellList.Length; i++)
            {
                spellList[i].abilityInfo.silenceIcon();
            }
        }

        // auto atk
        if (Input.GetMouseButtonDown(0))
        {
            basicAttack.Activate();
            photonView.RPC("Fire", RpcTarget.AllViaServer, GetComponent<Rigidbody>().position, GetComponent<Rigidbody>().rotation);
        }

    }

    #region spell logic
    private void OnCollisionEnter(Collision collision)
    {
        if (PV.IsMine) { return; }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            takeDamage(10);
        }
        if (collision.gameObject.CompareTag("Hammer"))
        {
            Debug.Log("hit by hammer. collision");
            takeDamage(30);
        }
        if (collision.gameObject.CompareTag("Dot"))
        {
            Debug.Log("hit by dot. collision");
            takeDamage(30);
        }
        if (collision.gameObject.CompareTag("Shield"))
        {
            canUseSpell = false;
        }
        if (collision.gameObject.CompareTag("Freeze"))
        {
            canUseSpell = false;
        }
        if (collision.gameObject.CompareTag("Beam"))
        {
            Invoke("takeDamage(20)", 0.25f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine) { return;  }
        if (other.CompareTag("Hammer"))
        {
            //Debug.Log("hit by hammer. trigger");
            takeDamage(30);
        }
        if (other.CompareTag("Shield"))
        {
            //Debug.Log("shield");
            canUseSpell = false;
        }
        //if (other.CompareTag("Dot"))
        //{
        //    Debug.Log("hit by dot. trigger");
        //    takeDamage(30);
        //}
        if (other.CompareTag("Freeze"))
        {
            canUseSpell = false;
            canMove = false;
        }
        if (other.CompareTag("Beam"))
        {
            Invoke("takeDamage(20)", 0.25f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Dot"))
        {
            takeDamage(0.25f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canUseSpell = true;
        canMove = true;
    }

    #endregion spell logic

    #region movement

    void Dash()
    {
        dashSilence.enabled = false;
        if (!canMove)
        {
            //dashSilence.enabled = true;
            dashAbilityInfo.silenceIcon();
        }

        // lerpAmount = Mathf.Max(lerpAmount - zoomSpeed * Time.deltaTime, 0f);
        if (dashAbilityInfo.isSpellReady())
        {
            //litDashIconBG.enabled = true;
            //litDashIcon.enabled = true;
            if (Input.GetMouseButtonDown(1) && !isDashing)
            {
                //litDashIconBG.enabled = false;
                //litDashIcon.enabled = false;
                isDashing = true;
                Debug.Log("dashing. ");
                // moveAmount += new Vector3(transform.position.x, 0, transform.position.z + 30);
                // moveAmount += new Vector3(transform.position.x + 30, 0, transform.position.z + 30);
                dashAbilityInfo.PutOnCoolDown();
                rb.AddForce(transform.forward * 15, ForceMode.Impulse);
                //Invoke(nameof(delayedDashForce), 0.25f); //add force to "maintain momentum.....?
            }
        }
        isDashing = false;

        // dash cd icon
        dashIconBG.fillAmount = (dashAbilityInfo.cdTime - dashAbilityInfo.currentCD) / dashAbilityInfo.cdTime;
        if (dashIconBG.fillAmount >= 0.23f)
        {
            dashIcon.fillAmount = (dashAbilityInfo.cdTime - dashAbilityInfo.currentCD) / dashAbilityInfo.cdTime;
        }

        //if (Input.GetMouseButton(1))
        //{
        //    //StartCoroutine(dashZoom());
        //    lerpAmount = Math.Min(lerpAmount + zoomSpeed * Time.deltaTime, 1f); // limit value to below 1
        //}
        //else
        //{
        //    lerpAmount = Mathf.Max(lerpAmount - zoomSpeed * Time.deltaTime, 0f);
        //}
        //// zoomed in effect to make dashing feel more dramatic
        //normal.weight = Math.Clamp(1 - lerpAmount, minNormalAmount, 1);
        //zoomed.weight = Math.Clamp(lerpAmount, 0, maxZoomedAmount);

        //POV.fieldOfView = Mathf.Lerp(normalFOV, zoomedFOV, lerpAmount);

        // Invoke(nameof(delayedTime), 0.01f);
        // rb.velocity = Vector3.zero;
        // rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
        //transform.position += new Vector3(dashForce * Time.deltaTime, 0.1f, 0f);
    }

    IEnumerator dashZoom()
    {
        lerpAmount = Math.Min(lerpAmount + zoomSpeed * Time.deltaTime, 1f); // limit value to below 1
        yield return new WaitForSeconds(1f);
    }

    void delayedDashForce()
    {
        rb.AddForce(transform.forward * 7, ForceMode.Impulse);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    void Move()
    {
        // WASD
        //Vector3 moveDir = new Vector3(horizInput, 0, vertInput).normalized;

        // sprint
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftControl) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);  
       
    }

    void Fly()
    {

        //disable sprint
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * flightSpeed, ref smoothMoveVelocity, smoothTime);
        //rb.useGravity = false;
        // rb.freezeRotation = false;
        
    }

    void DisableFlight()
    {
        rb.useGravity = true;
        // rb.freezeRotation = true;
        //TODO: set flight icon to greyed out; disable flight
    }

    // player view w/ mouse
    void Look()
    {
        // horizontal look
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        //vertical viewing
        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    // do physics stuff in fixed updates so our framerate doesn't affect it
    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;   
        }
        //if (isDashing)
        //{
        //    Dash();
        //}
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    #endregion movement

    void Die()
    {
        playerManager.Die();
    }

    #region dmg

    // runs on shooter's computer
    public void takeDamage(float damage)
    {
        PV.RPC(nameof(RPC_takeDamage), RpcTarget.All, damage);
        // PV.RPC(nameof(RPC_takeDamage), PV.Owner, damage);
    }

    [PunRPC] //sync dmg and prefabs over network
    void RPC_takeDamage(float damage, PhotonMessageInfo info)
    {
        //TODO: issue: dmg is calculated ON CLICK (most likely b/c of raycast; set the condition to on collision/ontrigger)
        // player health decreases after you hit them the 2nd time....?
        // player health is not updated every time they are hit w/ raycast (most likely due to latency....?) if you spam click
        // if you dont spam click everything is registered, might be due to the number of stuff it sends per second under RPC calls
        // LIMIT ATK SPEED TO SAME RATE; change projectile to another spell w/ low cd 

        // DO NOT ADD; method will only get executed on local computer (damage on particular player), not synced across network
        // if (!PV.IsMine) { return;  } // only sending dmg to owner of playerController

        Debug.Log("took dmg: " + damage);
        currentHealth -= damage;
        // decrease health bar for player pov
        healthBarImg.fillAmount = currentHealth / maxHealth;
        // decrease hp for enemies
        worldHealthBarImg.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            playerManager.Find(info.Sender).getKill();
        }
    }

    #endregion dmg

    [PunRPC]
    public void Fire(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = (float)(PhotonNetwork.Time - info.SentServerTime);
        GameObject bullet;

        /** Use this if you want to fire one bullet at a time **/
        bullet = Instantiate(BulletPrefab, position, Quaternion.identity) as GameObject;
        bullet.GetComponent<Projectile>().InitializeBullet(PV.Owner, (rotation * Vector3.forward), Mathf.Abs(lag));

    }

}
