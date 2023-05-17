using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Range(1, 3)]
    public int level = 1;

    [Tooltip("The y coordinate of the bottom boundary")]
    public float minYPosition = -10;
    public float minCameraY = -2;

    [Header("GUI and Text")]
    public GameObject gameOverDialog;
    public GameObject alertMessage;
    public float alertTime = 1.0f;


    [Header("Music and Sound Effects")]
    //public AudioClip[] levelTracks;
    public AudioClip gameOverSound;

    //private AudioSource musicSource;
    private AudioSource soundSource;

    public bool GameRunning { get; private set; } = true;

    public static GameManager Instance { get; private set; } // singleton

    private void Awake()
    {
        // If there is an instance, and it is not this one, delete it.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        Instance = this;
    }

    void Start()
    {
        //musicSource = GetComponents<AudioSource>()[0];
        soundSource = GetComponent<AudioSource>(); // sound effects source

        /*
        if (levelTracks.Length > 0) { 
            musicSource.clip = levelTracks[level - 1];
            musicSource.Play();
        }*/

        if(gameOverDialog)
            gameOverDialog.SetActive(false);

        if(alertMessage)
            alertMessage.SetActive(false);

        Time.timeScale = 1;
    }

    public void EndGame()
    {
        gameOverDialog.SetActive(true);
        GetComponents<AudioSource>()[0].Stop(); // stop music playing
        PlaySound(gameOverSound);

        Time.timeScale = 0;
        GameRunning = false;

        Cursor.lockState = CursorLockMode.Confined;
    }

    public IEnumerator ShowAlert()
    {
        alertMessage.SetActive(true);
        yield return new WaitForSeconds(alertTime);
        alertMessage.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1); // restart first level
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void PlaySound(AudioClip clip, float volumeScale = 1)
    {
        if (clip != null)
            soundSource.PlayOneShot(clip, volumeScale);
        else
            Debug.LogWarning("Sound effect has not been assigned to PlayerSound.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(new Vector2(0, minYPosition), new Vector2(100, 1));
    }

}