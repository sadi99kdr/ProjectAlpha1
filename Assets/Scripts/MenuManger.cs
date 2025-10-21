using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManger : MonoBehaviour
{
    private int selected_level = -1;
    public Button[] levelButtons;
    void Start()
    {
        
    }

    public void ExitGame()
    {
        Debug.Log("Exiting game...");

    #if UNITY_EDITOR
            // Stop play mode if testing in Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
    #else
            // Quit the built application
            Application.Quit();
    #endif
    }

    public void OnclickLevelSelector(int level)
    {
        selected_level = level;
        Debug.Log("Selected: " + selected_level);

        for(int i =0; i < levelButtons.Length; i++)
        {
            levelButtons[i].interactable = true;
        }

        levelButtons[selected_level].interactable = false;

    }

    public void PlaySelectedLevel()
    {
        if (selected_level ==-1)
        {
            Debug.LogWarning("Please select a level");
            return;
        }
        string sceneName = "Level" + (selected_level + 1);
        SceneManager.LoadScene(sceneName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
