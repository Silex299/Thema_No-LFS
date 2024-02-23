using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{


    [SerializeField] private int currentLevelIndex;

    private static LevelManager instance;

    public static LevelManager Instance
    {
        get => instance;
    }

    private void Awake()
    { 

        if(LevelManager.Instance == null)
        {
            instance = this;
        }
        else if(LevelManager.Instance != this)
        {
            Destroy(this);
        }

    }


    /// <summary>
    /// Normal Scene Change 
    /// </summary>
    /// <param name="index"> scene index to load </param>
    public void ChangeLevel(int index)
    {
        currentLevelIndex = index;
        SceneManager.LoadScene(index);
    }



}
