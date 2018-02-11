using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CommonShipController : MonoBehaviour {

	private const string PLAYER_TAG = "Player";
	private const string ENEMY_BULLET_TAG = "EnemyBullet";
	private const string PLAYER_BULLET_TAG = "PlayerBullet";
	private const float BULLET_SPEED = 200f;
	private const float ENEMY_BULLET_DAMAGE = 1f;
	private const float PLAYER_BULLET_DAMAGE = 20f;

	public GameObject enemyExplosion;
	public GameObject bulletPrefab;
	protected Rigidbody rigidBody;

	protected GameObject player;
	private float health;


	protected virtual void Start() {
		health = 100f;
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.useGravity = false;
		if (this is EnemyAI) {
			player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
		}
	}

	protected void Fire(Vector3 leftGunPosition, Vector3 rightGunPosition)
	{
		FireLazer(leftGunPosition);
		FireLazer(rightGunPosition);
	}


	void FireLazer(Vector3 position) {
		Vector3 gunPositionAbsolute = transform.TransformPoint(position);
		GameObject bullet = Instantiate( bulletPrefab, gunPositionAbsolute, Quaternion.identity);
		Quaternion bulletRotation = Quaternion.FromToRotation(bullet.transform.up, transform.forward);
		bullet.transform.localRotation = bulletRotation;
		bullet.GetComponent<Rigidbody>().velocity = transform.forward * BULLET_SPEED + rigidBody.velocity;
	}


	private void OnTriggerEnter(Collider other)
	{
		if (gameObject.CompareTag(PLAYER_TAG)) {
			if (ENEMY_BULLET_TAG == other.gameObject.tag) {
				health -= ENEMY_BULLET_DAMAGE;
				SoundController.instance.ShipShoted();
			}
		} else {
			// enemy was shooted
			if (PLAYER_BULLET_TAG == other.gameObject.tag) {
				health -= PLAYER_BULLET_DAMAGE;
			}
			if (health <= 0) {
				ExplodeEnemy();
				var distance = Vector3.Distance(transform.position, player.transform.position);
				SoundController.instance.EnemyExplosion(distance);
			}
		}

		Destroy(other.gameObject);
	}


	void ExplodeEnemy() {
		gameObject.GetComponent<Collider>().enabled = false;
		gameObject.GetComponent<Renderer>().enabled = false;
		GameObject explosionParent = Instantiate(enemyExplosion, transform.position, Quaternion.identity);
		var particleSystem = explosionParent.GetComponent<ParticleSystem>();
		particleSystem.Play();

		Light light = explosionParent.GetComponentInChildren<Light>();
		float duration = particleSystem.main.duration;
		StartCoroutine(FadeLight(light, duration));
		Destroy(explosionParent, duration);
		Destroy(gameObject, duration);
	}


	IEnumerator FadeLight(Light light, float fadeDuration) {
		float startIntensity = light.intensity;
		for (float t = 0; t < 1.0f; t += Time.deltaTime / fadeDuration) {
			light.intensity = startIntensity * (1 - t);
			yield return null;
		}
	}
}
