using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image loadingBar;
    [SerializeField] private Image loadingBackground;

    #region Instance

    public static LoadingScreen instance;

    void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than 1 instance of LoadingScreen exists. Destroying " + gameObject.name);
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    #endregion

    // #region Test
    // public void StartLoad(string sceneName)
    // {
    //     StartCoroutine(LoadingTest(sceneName));
    // }

    // IEnumerator LoadingTest(string sceneName)
    // {
    //     AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
    //     op.allowSceneActivation = false;

    //     while(!op.isDone)
    //     {
    //         if(op.progress >= .9f)
    //             op.allowSceneActivation = true;
            
    //         yield return null;
    //     }
    // }

    // #endregion

    public void StartLoad(AsyncOperation operation)
    {
        StartCoroutine(Loading(operation));
    }
    
    IEnumerator Loading(AsyncOperation operation)
    {
        // UI Background ON
        loadingBackground.enabled = true;

        operation.allowSceneActivation = false;

        while(operation.isDone == false)
        {
            Debug.Log(operation.progress);
            loadingBar.fillAmount = operation.progress;

            if(operation.progress >= .5f)
                operation.allowSceneActivation = true;
            
            yield return null;
        }

        // UI Background & Bar OFF
        loadingBackground.enabled = false;
        loadingBar.fillAmount = 0;
    }
}
