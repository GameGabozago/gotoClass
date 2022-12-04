using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool IsMelee;
    public bool IsRock;
    
    void OnCollisionEnter(Collision collision)
    {
        if(!IsRock && collision.gameObject.tag == "Floor"){
            Destroy(gameObject, 3);
        
        }
        else if (!IsRock && collision.gameObject.tag == "Wall"){
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
