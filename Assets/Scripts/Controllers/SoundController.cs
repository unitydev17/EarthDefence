using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{

	public static SoundController instance;
	private const float SILENCE_DISTANCE = 100f;

	private AudioSource audioSource;
	public AudioClip fireClip;
	public AudioClip shipShotedClip;
	public AudioClip enemyExplosionClip;


	void Awake()
	{
		if (instance == null) {
			instance = this;
		}

		audioSource = Camera.main.GetComponent<AudioSource>();
	}


	public void PlayerFire()
	{
		audioSource.volume = 0.8f;
		audioSource.PlayOneShot(fireClip);
	}


	public void ShipShoted()
	{
		audioSource.PlayOneShot(shipShotedClip);
	}


	public void EnemyExplosion(float distance)
	{
		audioSource.volume = 1 - distance/SILENCE_DISTANCE;
		audioSource.PlayOneShot(enemyExplosionClip);
	}

}
