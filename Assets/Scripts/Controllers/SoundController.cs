using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{

	public static SoundController instance;
	private const float SILENCE_EXPLOSION_DISTANCE = 150f;
	private const float SILENCE_LAZER_DISTANCE = 50f;
	private const float PLAYER_NOICE_LEVEL = 0.1f;

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
		audioSource.volume = 0.5f;
	}


	public void PlayerFire()
	{
		audioSource.PlayOneShot(fireClip, PLAYER_NOICE_LEVEL);
	}


	public void BotFire(float distance)
	{
		if (distance >= SILENCE_LAZER_DISTANCE) {
			return;
		}

		float volume = 1f - distance/SILENCE_LAZER_DISTANCE;
		audioSource.PlayOneShot(fireClip, volume);
	}


	public void ShipShoted()
	{
		audioSource.PlayOneShot(shipShotedClip, PLAYER_NOICE_LEVEL);
	}


	public void EnemyExplosion(float distance)
	{
		float volume = 1f - distance/SILENCE_EXPLOSION_DISTANCE;
		audioSource.PlayOneShot(enemyExplosionClip, volume);
	}

}
