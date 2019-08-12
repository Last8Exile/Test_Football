using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
   
    [Header("Init")]
    [SerializeField] private string mGameSceneName;

    public void StartGame()
    {
        #if UNITY_EDITOR
        CheckErrors();
        #endif

        SceneManager.LoadScene(mGameSceneName);
    }

    private void CheckErrors()
    {
        if (string.IsNullOrEmpty(mGameSceneName))
            throw new UnityException("Null ref to" + nameof(mGameSceneName));
    }
}
