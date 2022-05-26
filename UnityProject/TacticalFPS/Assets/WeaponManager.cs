using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] guns;
    private int currentIndex = 0;
    private Weapon currentGun;

    private void Start()
    {
        Equip();
    }

    // Start is called before the first frame update
    void Update()
    {
        if (currentGun.reloading || currentGun.shooting) return;
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentIndex++;
            if (currentIndex >= guns.Length) currentIndex = 0;
            Equip();
        }
    }

    private void Equip()
    {
        for(int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(i == currentIndex);
        }
        currentGun = guns[currentIndex].GetComponent<Weapon>();
    }
}
