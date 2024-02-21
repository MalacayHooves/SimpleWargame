using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SimpleWargame.MenuManagement
{
    /// <summary>
    /// This class handles loading of different scenes
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        [Header("Links to Objects")]
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI loadingText;

        /// <summary>
        /// Key is a name of opened scene, value is bool keep them opend or not (if true, then keep)
        /// </summary>
        private Dictionary<string, bool> openedScenes = new Dictionary<string, bool>();

        public bool IsLoading { get; private set; }

        private List<string> scenesToDeleteFromDictionary = new List<string>();

        private void Start()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void LoadScene(string newSceneName)
        {
            background.gameObject.SetActive(true);
            loadingText.gameObject.SetActive(true);
            
            foreach (string sceneName in openedScenes.Keys)
            {
                if (!openedScenes[sceneName])
                {
                    scenesToDeleteFromDictionary.Add(sceneName);
                    SceneManager.UnloadSceneAsync(sceneName);
                }
            }

            foreach (string sceneName in scenesToDeleteFromDictionary)
            {
                openedScenes.Remove(sceneName);
            }

            scenesToDeleteFromDictionary.Clear();
            scenesToDeleteFromDictionary.TrimExcess();

            SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
            IsLoading = true;
        }

        public void LoadSceneAdditive(string newSceneName)
        {
            background.gameObject.SetActive(true);
            loadingText.gameObject.SetActive(true);

            SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

            IsLoading = true;
        }

        public void SceneIsLoaded(string newSceneName, bool keepSceneLoaded)
        {
            openedScenes.Add(newSceneName, keepSceneLoaded);

            background.gameObject.SetActive(false);
            loadingText.gameObject.SetActive(false);

            IsLoading = false;
        }
    }
}
