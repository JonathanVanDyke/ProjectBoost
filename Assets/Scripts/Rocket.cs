using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    #region ExternalComponents
    Rigidbody rigidBody;
    AudioSource audiosource;
    TrailRenderer trailRenderer;
    #endregion

    #region Fields
    [SerializeField] float thrust = 100f;
    [SerializeField] float rotateMag = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathSoundParticles;
    [SerializeField] ParticleSystem levelSoundParticles;
    #endregion

    #region GameState
    enum RocketState {Thrusting, Grounded, Stalled};
    RocketState currentState = RocketState.Stalled;
    enum State { Alive, Dying, Trancending }
    State state = State.Alive;
    #endregion

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
        if (state == State.Alive)
        {
            ProcessInput();
            AudioManager();
        }
    }

    private void ProcessInput()
    {
        ThrustInputs();
        RotateInputs();
        LevelSkipper();
    }

    private void LevelSkipper()
    {
        if (Input.GetKey(KeyCode.L))
        {
            LoadNextScene();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                WinProcess();
                break;
            default:
                DeathProcess();
                break;
        }
    }

    private void DeathProcess()
    {
        state = State.Dying;
        audiosource.Stop();
        deathSoundParticles.Play();
        audiosource.PlayOneShot(deathSound);
        Invoke("ResetScene", levelLoadDelay);
    }

    private void WinProcess()
    {
        levelSoundParticles.Play();
        state = State.Trancending;
        audiosource.PlayOneShot(levelSound);
        Invoke("LoadNextScene", levelLoadDelay);
    }

    private void ResetScene()
    {
        audiosource.Stop();
        deathSoundParticles.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        audiosource.Stop();
        levelSoundParticles.Stop();
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
        state = State.Alive;
    }

    private void ThrustInputs()
    {
        var spacePressed = Input.GetKey(KeyCode.Space);
        var spaceUp = Input.GetKeyUp(KeyCode.Space);

        if (spacePressed)
        {
            ApplyThrust();
        }

        if (spaceUp)
        {
            currentState = RocketState.Stalled;
            trailRenderer.time = 0;
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * thrust * Time.deltaTime);
        currentState = RocketState.Thrusting;
        trailRenderer.time = 1;
        mainEngineParticles.Play();
    }

    private void RotateInputs()
    {

        rigidBody.angularVelocity = Vector3.zero;
        var aPressed = Input.GetKey(KeyCode.A);
        var dPressed = Input.GetKey(KeyCode.D);

        if (aPressed && !dPressed)
        {
            transform.Rotate(Vector3.forward * rotateMag * Time.deltaTime);
        }

        if (dPressed && !aPressed)
        {
            transform.Rotate(-Vector3.forward * rotateMag * Time.deltaTime);
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
                    audiosource.PlayOneShot(mainEngine);
                }
                break;
            case RocketState.Stalled:
                audiosource.Stop();
                mainEngineParticles.Stop();
                break;
        }
    }


}
