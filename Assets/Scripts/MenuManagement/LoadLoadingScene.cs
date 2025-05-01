using SimpleWargame.MenuManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleWargame
{
    /// <summary>
    /// this class loads loading scene on a start of the game
    /// </summary>
    public class LoadLoadingScene : MonoBehaviour
    {
        private const string LOADING_SCENE_NAME = "LoadingScene";
        private void Awake()
        {
            if (SceneLoader.Instance == null) SceneManager.LoadScene(LOADING_SCENE_NAME, LoadSceneMode.Additive);
        }
    }
}
