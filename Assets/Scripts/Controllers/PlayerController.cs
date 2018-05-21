using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class PlayerController : CommonShipController
{
	public static event Action<bool> eventHandlers;
    // test

	public enum MoveState
	{
		Idle,
		Accelerate
	}


	private enum GunState
	{
		Idle,
		Fire
	}

	public const float BULLET_SPAWN_DISTANCE = 0.5f;
	private const float SHIP_STOPPED_SPEED = 1f;
	private const int CHAIN_FIRE_NUMBERS = 3;

	private Vector3 rightGunPosition = new Vector3(1.967f, 0.276f, 2f);
	private Vector3 leftGunPosition = new Vector3(-1.967f, 0.276f, 2f);
	private float rotationY;

	private float startChainTime;
	private int chainFireNumber;
	private GunState gunState = GunState.Idle;

	public GameObject centralEngine;
	public GameObject leftEngine;
	public GameObject rightEngine;

    public Image healthBar;

	private ParticleSystem centralPS;
	private ParticleSystem leftPS;
	private ParticleSystem rightPS;
    private ParticleSystem playerExplosion;

    private float enginesForce;
	private bool isAlive;


	void Start()
	{
		base.Start();
		centralPS = centralEngine.GetComponent<ParticleSystem>();
		leftPS = leftEngine.GetComponent<ParticleSystem>();
		rightPS = rightEngine.GetComponent<ParticleSystem>();
		enginesForce = 1;
		isAlive = true;
	}


	private void Update()
	{
		if (isAlive) {
			HandleInput();
			ProcessActions();
            if (healthBar.fillAmount == 0)
            {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
                Destroy(gameObject);
            }
		}
	}


	private void OnGUI() {
		GUI.TextArea(new Rect(0, 30, 100, 50), "Velocity: " + rigidBody.velocity.magnitude);
	}


	private void LateUpdate()
	{
		HandleMouseDirections();
	}


	private void ProcessActions()
	{
		if (GunState.Fire == gunState) {
				
			if (Time.time - startChainTime > 0.02f) {

				if (++chainFireNumber == CHAIN_FIRE_NUMBERS) {
					gunState = GunState.Idle;
					return;
				}

				startChainTime = Time.time;
				Fire(leftGunPosition, rightGunPosition);
				SoundController.instance.PlayerFire();
			}
		}
	}


	private void HandleInput()
	{
		if (Input.GetMouseButtonDown(0)) {
			gunState = GunState.Fire;
			chainFireNumber = 0;
		}

		// Forward
		ProcessMoveForward();


		// Brake
		if (Input.GetKey(KeyCode.S)) {
			Brake();
		}

		// Right incline
		if (Input.GetKey(KeyCode.D)) {
			transform.RotateAround(transform.position, transform.forward, -1f);
		}

		// Left incline
		if (Input.GetKey(KeyCode.A)) {
			transform.RotateAround(transform.position, transform.forward, 1f);
		}		
	}


	void ProcessMoveForward()
	{
		if (Input.GetKeyDown(KeyCode.W)) {
			eventHandlers.Invoke(true);
			StartEngines();

		} else if (Input.GetKeyUp(KeyCode.W)) {
			eventHandlers.Invoke(false);
			StopEngines();
		}

		if (Input.GetKey(KeyCode.W)) {
			MoveForward();
		}

		ApplyMoveRestrictions();
	}


	void MoveForward()
	{
		Vector3 forceForward = Vector3.Lerp(Vector3.zero, transform.forward, Time.deltaTime * SHIP_ACCELERATION);
		rigidBody.AddForce(forceForward, ForceMode.Impulse);
		EnginesForceUp();
	}


	void StartEngines()
	{
		StartParticle(centralPS);
		StartParticle(leftPS);
		StartParticle(rightPS);
	}


	void StartParticle(ParticleSystem ps)
	{
		ParticleSystem.MainModule main = ps.main;
		ps.Play();
	}


	void StopEngines()
	{
		enginesForce = 1;
		StopEngine(centralPS);
		StopEngine(leftPS);
		StopEngine(rightPS);
	}


	void StopEngine(ParticleSystem ps)
	{
		ParticleSystem.MainModule main = ps.main;
		main.simulationSpeed = enginesForce;
		ps.Stop();
	}


	void EnginesForceUp()
	{
		enginesForce += Time.deltaTime;
		EngineForceUp(centralPS);
		EngineForceUp(leftPS);
		EngineForceUp(rightPS);
	}


	void EngineForceUp(ParticleSystem ps)
	{
		ParticleSystem.MainModule main = ps.main;
		main.simulationSpeed = enginesForce;
	}


	void Brake()
	{
		if (Mathf.Abs(rigidBody.velocity.magnitude) > SHIP_STOPPED_SPEED) {
			Vector3 forceForward = Vector3.Lerp(Vector3.zero, rigidBody.velocity.normalized, Time.deltaTime * SHIP_ACCELERATION);
			rigidBody.AddForce(-forceForward, ForceMode.Impulse);
			return;
		}
		rigidBody.velocity = Vector3.zero;
	}


	private void HandleMouseDirections()
	{
		var crossHairPosition = CrossHairController.position;
		var direction = Camera.main.ScreenPointToRay(crossHairPosition).direction;
		transform.rotation = Quaternion.LookRotation(direction, transform.up);
	}

	override protected void ExplodeShip()
	{
		CrossHairController.isEnabled = false;
		isAlive = false;
		StopEngines();
		base.ExplodeShip();
	}
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyBullet")
        {
            StartCoroutine(Deleteline(healthBar));
        }
    }

    public IEnumerator Deleteline(Image line)
    {
        int i = 0;
        while (i <= 10)
        {
            line.fillAmount -= 0.02f;
            i++;
            yield return new WaitForSeconds(0.03f);
        }
        yield return null;
    }
}
