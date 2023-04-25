using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
          LoadingScreenScene,
          MainMenuScene,
          SampleScene,
    }

    public static Action _onLoadCallback;

    public static void LoadScene(Scene scene){

        _onLoadCallback = () => {
            SceneManager.LoadScene(scene.ToString());
        };

        SceneManager.LoadScene(Scene.LoadingScreenScene.ToString());
    }

    public static void LoadCallback(){
        if(_onLoadCallback != null){
            _onLoadCallback();
            _onLoadCallback = null;
        }
    }

    public static void LoadNewGame(){
        SceneLoader.LoadScene(Scene.SampleScene);
    }

    public static void LoadMainMenu(){
        //
    }
}
