using UnityEngine;

public class MasterManager : MonoBehaviour
{

    private static MasterManager instance;

    public static MasterManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if(MasterManager.Instance == null)
        {
            MasterManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (MasterManager.Instance != this)
        {
            Destroy(gameObject);
        }
    }


}
