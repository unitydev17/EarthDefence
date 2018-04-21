using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering.PostProcessing;


public class PostProcessingCtrl : MonoBehaviour
{
	private const float SPEED_UP = 1f;
	private const float SLOW_DOWN = 1f;

	private PostProcessVolume volume;
	private float weight;
	private bool speedUp;


	void Start()
	{
		PlayerController.eventHandlers += AdjustSpeedEffect;
		volume = GetComponent<PostProcessVolume>();
		weight = 0;
		speedUp = false;
	}


	public void AdjustSpeedEffect(bool speedUp)
	{
		this.speedUp = speedUp;
	}


	void Update()
	{
		ProcessStates();
	}


	void ProcessStates()
	{
		if (speedUp) {
			weight += Time.deltaTime * SPEED_UP;
		} else {
			weight -= Time.deltaTime * SLOW_DOWN;
		}
		weight = Mathf.Clamp(weight, 0, 1);
		volume.weight = weight;
	}


}
