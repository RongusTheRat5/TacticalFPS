using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject bullethole;

    private void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 4f))
        {

            if (hit.transform.root.CompareTag("Target"))
            {
                hit.transform.root.GetComponent<Animator>().enabled = false;
                setRbRecursively(hit.transform.root);
                hit.transform.root.GetChild(7).GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * 50f, hit.point, ForceMode.Impulse);
            }
            else
            {
                GameObject newBulletHole = Instantiate(bullethole, hit.point, Quaternion.identity);
                newBulletHole.transform.LookAt(hit.point + 0.001f * hit.normal);
                Destroy(newBulletHole, 3f);
            }
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    private void setRbRecursively(Transform t)
    {
        if(t.GetComponent<Rigidbody>())
        {
            t.GetComponent<Rigidbody>().isKinematic = false;
        }
        foreach (Transform child in t.transform)
        {
            setRbRecursively(child);
        }
    }
}
