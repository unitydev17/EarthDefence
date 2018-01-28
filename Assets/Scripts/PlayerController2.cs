using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour {

    public Rigidbody player;
    public float speed;
    public float bulletSpeed;
    public GameObject bullet;
    public GameObject directionPoint;
    public Transform bulletSpawn;
    private Vector3 direction;
    private float playerRotate;

    private bool pressControllerUp;
    private bool pressControllerLeft;
    private bool pressControllerRight;

    private void Start()
    {
        playerRotate = player.transform.eulerAngles.y;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            direction = gameObject.transform.position - directionPoint.transform.position;
            direction.Normalize();
            direction.y = 0;
            GameObject bulletClone = Instantiate(bullet, bulletSpawn.position, Quaternion.identity);
            Rigidbody bulletRig = bulletClone.GetComponent<Rigidbody>();
            bulletRig.velocity = -direction * bulletSpeed;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            pressControllerUp = true;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            pressControllerUp = false;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pressControllerLeft = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            pressControllerLeft = false;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            pressControllerRight = true;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            pressControllerRight = false;
        }
        if (pressControllerUp)
        {
            direction = gameObject.transform.position - directionPoint.transform.position;
            direction.Normalize();
            direction.y = 0;
            player.velocity = -direction * speed;
        }
        if (pressControllerRight)
        {
            player.transform.eulerAngles = new Vector3(0, playerRotate + 2, 0);
            playerRotate += 2;
        }
        if (pressControllerLeft)
        {
            player.transform.eulerAngles = new Vector3(0, playerRotate - 2, 0);
            playerRotate -= 2;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
