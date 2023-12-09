using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

// indicator + spawn prefab
public class BasicSpell : Spell
{
    public Canvas canvas;
    public Image indicatorImg;
    Vector3 position;
    // [SerializeField] private Transform player;

    // ability2 input
    Vector3 posUp;
    [SerializeField] private float abilityRange;
    Camera POV;

    Vector3 newHitPos;

    bool canCast = false;
    public bool silenced = false;

    private void Awake()
    {

        // canvas = GetComponentInChildren<Canvas>();
        // indicatorImg = GetComponentInChildren<Image>();
        POV = GetComponentInChildren<Camera>();
        // indicatorImg = GetComponentInChildren<Image>();

        indicatorImg.enabled = false;

        if (keyBind == null) 
        { 
            keyBind = "e";
            Debug.LogWarning("Keybind not specified in BasicSpell. ");
        }

        abilityInfo.currentCD = 0;
        abilityInfo.currentActiveCD = 0;

        DontDestroyOnLoad(indicatorImg);

    }

    //private void Update()
    //{
    //    indicatorImg.enabled = false;
    //    if (Input.GetKey(KeyCode.E))
    //    {
    //        indicatorImg.enabled = true;
    //        showIndicator();
    //    }

    //    if (Input.GetKeyUp("e"))
    //    {
    //        spawnSpell();
    //    }
    //}

    public override void Activate()
    {
        // update
        indicatorImg.enabled = false;
        if (abilityInfo.isSpellReady())
        {
            if (Input.GetKey(keyBind))
            {
                indicatorImg.enabled = true;
                showIndicator();
            }

            if (Input.GetKeyUp(keyBind))
            {
                Debug.Log("using basic spell " + keyBind);
                spawnSpell();
                abilityInfo.PutOnActiveCoolDown();
                abilityInfo.PutOnCoolDown();
            }
        }
    }

    void spawnSpell()
    {
        // PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", spellPrefab.name), newHitPos, Quaternion.identity);
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", spellPrefab.name), newHitPos, Quaternion.LookRotation(newHitPos.normalized, Vector3.up) * spellPrefab.transform.rotation);
    }

    void showIndicator()
    {
        RaycastHit hit;
        Ray ray = POV.ScreenPointToRay(Input.mousePosition);

        // ability1 inputs
        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    position = new Vector3(hit.point.x, hit.point.y);
        //}

        // ability2
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject != this.gameObject)
            {
                posUp = new Vector3(hit.point.x, .1f, hit.point.z);
                position = hit.point;
                // position = hit.point;
            }
        }

        // ability1 canvas inputs
        //Quaternion transRot = Quaternion.LookRotation(position - player.transform.position);
        //canvas.transform.rotation = Quaternion.Lerp(transRot, canvas.transform.rotation, 0f);

        // ability2 canvas inputs
        var hitPosDir = (hit.point - transform.position).normalized;
        float distance = Vector3.Distance(hit.point, transform.position);
        distance = Mathf.Min(distance, abilityRange);

        newHitPos = transform.position + hitPosDir * distance;
        // offset y so indicator doesn't jitter
        newHitPos.y = newHitPos.y + 0.1f;

        canvas.transform.position = (newHitPos);

        //var hitPosDir = (position - transform.position).normalized;
        //float distance = Vector3.Distance(position, transform.position);
        //distance = Mathf.Min(distance, abilityRange);

        //var newHitPos = transform.position + hitPosDir * distance;
        //canvas.transform.position = (newHitPos);
    }

    // spawn object at ANY cursor location (3d space)
    // unused 
    void spawnSpellAnywhere()
    {
        Vector3 worldPt = Input.mousePosition;
        worldPt.z = 10f;
        // worldPt.z = Mathf.Abs(Camera.main.transform.position.z);
        Vector3 objPos = POV.ScreenToWorldPoint(worldPt);
        // objPos.z = 0f;
        Instantiate(spellPrefab, objPos, Quaternion.identity);
    }
}
