using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundController : MonoBehaviour
{

	public static SoundController instance;
	private const float SILENCE_EXPLOSION_DISTANCE = 150f;
	private const float SILENCE_LAZER_DISTANCE = 50f;
	private const float PLAYER_NOICE_LEVEL = 0.1f;

	private AudioSource _audioSource_Player;
	private AudioSource _audioSource_PlayerVoice;
	private AudioSource _audioSource_Bots;
	private AudioSource _audioSource_BotsExplosion;
	private AudioSource _audioSource_Music;


	public AudioClip fireClip;
	public AudioClip shipShotedClip;
	public AudioClip enemyExplosionClip;
	public AudioClip storyRetryClip;
	public AudioClip storyWinClip;


	public AudioClip battleMusicClip;


	void Awake()
	{
		if (instance == null) {
			instance = this;
		}
		SetupAudioSources ();
	}


	void SetupAudioSources() {
		_audioSource_Player = Camera.main.gameObject.AddComponent<AudioSource>();
		_audioSource_Bots = Camera.main.gameObject.AddComponent<AudioSource>();
		_audioSource_BotsExplosion = Camera.main.gameObject.AddComponent<AudioSource>();
		_audioSource_Music = Camera.main.gameObject.AddComponent<AudioSource>();
		_audioSource_PlayerVoice = Camera.main.gameObject.AddComponent<AudioSource>();
	}


	public void PlayBattleMusic() {
		_audioSource_Music.loop = true;
		_audioSource_Music.clip = battleMusicClip;
		_audioSource_Music.Play ();
	}


	public void PauseBattleMusic() {
		_audioSource_Music.Pause ();
	}


	public void UnPauseBattleMusic() {
		_audioSource_Music.UnPause ();
	}


	public void PlayStoryRetry() {
		_audioSource_Player.PlayOneShot (storyRetryClip);
	}


	public void PlayStoryWin() {
		_audioSource_PlayerVoice.PlayOneShot (storyWinClip);
	}


	public void PlayerFire()
	{
		_audioSource_Player.PlayOneShot(fireClip);
	}


	public void BotFire(float distance)
	{
		if (distance >= SILENCE_LAZER_DISTANCE) {
			return;
		}

		float volume = 1f - distance/SILENCE_LAZER_DISTANCE;
		_audioSource_Bots.PlayOneShot(fireClip, volume);
	}


	public void ShipShoted()
	{
		_audioSource_Player.PlayOneShot(shipShotedClip, PLAYER_NOICE_LEVEL);
	}


	public void Explosion(float distance)
	{
		float volume = 1f - distance/SILENCE_EXPLOSION_DISTANCE;
		_audioSource_BotsExplosion.PlayOneShot(enemyExplosionClip, volume);
	}

}
