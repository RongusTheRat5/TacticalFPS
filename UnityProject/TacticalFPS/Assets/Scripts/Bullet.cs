using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            Destroy(collision.gameObject);
        }
            Destroy(this.gameObject);
    }
}
