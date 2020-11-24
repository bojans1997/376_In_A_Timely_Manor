﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class FullScreenInventory : MonoBehaviour {
    public PostProcessVolume volume;
    private DepthOfField depthOfField;

    public static bool inMenu;

    private static FullScreenInventory instance;

    public Animator animator;

    public GameObject text;
    public GameObject clock;
    public GameObject inventory;

    void Start() {
        volume.profile.TryGetSettings(out depthOfField);

        inMenu = false;
        instance = this;

        gameObject.SetActive(false);
    }

    public static void startFullScreenInventory() {
        inMenu = true;

        instance.text.SetActive(false);
        instance.clock.SetActive(false);
        instance.inventory.SetActive(false);

        instance.gameObject.SetActive(true);
        instance.animator.SetTrigger("Start");
        instance.StartCoroutine(instance.onStartFullScreenInventory());
    }

    public static void exitFullScreenInventory() {
        inMenu = false;

        instance.text.SetActive(true);
        instance.clock.SetActive(true);
        instance.inventory.SetActive(true);

        instance.animator.SetTrigger("Exit");
        instance.StartCoroutine(instance.onExitFullScreenInventory());
    }

    IEnumerator onStartFullScreenInventory() {
        float timer;

        depthOfField.focalLength.value = 50.0f;

        timer = 0.0f;
        while (timer < 0.5f) {
            timer += Time.deltaTime;

            depthOfField.focalLength.value = Mathf.Lerp(50.0f, 100.0f, timer / 0.5f);

            yield return null;
        }
    }

    IEnumerator onExitFullScreenInventory() {
        float timer;

        timer = 0.0f;
        while (timer < 0.5f) {
            timer += Time.deltaTime;

            depthOfField.focalLength.value = Mathf.Lerp(100.0f, 0.0f, timer / 0.5f);
            yield return null;
        }
    }
}