﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Singleton para centralizar el manejo de audio y musica de las escenas
 * Esteban.Hernandez
 */
public class SoundManager : MonoBehaviour {

	public AudioSource fxSource;
	public AudioSource playerSource;
	public AudioSource musicSource;
	public static SoundManager instance = null;

	public AudioClip[] bgSceneMusic;

	public float lowPitch = 0.95f;
	public float highPitch  = 1.05f;


	void Awake() {
		MakeSingleton ();
	}

	private void MakeSingleton() {
		if (instance != null) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	public void StopBackgroundMusic()
	{
		musicSource.Stop();
	}


	public void StartBackgroundMusic()
	{
		musicSource.Play();
	}

	public void StartBackgroundMusic(AudioClip bgm)
	{
		musicSource.clip = bgm;
		musicSource.Play();
	}

	public void PlayOnce(AudioClip clip)
	{
		if (!fxSource.isPlaying)
		{
			fxSource.clip = clip;
			fxSource.pitch = 1f;
			fxSource.Play();
		}
	}
	public void PlayBGM(AudioClip clip)
	{
		musicSource.Stop();
		musicSource.clip = clip;
		musicSource.pitch = 1f;
		musicSource.Play();
	}

	public void PlayOnce(AudioClip clip, float pitch)
	{
		fxSource.clip = clip;
		fxSource.pitch = pitch;
		fxSource.Play();
	}


	public void PlayPlayerOnce (AudioClip clip){
		if (!playerSource.isPlaying)
		{
			playerSource.volume = 1f;
			playerSource.clip = clip;
			playerSource.pitch = 1f;
			playerSource.Play();
		}
	}


	public void RandomizePlayerFx (params AudioClip [] clips){
		
		int random = Random.Range (0, clips.Length);
		float randomPitch = Random.Range (this.lowPitch, this.highPitch);

		playerSource.clip = clips[random];
		playerSource.pitch = randomPitch;
		playerSource.Play ();
	}

	public void RandomizeFx(params AudioClip[] clips)
	{

		int random = Random.Range(0, clips.Length);
		float randomPitch = Random.Range(this.lowPitch, this.highPitch);
		if (clips.Length > 0) { 
			fxSource.clip = clips[random];
			fxSource.pitch = randomPitch;
			fxSource.Play();
		}
	}

	public void changeMusic(int index){
		if (bgSceneMusic[index] != musicSource.clip)
		{
			musicSource.Stop();
			if (bgSceneMusic.Length > index)
			{
				musicSource.clip = bgSceneMusic[index];
			}
			else
			{
				musicSource.clip = bgSceneMusic[0];
			}
			musicSource.Play();
		}
	}
}
