
using UnityEngine;
using System.Collections;

public class SingletonComponent<T> : ComponentBase where T : MonoBehaviour
{
    private static T instance = null;

    protected virtual void Awake() => SetInstance();

    /// <summary>
    /// 인스턴스를 얻어옵니다.
    /// </summary>
    public static T Instance
    {
        get
        {
            CheckInstance();
            return instance;
        }
    }

    /// <summary>
    /// 할당된 인스턴스가 있나 체크하고, 없다면 새로 인스턴스를 만듭니다.
    /// </summary>
    public static void CheckInstance()
    {
        if (instance == null)
        {
#if CUSTOM_LOG_ON
            UnityExtend.Logger.Push(UnityExtend.eLogType.GameMgr_Init);
#endif
            GameObject obj = new GameObject(string.Format("[ {0} ]", typeof(T)));
            instance = obj.AddComponent<T>();
        }
    }

    /// <summary>
    /// 인스턴스를 셋팅하거나 이미 인스턴스가 있는 경우 현재 오브젝트를 파괴합니다. 통상적으로 Awake에서 부릅니다.
    /// </summary>
    protected bool SetInstance(bool dontDestroyOnLoad = false)
    {
        if (instance != null && instance != this)
        {
            Debug.Log(string.Format("Duplicated UniqueComponent <{0}>.", this));

            DestroyImmediate(this);

            return false;
        }

        instance = this as T;

        if (dontDestroyOnLoad) DontDestroyOnLoad(this);

        return true;
    }

    public static bool HasInstance => instance != null;
}
