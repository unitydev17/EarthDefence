using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CommonShipController : MonoBehaviour
{

	private const string PLAYER_TAG = "Player";
	private const string ENEMY_BULLET_TAG = "EnemyBullet";
	private const string PLAYER_BULLET_TAG = "PlayerBullet";

	private const float BULLET_SPEED = 200f;
	private const float BULLET_LIFETIME_SEC = 3f;
	private const float ENEMY_BULLET_DAMAGE = 10f;
	private const float PLAYER_BULLET_DAMAGE = 20f;
	private const float EXPLOSION_MAX_DISTANCE = 100f;
	private const float EXPLOSION_FORCE = 300f;

	public const float MAX_VELOCITY = 2500f;
	protected const float SHIP_ACCELERATION = 80f;

	public GameObject explosionPrefab;
	public GameObject bulletPrefab;
	public GameObject shotVFXPrefab;

	protected Rigidbody rigidBody;

	protected GameObject player;
	protected float health;


	public GameObject GetPlayer() {
		return player;
	}


	protected virtual void Start()
	{
		health = 100f;
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.useGravity = false;
		if (this is EnemyAIOld) {
			player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
		} else {
			player = transform.gameObject;
		}
		GameController.eventBus += ProcessCommand;
	}


	public virtual void ProcessCommand(string command, object param)
	{
		if (GameController.EXPLOSION_IMPACT_COMMAND == command) {
			ExplosionImpact(param);
		}
	}


	void ExplosionImpact(object param)
	{
		Vector3 expCenter = (Vector3)param;
		float distance = Vector3.Distance(expCenter, transform.position);
		distance = Mathf.Clamp(distance, 0, EXPLOSION_MAX_DISTANCE);
		Vector3 direction = (transform.position - expCenter).normalized;
		Vector3 expForce = EXPLOSION_FORCE * direction * (1 - distance / EXPLOSION_MAX_DISTANCE);
		rigidBody.AddForce(expForce);
		rigidBody.AddTorque(expForce);
	}


	public void Fire(Vector3 leftGunPosition, Vector3 rightGunPosition)
	{
		FireLazer(leftGunPosition);
		FireLazer(rightGunPosition);
	}


	void FireLazer(Vector3 position)
	{
		Vector3 gunPositionAbsolute = transform.TransformPoint(position);
		GameObject bullet = Instantiate(bulletPrefab, gunPositionAbsolute, Quaternion.identity);
		bullet.transform.parent = GameController.root.transform;
		Quaternion bulletRotation = Quaternion.FromToRotation(bullet.transform.up, transform.forward);
		bullet.transform.localRotation = bulletRotation;
		bullet.GetComponent<Rigidbody>().velocity = transform.forward * BULLET_SPEED + rigidBody.velocity;
		Destroy(bullet, BULLET_LIFETIME_SEC);
	}


	private void OnTriggerEnter(Collider other)
	{
		if (gameObject.CompareTag(PLAYER_TAG)) {
			if (ENEMY_BULLET_TAG == other.gameObject.tag) {
				//health -= ENEMY_BULLET_DAMAGE;
				SoundController.instance.ShipShoted();
				ShotVFX(other);
				if (health <= 0) {
					ExplodeShip();
				}
			}
		} else {
			// enemy was shooted
			if (PLAYER_BULLET_TAG == other.gameObject.tag) {
				health -= PLAYER_BULLET_DAMAGE;
			}
			if (health <= 0) {
				ExplodeShip();
			}
		}

		Destroy(other.gameObject);
	}


	void ShotVFX(Collider other) {
		GameObject vfx = Instantiate(shotVFXPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
		ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
		ps.Play();
		Destroy(vfx, ps.main.duration);

		//shake camera

	}


	void ExplosionSFX()
	{
		var distance = Vector3.Distance(transform.position, player.transform.position);
		SoundController.instance.EnemyExplosion(distance);
	}


	protected virtual void ExplodeShip()
	{
		ExplosionSFX();
		gameObject.GetComponent<Collider>().enabled = false;
		gameObject.GetComponent<Renderer>().enabled = false;
		GameObject explosionParent = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
		var particleSystem = explosionParent.GetComponent<ParticleSystem>();
		particleSystem.Play();

		Light light = explosionParent.GetComponentInChildren<Light>();
		float duration = particleSystem.main.duration;
		StartCoroutine(FadeLight(light, duration));
		Destroy(explosionParent, duration);
		Destroy(gameObject, duration);

		RemoveItemFromParent ();

		GameController.eventBus -= ProcessCommand;
		GameController.ExplosionImpact(transform.position);
	}

	// Stub. Could be implemented in ancestor classes.
	protected virtual void RemoveItemFromParent() {
	}


	IEnumerator FadeLight(Light light, float fadeDuration)
	{
		float startIntensity = light.intensity;
		for (float t = 0; t < 1.0f; t += Time.deltaTime / fadeDuration) {
			light.intensity = startIntensity * (1 - t);
			yield return null;
		}
	}

	protected void ApplyMoveRestrictions() {
		RestrictMaxVelocity();
	}

	void RestrictMaxVelocity() {
		if (rigidBody.velocity.magnitude > MAX_VELOCITY) {
			rigidBody.velocity = rigidBody.velocity.normalized * MAX_VELOCITY;
		}
	}


	#region DEBUG

	protected void DebugPath(LinkedList<Vector3> points) {
		if (points.Count == 0) {
			return;
		}

		IEnumerator enumerator = points.GetEnumerator();
		enumerator.MoveNext();
		Vector3 first = (Vector3)enumerator.Current;
		while (enumerator.MoveNext()) {
			Vector3 second = (Vector3)enumerator.Current;
			Debug.DrawLine(first, second);
		}
	}
	#endregion
}
