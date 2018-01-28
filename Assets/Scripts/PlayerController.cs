 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Rigidbody player;
    public float speed;
    public float bulletSpeed;
    public GameObject bullet;
    public GameObject directionPoint;
    public Transform bulletSpawn;
    private Vector3 direction;
    private float playerRotate;

    private bool pressControllerW;
    private bool pressControllerA;
    private bool pressControllerD;

    private void Start()
    {
        playerRotate = player.transform.eulerAngles.y;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock))
        {
            direction = gameObject.transform.position - directionPoint.transform.position;
            direction.Normalize();
            direction.y = 0;
            GameObject bulletClone = Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
            Rigidbody bulletRig = bulletClone.GetComponent<Rigidbody>();
            bulletRig.velocity = -direction * bulletSpeed;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            pressControllerW = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            pressControllerW = false;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            pressControllerA = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            pressControllerA = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            pressControllerD = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            pressControllerD = false;
        }
        if (pressControllerW) 
        {
            direction = gameObject.transform.position - directionPoint.transform.position;
            direction.Normalize();
            direction.y = 0;
            player.velocity = -direction * speed;
        }
        if (pressControllerD)
        {
            player.transform.eulerAngles = new Vector3(0, playerRotate + 2, 0);
            playerRotate += 2;
        }
        if (pressControllerA)
        {
            player.transform.eulerAngles = new Vector3(0, playerRotate - 2, 0);
            playerRotate -= 2;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet2")
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
