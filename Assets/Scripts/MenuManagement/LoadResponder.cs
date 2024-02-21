using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleWargame.MenuManagement
{
    /// <summary>
    /// responses when scene is loaded
    /// </summary>
    public class LoadResponder : MonoBehaviour
    {
        [SerializeField] private string sceneName;
        [SerializeField] private bool keepSceneLoaded;

        async void Start()
        {            
            await ResponseWhenSceneIsLoaded();
        }

        public async Task ResponseWhenSceneIsLoaded()
        {
            while (SceneLoader.Instance == null) await Task.Yield();

            SceneLoader.Instance.SceneIsLoaded(sceneName, keepSceneLoaded);
        }
    }
}
