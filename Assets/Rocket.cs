using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audiosource;
    TrailRenderer trailRenderer;
    enum RocketState {Thrusting, Grounded, Stalled};
    RocketState currentState = RocketState.Stalled;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        AudioManager();
    }

    private void ProcessInput()
    {
        ThrustInputs();
        RotateInputs();
    }

    private void ThrustInputs()
    {
        var spacePressed = Input.GetKey(KeyCode.Space);
        var spaceUp = Input.GetKeyUp(KeyCode.Space);

        if (spacePressed)
        {
            rigidBody.AddRelativeForce(Vector3.up);
            currentState = RocketState.Thrusting;
            //trailRenderer.enabled = true;
            trailRenderer.time = 1;
        }

        if (spaceUp)
        {
            currentState = RocketState.Stalled;
            //trailRenderer.enabled = false;
            trailRenderer.time = 0;
        }
    }

    private void RotateInputs()
    {
        var aPressed = Input.GetKey(KeyCode.A);
        var dPressed = Input.GetKey(KeyCode.D);

        if (aPressed && !dPressed)
        {
            transform.Rotate(Vector3.forward);
        }

        if (dPressed && !aPressed)
        {
            transform.Rotate(-Vector3.forward);
        }
    }

    private void AudioManager()
    {
        switch (currentState)
        {
            case RocketState.Thrusting:
                if (!audiosource.isPlaying)
                {
                    audiosource.volume = .379f;
                    audiosource.Play();
                }
                break;
            case RocketState.Stalled:
                audiosource.Stop();
                break;
        }
    }


}
