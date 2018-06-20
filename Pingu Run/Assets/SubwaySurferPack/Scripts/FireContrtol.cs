using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class FireContrtol : NetworkBehaviour {

    public GameObject bullutPrefab;
    public GameObject bulletSpawn;
    public GameObject NGM;

    float bulletTime = 6.0f;

    private bool canShoot = true;

    private void Start()
    {
        //icon.SetActive(true);

    }


    // Update is called once per frame
    void Update()
    {

        if (!isLocalPlayer) return;

        if (!canShoot) return;
            
        if (Input.GetKeyDown("x"))
        {
            canShoot = false;
            CmdShoot();
            Invoke("Reload", 2f);
            NetGameManager.singleton.Fire();
        }
    }

    private void Reload()
    {
        canShoot = true;
        NetGameManager.singleton.reload();
    }

    void CreateBullet()
    {
        GameObject bullet = Instantiate(bullutPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
        bullet.GetComponent<Rigidbody>().velocity = bulletSpawn.transform.forward * 30;
        Destroy(bullet, bulletTime);
    }

    [ClientRpc]
    void RpcCreateBullet()
    {
        if (!isServer)
            CreateBullet();
    }

    [Command]
    void CmdShoot()
    {
        CreateBullet();
        RpcCreateBullet();

    }


}
