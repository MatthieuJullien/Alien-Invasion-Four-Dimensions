using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    public static bool m_isQuitting; // Adjusting the Event Bus section ???
    private static T m_Instance;

    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<T>();

                if (m_Instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    m_Instance = obj.AddComponent<T>();
                }
            }
            return m_Instance;
        }
    }

    // virtual Awake that can be overriden in a derived class
    public virtual void Awake()
    {
        if (m_Instance == null)
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
