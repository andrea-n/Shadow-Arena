using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class InGameMenuManager : MonoBehaviour {

	public Canvas inGameCanvas;
	public Canvas menuCanvas;
	public MenuManager menuManager;

	private bool isPaused = false;

	public void TogglePause(){
		isPaused = !isPaused;
		inGameCanvas.gameObject.SetActive (!isPaused);
		menuCanvas.gameObject.SetActive (isPaused);
		Camera.main.GetComponent<Blur> ().enabled = isPaused;
		Camera.main.GetComponent<cameraMovement> ().enabled = !isPaused;
		switch (isPaused) {
		case true:
			Time.timeScale = 0f;
			break;
		case false:
			Time.timeScale = 1f;
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			TogglePause ();
		}
	}
}
