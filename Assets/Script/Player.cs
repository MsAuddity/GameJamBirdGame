﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Variable declaration
    public GameObject player, missionComplete, missionFail, targetPhone;
    public Transform beakBindPoint, currentEavesdrop;
    public float listenModifier, eavesdropLevel, suspicion;
    public bool eavesdropping;

    private Transform playerT;
    private ObjectHandler obj = new ObjectHandler();
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        #region Variable Definition
        //obj = new ObjectHandler();
        if (!player)
        {
            player = this.gameObject;
        }

        suspicion = 0f;
        int whilestop = 0;
        while (!targetPhone && whilestop <10)
        {
            targetPhone = TargetDevice();
            if (targetPhone) Debug.Log("Got Device");
        }


        beakBindPoint = transform.Find("BeakBindPoint").GetComponent<Transform>();
        playerT = player.GetComponent<Transform>();
        eavesdropLevel = 0f;
        eavesdropping = false;
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        // UpdateEavesdropLevel();
        if (Input.GetButton("dropKeybind")) obj.DropObject(beakBindPoint);

    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.CompareTag("Phone"))
        {
            Grab(other.gameObject.GetComponent<Transform>());
        }
        */
        if (other.CompareTag("Interactive") && other.gameObject.GetComponent<ObjectInfo>().canGrab)
        {
            obj.GrabObject(other.gameObject, beakBindPoint);
        }
        if (other.CompareTag("NPC"))
        {
            //TODO: spike Suspicion to super-high, not 100
        }
        if ((other.gameObject.name == "ListenField") && !eavesdropping)
        {
            eavesdropping = true;
            var target = other.gameObject.GetComponent<Transform>().parent.GetComponent<Transform>();
            currentEavesdrop = target;
            Debug.Log(currentEavesdrop, currentEavesdrop.gameObject);
            BroadcastMessage("EnableEavesdrop");
        }
        if (other.gameObject.name == "BirdNest")
        {
            obj.DropObject(beakBindPoint, other.gameObject);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "ListenField")
        {
            eavesdropping = false;
            BroadcastMessage("DisableEavesdrop");
        }
    }

    public GameObject TargetDevice()
    {
        var NPCs = new List<ObjectInfo>();

        foreach (Transform NPC in GameObject.Find("Humans").GetComponentInChildren<Transform>())
        {
            var info = NPC.gameObject.GetComponent<ObjectInfo>();
            if (info.objectType == "NPCContainer")
                NPCs.Add(info);
            else Debug.LogWarning(info + " may be missing an ObjectInfo component.", info);
        }

        ObjectInfo spyInfo = NPCs[Random.Range(0, NPCs.Count)];
        GameObject Spy = spyInfo.gameObject;

        for (int i = 0; i < spyInfo.tags.Length; i++)
        {
            if (spyInfo.tags[i] == "Innocent" || spyInfo.tags[i] == "Group")
            {
                spyInfo.tags[i] = "Spy";
                break;
            }
            else continue;
        }
        Debug.Log(Spy.ToString() + " is a spy!", Spy);
        GameObject targetDevice;
        foreach (ObjectInfo info in Spy.GetComponentsInChildren<ObjectInfo>())
        {

            if (info.objectType == "Phone")
            {
                Debug.Log("Got a phone to try on");
                targetDevice = info.gameObject;
                Debug.Log("Device set");
                info.objectType = "Intel";
                Debug.Log("Phone Tag Set, returning device");
                return targetDevice;
            }
            else continue;

        }
        Debug.LogError("Phone not found for selected spy, retrying.", Spy);
        return null;
    }



}
