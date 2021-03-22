using UnityEngine;

public class DemoManager : MonoBehaviour
{
    #region Singleton Pattern
    private static DemoManager instance;
    private DemoManager()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
    }
    public static DemoManager Instance
    {
        get
        {
            if (instance == null)
            {
                new DemoManager();
            }
            return instance;
        }
    }
    #endregion

    public bool IsReadingFromFile { get => isReadingFromFile; }
    [SerializeField] private bool isReadingFromFile = true;

    public void Quit()
    {
        Application.Quit();
    }
}
