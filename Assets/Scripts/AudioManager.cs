﻿using UnityEngine.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;

            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public void Play(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.Play();
    }

    public void Stop(string name) {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + base.name + " not found!");
            return;
        }

        //s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        //s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Stop();
    }

    public void StopFadeOut(string name) {
        StopFadeOut(name, 1.0f);
    }

    public void StopFadeOut(string name, float secondsToFadeOut) {
        Sound s = Array.Find(sounds, item => item.name == name);
        if (s == null) {
            Debug.LogWarning("Sound: " + base.name + " not found!");
            return;
        }

        AudioSource audioSource = s.source;
        float startingVolume = audioSource.volume;
        StartCoroutine(FadeOut(startingVolume, audioSource, secondsToFadeOut));
    }

    IEnumerator FadeOut(float startingVolume, AudioSource audioSource, float secondsToFadeOut) {
        float timer;

        timer = 0.0f;
        while (timer < secondsToFadeOut) {
            timer += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(startingVolume, 0.0f, timer / secondsToFadeOut);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }
}