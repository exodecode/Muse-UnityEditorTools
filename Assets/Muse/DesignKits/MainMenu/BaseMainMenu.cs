using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Muse
{
    public class BaseMainMenu : MonoBehaviour
    {
        public GameObject baseButtonPrefab;

        Button[] sceneButtons;

        Button GenerateSceneButton(int sceneIndex)
        {
            var g = Instantiate(baseButtonPrefab);
            var text = g.GetComponent<TMPro.TMP_Text>();
            var button = g.GetComponent<Button>();
            var sceneName = SceneNameFromIndex(sceneIndex);

            text.text = $"{sceneIndex}: {sceneName}";
            button.onClick.AddListener(() => SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single));

            return button;
        }

        Button[] GenerateSceneButtons() =>
            Enumerable.Range(0, GetSceneCount())
                .Select(i => GenerateSceneButton(i))
                .ToArray();

        int GetSceneCount()
        {
            return 0;
        }

        static string SceneNameFromIndex(int sceneIndex)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            int slash = path.LastIndexOf('/');
            string name = path.Substring(slash + 1);
            int dot = name.LastIndexOf('.');
            return name.Substring(0, dot);
        }
    }
}