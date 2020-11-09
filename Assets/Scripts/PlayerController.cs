﻿using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // Getter and setter
    public static bool CanMove { get; set; }

    [SerializeField]
    float speed = 5.0f;

    [SerializeField]
    bool inPast = false;
    private bool isSafeSpot;
    private int numOfTries;
    Vector2 telePosition;
    [SerializeField]
    float travel = 150.0f; // Distance of 2nd timeline in y

    [SerializeField]
    Text timeIndicator;

    public TextMeshProUGUI interactTextObject;

    private Vector2 movement;

    public Animator animator;

    public static bool isTravelling;
    public PostProcessVolume volume;
    private Bloom bloom;
    private LensDistortion lensDistortion;
    private Grain grain;
    private DepthOfField depthOfField;
    private ColorGrading colorGrading;
    private ChromaticAberration chromaticAberration;

    void Start() {
        volume.profile.TryGetSettings(out bloom);
        volume.profile.TryGetSettings(out lensDistortion);
        volume.profile.TryGetSettings(out grain);
        volume.profile.TryGetSettings(out depthOfField);
        volume.profile.TryGetSettings(out colorGrading);
        volume.profile.TryGetSettings(out chromaticAberration);

        isSafeSpot = true;
        numOfTries = 0;

        CanMove = true;
        isTravelling = false;
        timeIndicator.text = "Present";
    }

    // Update is called once per frame
    void Update()
    {
        if(CanMove) {
            // Time travel.
            if (Input.GetButtonDown("TimeShift"))
                TimeShift();
        } else {
            animator.SetFloat("Speed", 0);
        }

    }

    void FixedUpdate()
    {
        if (CanMove)
        {
            MovePlayer();
        }
    }


    void MovePlayer() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (movement != Vector2.zero) {
            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);
        }

        animator.SetFloat("Speed", movement.sqrMagnitude);

        this.transform.Translate(movement.normalized * speed * Time.deltaTime);
    }

    // Go from past to present and vice-versa
    public void TimeShift() {
        // Boolean switch
        inPast = !inPast;

        // HUD element
        // TODO change for cool animation
        Clock.TimeTravel();
        timeIndicator.text = inPast ?  "Past" : "Present";

        StartCoroutine(StartTimeShift());
    }

    Vector3 checkTeleportPosition() {
        telePosition = this.transform.position + new Vector3(0, (inPast ? travel : -travel), 0);
        RaycastHit2D hit = Physics2D.Raycast(telePosition, Vector2.up);

        if (hit.collider != null && hit.collider.bounds.Contains(new Vector3(telePosition.x, telePosition.y, hit.transform.position.z))) {

            isSafeSpot = false;
            while (isSafeSpot == false) {
                telePosition = this.transform.position + new Vector3(Random.Range(-3 - (numOfTries / 10), 3 + (numOfTries / 10)), (inPast ? travel : -travel) + Random.Range(-3 - (numOfTries / 10), 3 + (numOfTries / 10)), 0);

                hit = Physics2D.Raycast(telePosition, Vector2.up);

                if (hit.collider == null || !hit.collider.bounds.Contains(new Vector3(telePosition.x, telePosition.y, hit.transform.position.z))) {
                    isSafeSpot = true;
                }
                if (numOfTries <= 20) {
                    numOfTries += 1;
                }

            }
            numOfTries = 0;
        }

        return telePosition;
    }


    // Time shift animation
    public IEnumerator StartTimeShift() {
        CanMove = false;

        for (int i = 0; i < 100; i++) {
            bloom.intensity.value = i / 4;
            lensDistortion.intensity.value = i / 5;
            depthOfField.focalLength.value = i / 1.5f;
            chromaticAberration.intensity.value = i / 200;

            yield return new WaitForSeconds(0.005f);
        }

        //this.transform.position = checkTeleportPosition();
        this.transform.position = new Vector2(this.transform.position.x, inPast ? this.transform.position.y + 150 : this.transform.position.y - 150);

        grain.intensity.value = (inPast ? 1 : 0);

        if (inPast) {
            colorGrading.active = true;
            colorGrading.mixerBlueOutGreenIn.value = 50;
            colorGrading.mixerBlueOutBlueIn.value = 50;
            colorGrading.mixerBlueOutRedIn.value = 50;
        } else {
            colorGrading.active = false;
            colorGrading.mixerBlueOutGreenIn.value = 0;
            colorGrading.mixerBlueOutBlueIn.value = 0;
            colorGrading.mixerBlueOutRedIn.value = 0;
        }

        for (int i = 100; i >= 0; i--) {
            bloom.intensity.value = i / 4;
            lensDistortion.intensity.value = i / 5;
            depthOfField.focalLength.value = i / 1.5f;
            chromaticAberration.intensity.value = i / 200;

            yield return new WaitForSeconds(0.005f);
        }

        CanMove = true;

        //is set to true from whichever script that cares if the player is travelling
        isTravelling = false;

    }


}
