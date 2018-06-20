using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireContAi2 : MonoBehaviour
{

    public GameObject bulletPrefab;
    public GameObject bulletSpawn;

    float bulletTime = 6.0f;

    private bool canShoot = true;

    // Update is called once per frame
    void Update()
    {

        if (!canShoot) return;

        if (Input.GetKeyDown("x"))
        {
            canShoot = false;
            CreateBullet();
            Invoke("Reload", 2f);
        }
    }

    public void tryShot()
    {
        if (!canShoot) return;

        canShoot = false;
        CreateBullet();
        Invoke("Reload", 2f);
        

    }


    private void Reload()
    {
        canShoot = true;
    }

    void CreateBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * 50;
        Destroy(bullet, bulletTime);

    }

}
