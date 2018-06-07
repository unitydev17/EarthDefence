using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[RequireComponent(typeof(Rigidbody))]
public class CommonShipController : MonoBehaviour
{

	public const string PLAYER_TAG = "Player";
	public const string PLAYER_TEAM_TAG = "PlayerTeam";
	public const string BULLET_TAG = "Bullet";
	public const string ENEMY_TAG = "Enemy";

	private const float BULLET_SPEED = 200f;
	private const float BULLET_LIFETIME_SEC = 3f;
	private const float BULLET_DAMAGE = 5f;
	private const float PLAYER_BULLET_DAMAGE = 2f;
	private const float EXPLOSION_MAX_DISTANCE = 100f;
	private const float EXPLOSION_FORCE = 300f;

	public const float MAX_VELOCITY = 20.5f;
	protected const float SHIP_ACCELERATION = 40f;
	protected const float SHIP_DECELERATION = 20f;

	public GameObject shotVFXPrefab;

	protected Rigidbody rigidBody;

	protected GameObject player;

	private float health;
	private float Health {
		get { return health; }
		set {
			health = value;
			UpdateHealthBar (health);
		}
	}

	protected virtual void UpdateHealthBar(float health) {}

	protected Collider _collider;
	protected Renderer _renderer;



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
		Health = 100f;
		rigidBody = GetComponent<Rigidbody>();
		rigidBody.useGravity = false;

		if (tag == PLAYER_TAG) {
			player = transform.gameObject;
		} else {
			player = GameObject.FindGameObjectWithTag(PLAYER_TAG);
		}
		GameController.eventBus += ProcessCommand;
		MasterAI.masterAIEvents += ProcessCommand;
	}


	protected virtual void ProcessCommand(string command, object param)
	{
		if (GameController.EXPLOSION_IMPACT_COMMAND == command) {
			ExplosionImpact (param);
		}
	}


	void ExplosionImpact(object param)
	{
		try {
			if (transform != null) {
				Vector3 expCenter = (Vector3)param;
				float distance = Vector3.Distance(expCenter, transform.position);
				distance = Mathf.Clamp(distance, 0, EXPLOSION_MAX_DISTANCE);
				Vector3 direction = (transform.position - expCenter).normalized;
				Vector3 expForce = EXPLOSION_FORCE * direction * (1 - distance / EXPLOSION_MAX_DISTANCE);
				rigidBody.AddForce(expForce);
				//rigidBody.AddTorque(expForce);
			}
		} catch (Exception e) {
			Debug.Log (e);
		}
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
		if (player) {
			float distance = Vector3.Distance (transform.position, player.transform.position);
			SoundController.instance.BotFire (distance);
		}
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
				Health -= PLAYER_BULLET_DAMAGE;
				SoundController.instance.ShipShoted();
				ShotVFX(other);
				if (Health <= 0) {
					ExplodeShip();
				}
			}
		} else {
			// player teammates shooted
			// enemy was shooted
			if (other.gameObject.CompareTag(BULLET_TAG)) {
				Health -= BULLET_DAMAGE;
				ShotVFX(other);
			}

			if (Health <= 0) {
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
		SoundController.instance.Explosion(distance);
	}


	protected virtual void ExplodeShip()
	{
		ExplosionSFX();

		_collider.enabled = false;
		_renderer.enabled = false;
		foreach (Transform child in gameObject.transform) {
			TrailRenderer tr = child.GetComponent<TrailRenderer> ();
			if (tr != null) {
				tr.Clear ();
			}
		}

		GameObject explosionParent = Pools.Instance.GetExplosion (transform.position);
		explosionParent.transform.parent = GameController.root.transform;
		var particleSystem = explosionParent.GetComponent<ParticleSystem>();
		particleSystem.Play();

		Light light = explosionParent.GetComponentInChildren<Light>();
		float duration = particleSystem.main.duration;
		StartCoroutine(FadeLight(light, duration));
			
		StartCoroutine(DelayedDestroy(explosionParent, duration));
		DestroyShip (duration);

		RemoveItemFromParent ();

		GameController.ExplosionImpact(transform.position);
	}

	protected virtual void DestroyShip(float duration) {
		StartCoroutine(DelayedDestroy(gameObject, duration));
	}
		
	protected virtual void RemoveItemFromParent() {
		GameController.eventBus -= ProcessCommand;
		MasterAI.masterAIEvents -= ProcessCommand;
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
