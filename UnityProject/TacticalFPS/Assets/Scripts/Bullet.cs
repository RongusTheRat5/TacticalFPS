using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bullethole;

    private void Update()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 4f))
        {
            GameObject newBulletHole = Instantiate(bullethole, hit.point, Quaternion.identity);
            newBulletHole.transform.LookAt(hit.point + 0.001f * hit.normal);
            Destroy(newBulletHole, 3f);
            if (hit.transform.CompareTag("Target"))
            {
                Destroy(hit.transform.gameObject);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }
}
