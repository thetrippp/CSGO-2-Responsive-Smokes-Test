using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WithCode.Projects.CSGO2ResponsiveSmokes;

public class PlayerThrow : MonoBehaviour
{

    public Transform spawnPoint;
    public GameObject grenade;
    public float throwSpeed;

    GameObject instance;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            instance = Instantiate(grenade, spawnPoint.position, Quaternion.identity);
            instance.transform.parent = spawnPoint;
        }
        if (Input.GetMouseButton(0))
        {
            instance.transform.position = spawnPoint.position;
        }
        if (Input.GetMouseButtonUp(0))
        {
            instance.transform.GetComponent<Rigidbody>().velocity = transform.forward * throwSpeed;
            instance.transform.GetComponent<Rigidbody>().isKinematic = false;
            instance.transform.GetComponent<ResponsiveSmokesV2>().grenadeState = ResponsiveSmokesV2.GrenadeState.active;
            instance.transform.parent = null;
            instance = null;
        }
        if (Input.GetMouseButtonDown(1))
        {
            instance = Instantiate(grenade, spawnPoint.position, Quaternion.identity);
            instance.transform.parent = spawnPoint;
        }
        if (Input.GetMouseButton(1))
        {
            instance.transform.position = spawnPoint.position;
        }
        if (Input.GetMouseButtonUp(1))
        {
            instance.transform.GetComponent<Rigidbody>().velocity = transform.forward * throwSpeed * 0.25f;
            instance.transform.GetComponent<Rigidbody>().isKinematic = false;
            instance.transform.GetComponent<ResponsiveSmokesV2>().grenadeState = ResponsiveSmokesV2.GrenadeState.active;
            instance.transform.parent = null;
            instance = null;
        }
    }
}
