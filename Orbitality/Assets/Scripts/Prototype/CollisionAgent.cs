using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionAgent : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        Debug.Log("OnCollisionEnter2D");
    }

    void OnTriggerStay2D(Collider2D collider2D)
    {
        Debug.Log("OnTriggerStay2D");
    }

    void OnCollisionExit2D(Collision2D collision2D)
    {
        Debug.Log("OnCollisionExit2D");
    }


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
    void OnCollisionStay(Collision collision)
    {
        Debug.Log("OnCollisionStay");

        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log("OnCollisionExit");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit");
    }
}
