using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Level_Manager : MonoBehaviour {
	
	public void LoadLevel(string name){
		SceneManager.LoadScene(name);
	}
	
	public void QuitLevel(){
		Application.Quit();
	}

    public void LoadNextLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void BrickDestroyed() {
        if (Brick3D.breakableCount <= 0) {
            LoadNextLevel();
        }
    }

}
