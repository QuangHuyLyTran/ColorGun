using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreateHealth : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        SetActive();
    }
    private void Update()
    {
        Invoke("SetActive", 10f);
    }
    void SetActive()
    {
        if (gameObject.active == false)
            gameObject.SetActive(true);
    }
}
