using UnityEngine;
using System.Collections;

public class BackPressCloseHandler : MonoBehaviour
{
    static protected BackPressCloseHandler m_Instance;

    /*
    static public BackPressCloseHandler instance
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject o = new GameObject("BackPressCloseHandler");
                DontDestroyOnLoad(o);
                m_Instance = o.AddComponent<BackPressCloseHandler>();
            }

            return m_Instance;
        }
    }
    */

    void Awake()
    {
        if (m_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
                return;
            }
        }
    }
}