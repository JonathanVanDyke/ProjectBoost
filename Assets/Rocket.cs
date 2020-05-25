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
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelSound;
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
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; }

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                print("OK"); 
                break;
            case "Finish":
                print("Hit Finish");
                state = State.Trancending;
                audiosource.PlayOneShot(levelSound);
                Invoke("LoadNextScene", 1f);
                break;
            default:
                print("Dead");
                state = State.Dying;
                audiosource.Stop();
                audiosource.PlayOneShot(deathSound);
                Invoke("ResetScene", 1f);
                break;
        }
    }

    private void ResetScene()
    {
        audiosource.Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        audiosource.Stop();
        SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
        state = State.Alive;
    }

    private void ThrustInputs()
    {
        var spacePressed = Input.GetKey(KeyCode.Space);
        var spaceUp = Input.GetKeyUp(KeyCode.Space);

        if (spacePressed)
        {
            rigidBody.AddRelativeForce(Vector3.up * thrust * Time.deltaTime);
            currentState = RocketState.Thrusting;
            trailRenderer.time = 1;
        }

        if (spaceUp)
        {
            currentState = RocketState.Stalled;
            trailRenderer.time = 0;
        }
    }

    private void RotateInputs()
    {

        rigidBody.freezeRotation = true;
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
        rigidBody.freezeRotation = false;
        
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
                break;
        }
    }


}
