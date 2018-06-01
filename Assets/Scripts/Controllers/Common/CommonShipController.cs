using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CommonShipController : MonoBehaviour
{

	public const string PLAYER_TAG = "Player";
	public const string PLAYER_TEAM_TAG = "PlayerTeam";
	public const string BULLET_TAG = "Bullet";
	public const string ENEMY_TAG = "Enemy";

	private const float BULLET_SPEED = 200f;
	private const float BULLET_LIFETIME_SEC = 3f;
	private const float BULLET_DAMAGE = 10f;
	private const float EXPLOSION_MAX_DISTANCE = 100f;
	private const float EXPLOSION_FORCE = 300f;

	public const float MAX_VELOCITY = 2500f;
	protected const float SHIP_ACCELERATION = 80f;

	public GameObject shotVFXPrefab;

	protected Rigidbody rigidBody;

	protected GameObject player;
	protected float health;

	private Collider _collider;
	private Renderer _renderer;



	public GameObject GetPlayer() {
		return player;
	}


	void Awake() {
		_collider = gameObject.GetComponent<Collider>();
		_renderer = gameObject.GetComponent<Renderer>();
	}


	public void Init() {
		_collider.enabled = true;
		_renderer.enabled = true;
	}


	protected virtual void Start()
	{
		health = 100f;
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.useGravity = false;

		if (tag == PLAYER_TAG) {
			player = transform.gameObject;
		} else {
			player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
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
		//rigidBody.AddTorque(expForce);
	}


	public void Fire(Vector3 leftGunPosition, Vector3 rightGunPosition)
	{
		FireLazer(leftGunPosition);
		FireLazer(rightGunPosition);
	}


	void FireLazer(Vector3 position)
	{
		Vector3 gunPositionAbsolute = transform.TransformPoint(position);

		bool isPlayer = this is PlayerController;
		GameObject bullet = isPlayer ? Pools.Instance.GetPlayerBullet (gunPositionAbsolute) : Pools.Instance.GetEnemyBullet(gunPositionAbsolute);

		bullet.transform.parent = GameController.root.transform;
		Quaternion bulletRotation = Quaternion.FromToRotation(bullet.transform.up, transform.forward);
		bullet.transform.localRotation = bulletRotation;
		bullet.GetComponent<Rigidbody>().velocity = transform.forward * BULLET_SPEED + rigidBody.velocity;
		StartCoroutine (DelayedDestroy(bullet, BULLET_LIFETIME_SEC));
	}



	IEnumerator DelayedDestroy(GameObject obj, float delay) {
		WaitForSeconds wait = new WaitForSeconds (delay);
		yield return wait;
		obj.SetActive (false);
	}


	private void OnTriggerEnter(Collider other)
	{
		// Player shooted
		if (gameObject.CompareTag(PLAYER_TAG)) {
			if (other.gameObject.CompareTag(BULLET_TAG)) {
				//health -= ENEMY_BULLET_DAMAGE;
				SoundController.instance.ShipShoted();
				ShotVFX(other);
				if (health <= 0) {
					ExplodeShip();
				}
			}
		} else {
			// player teammates shooted
			// enemy was shooted
			if (other.gameObject.CompareTag(BULLET_TAG)) {
				health -= BULLET_DAMAGE;
			}

			if (health <= 0) {
				ExplodeShip();
			}
		}

		other.gameObject.SetActive (false);
	}


	void ShotVFX(Collider other) {
		GameObject vfx = Instantiate(shotVFXPrefab, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
		vfx.transform.parent = GameController.root.transform;
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

		_collider.enabled = false;
		_renderer.enabled = false;
		GameObject explosionParent = Pools.Instance.GetExplosion (transform.position);
		explosionParent.transform.parent = GameController.root.transform;
		var particleSystem = explosionParent.GetComponent<ParticleSystem>();
		particleSystem.Play();

		Light light = explosionParent.GetComponentInChildren<Light>();
		float duration = particleSystem.main.duration;
		StartCoroutine(FadeLight(light, duration));
			
		StartCoroutine(DelayedDestroy(explosionParent, duration));
		StartCoroutine(DelayedDestroy(gameObject, duration));

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
