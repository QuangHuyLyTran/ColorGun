using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatHealth : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Health"))
        {
            
            other.gameObject.SetActive(false);
            
        }
    }
}
