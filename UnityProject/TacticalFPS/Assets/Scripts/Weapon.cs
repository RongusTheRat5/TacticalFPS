using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Vector3 adsPos;

    public Transform muzzlePos;
    public GameObject bullet;
    public GameObject flash;
    public float force;
    public Transform cam;
    public CamController cameraController;
    public LayerMask mask;
    private Transform holder;
    public float kickback;
    public int magMaxCap;
    public int heatDecay;

    public float bloom;
    public float recoil;
    public bool fullAuto;
    public int bulletsInShot;

    public float aimFOV;
    public float normalFOV;

    public float strength;

    private int magCap;

    private float heat = 0;

    public bool reloading;
    public bool shooting;

    private Sway sway;

    private Animator animator;

    public Camera weaponCam;

    public Vector2[] recoilValues;
    private Vector2 lastRecoil;

    public AudioSource audioSource;
    public float timeBetweenShot;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        holder = transform.parent.parent;
        sway = transform.root.GetComponent<Sway>();
        magCap = magMaxCap;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateADS();

        holder.localRotation = Quaternion.Slerp(holder.localRotation, Quaternion.identity, Time.deltaTime * 8f);
        holder.localPosition = Vector3.Lerp(holder.localPosition, Vector3.zero, Time.deltaTime * 8f);

        if (timer <= timeBetweenShot)
        {
            cameraController.horizontalRotation += (lastRecoil.x / (timeBetweenShot)) * Time.deltaTime;
            cameraController.verticalRotation += (lastRecoil.y / (timeBetweenShot)) * Time.deltaTime;
        }

        if (reloading) { sway.aim = false; return; }

        if (timer > timeBetweenShot + 0.1f) shooting = false;

        if (!shooting)
        {
            if (heat > 0)
            {
                heat -= Time.deltaTime * heatDecay;
            }
            else heat = 0;
        }

        if (Input.GetKeyDown(KeyCode.R) && magMaxCap != magCap) {StartCoroutine("Reload"); return; }

        timer += Time.deltaTime;

        if (timer < timeBetweenShot) return;

        if (magCap <= 0) return;

        if(fullAuto ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0))
        {
            magCap--;
            timer = 0;

                Shoot();
        }
    }

    IEnumerator Reload()
    {
        heat = 0;
        lastRecoil = Vector2.zero;
        reloading = true;
        animator.SetTrigger("Reload");
        yield return new WaitForSeconds(2f);
        magCap = magMaxCap;
        reloading = false;  
    }

    private void UpdateADS()
    {
        Vector3 targetPosition = new Vector3();
        if (sway.aim)
        {
            targetPosition = adsPos;
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, aimFOV, Time.deltaTime * 8f);
        }
        else
        {
            targetPosition = new Vector3(0, -0.1f, 0f);
            weaponCam.fieldOfView = Mathf.Lerp(weaponCam.fieldOfView, normalFOV, Time.deltaTime * 8f);
        }
        transform.parent.localPosition = Vector3.Lerp(transform.parent.localPosition, targetPosition, Time.deltaTime * /*ADS SPEED*/ 8f);
    }

    private void Shoot()
    {
        shooting = true;
        for (int i = 0; i < bulletsInShot; i++)
        {
            Vector3 direction;
            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, 100f, mask))
            {
                direction = hit.point - muzzlePos.position;
                direction.Normalize();
            }
            else
            {
                direction = muzzlePos.forward;
            }
            direction += Random.insideUnitSphere * bloom * 0.005f;
            GameObject newBullet = Instantiate(bullet, muzzlePos.position, muzzlePos.rotation);
            newBullet.GetComponent<Rigidbody>().AddForce(force * direction, ForceMode.Impulse);
            Destroy(newBullet, 10f);
        }
        GameObject newFlash = Instantiate(flash, muzzlePos.position, muzzlePos.rotation);
        Destroy(newFlash, 0.05f);

        Vector3 l_recoil = new Vector3(
            Random.Range(-1, -0.5f) * recoil,
            Random.Range(-0.2f, 0.2f) * recoil,
            Random.Range(-0.2f, 0.2f) * recoil
            );
        if (sway.aim) l_recoil /= 5;
        holder.localRotation *= Quaternion.Euler(l_recoil);
        holder.localPosition += sway.aim ? new Vector3(0, 0, 1) * -0.02f * kickback : new Vector3(0, 0, 1) * -0.1f * kickback;
        holder.localPosition += sway.aim ? holder.right * Random.Range(-0.005f, 0.005f) : holder.right * Random.Range(-0.03f, 0.03f);

        audioSource.pitch = Random.Range(-0.05f, 0.05f) + 1;
        audioSource.volume = Random.Range(-0.05f, 0.05f) + 0.7f;
        audioSource.Play();

        cameraController.Shake(strength);

        //recoil
        lastRecoil = recoilValues[Mathf.RoundToInt(heat)] * 2;

        heat++;
    }
}
