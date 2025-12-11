using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zlipacket.CoreZlipacket.Tools;

namespace Zlipacket.CoreZlipacket.Scene
{
    public class SceneController : Singleton<SceneController>
    {
        [SerializeField] private string loadingSceneName;
        [SerializeField] private Animator transition;
        [SerializeField] private float transitionTime = 0.5f;
        [SerializeField] private float fakeLoadingTime = 0.5f;
        
        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
            else
            {
                StartCoroutine(TransitionToScene(sceneName));
            }
            
            Debug.Log("Scene " + sceneName + " loaded");
        }

        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadSceneAsync(sceneName);
            Debug.Log("Scene" + sceneName + " loaded async");
        }

        public void UnloadSceneAsync(string sceneName)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            Debug.Log("Scene" + sceneName + " unloaded async");
        }
        
        private IEnumerator TransitionToScene(string sceneName)
        {
            transition.gameObject.SetActive(true);
            transition.SetTrigger("Start");
            yield return new WaitForSeconds(transitionTime);
            
            //Loading Screen
            SceneManager.LoadScene(loadingSceneName);
            yield return new WaitForSeconds(fakeLoadingTime);
            
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncOperation.isDone)
            {
                yield return null;
            }
            
            transition.SetTrigger("End");
            yield return new WaitForSeconds(transitionTime);
            transition.gameObject.SetActive(false);
        }
    }
}