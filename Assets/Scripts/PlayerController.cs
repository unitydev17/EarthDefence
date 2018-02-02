using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public const float BULLET_SPAWN_DISTANCE = 0.5f;

    public Rigidbody player;
    public float speed;
    public float bulletSpeed;
    public GameObject bullet;
    public Transform bulletSpawn;

	private float rotationY;


    private void Update()
    {
		if (Input.GetMouseButtonDown(0))
        {
			Vector3 bulletSpawnPosition = transform.position + transform.forward * BULLET_SPAWN_DISTANCE;
			GameObject bulletClone = Instantiate(bullet, bulletSpawnPosition, Quaternion.identity);
            Rigidbody bulletRig = bulletClone.GetComponent<Rigidbody>();
			bulletRig.velocity = transform.forward * bulletSpeed;
        }

		if (Input.GetKey(KeyCode.W))
        {
			player.GetComponent<Rigidbody>().AddForce(transform.forward * speed);
        }

		if (Input.GetKey(KeyCode.S))
		{
			player.GetComponent<Rigidbody>().AddForce(-transform.forward * speed);
		}


		if (Input.GetKey(KeyCode.D))
        {
			player.transform.RotateAround (transform.position, Vector3.up, 2);
        }

		if (Input.GetKey(KeyCode.A))
        {
			player.transform.RotateAround (transform.position, Vector3.up, -2);
		}

		HandleMouseDirections ();

    }

	private void HandleMouseDirections() {
		float dx = Input.GetAxis ("Mouse X") * 3;
		float dy = Input.GetAxis ("Mouse Y") * 3;
		float rotationX = transform.localEulerAngles.y + dx;
		rotationY += dy;
		transform.localEulerAngles = new Vector3 (-rotationY, rotationX, 0);
	}


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet2")
        {
			Debug.Log (Time.time + "Shot!");
            //Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
