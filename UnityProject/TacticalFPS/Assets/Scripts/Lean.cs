using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lean : MonoBehaviour
{
    public float leanValue;
    public float leanSpeed;

    [HideInInspector] public int lean = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float floatRotation = -leanValue * lean;

        Vector3 rotationd = new Vector3(0f, 0f, floatRotation);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(rotationd), Time.deltaTime * leanSpeed);



        if(Input.GetKeyDown(KeyCode.Q))
        {
            if (lean == -1) lean = 0;
            else lean = -1;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (lean == 1) lean = 0;
            else lean = 1;
        }
    }
}
