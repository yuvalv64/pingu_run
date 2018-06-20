using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCont : MonoBehaviour
{

    public GameObject bulletPrefab;
    public GameObject bulletSpawn;
    public GameObject icon;

    float bulletTime = 6.0f;

    private bool canShoot = true;

    private void Start()
    {

        icon.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

        if (!canShoot) return;

        if (Input.GetKeyDown("x"))
        {
            canShoot = false;
            CreateBullet();
            Invoke("Reload", 2f);
            icon.SetActive(false);
        }
    }


    private void Reload()
    {
        canShoot = true;
        icon.SetActive(true);
    }

    void CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * 30;
        Destroy(bullet, bulletTime);

    }


}
