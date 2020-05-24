using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audiosource;
    enum RocketState {Thrusting, Grounded, Stalled};
    RocketState currentState = RocketState.Stalled;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
        AudioManager();
    }

    private void ProcessInput()
    {
        var spacePressed = Input.GetKey(KeyCode.Space);
        var spaceUp = Input.GetKeyUp(KeyCode.Space);
        var aPressed = Input.GetKey(KeyCode.A);
        var dPressed = Input.GetKey(KeyCode.D);

        if (spacePressed)
        {
            rigidBody.AddRelativeForce(Vector3.up);
            currentState = RocketState.Thrusting;
        }

        if (spaceUp)
        {
            currentState = RocketState.Stalled;
        }
        
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
