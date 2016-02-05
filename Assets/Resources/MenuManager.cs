using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class MenuManager : MonoBehaviour {

	public static Preferences preferences;

	public GameObject tooltipPanel;
	public SpinnerHorizontal resolutionSpinner;
	public SpinnerHorizontal fullscreenSpinner;
	public SpinnerHorizontal antialiasingSpinner;
	public SpinnerHorizontal occlusionSpinner;
	public SpinnerHorizontal bloomSpinner;
	public SpinnerHorizontal depthOfFieldSpinner;
	public SpinnerHorizontal vignetteSpinner;
	public SpinnerHorizontal vSyncSpinner;

	private List<Resolution> resolutions;
	private int screenshotCounter = 0;

	//TOOLTIP CONTROLS
	public void DisplayTooltip(string message){
		tooltipPanel.SetActive (true);
		tooltipPanel.GetComponentInChildren<Text> ().text = message.Replace("\\n", "\n");
	}

	public void HideTooltip (){
		tooltipPanel.SetActive (false);
		tooltipPanel.GetComponentInChildren<Text> ().text = "";
	}

	public IEnumerator DisplayTimedTooltip(string message){
		DisplayTooltip (message);
		yield return new WaitForSeconds (4f);
		HideTooltip ();
	}

	public void QuitGame(){
		Application.Quit ();
	}

	public void StartGame(){
		SceneManager.LoadScene (1);
	}

	public void QuitToMenu(){
		SceneManager.LoadScene (0);
	}

	//PREFS LOAD AND SAVE
	public void LoadPreferences(){
		//Loads prefs is file exists or creates new default prefs
		if (File.Exists (Application.dataPath + "/userprefs.data")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.dataPath + "/userprefs.data", FileMode.Open);
			preferences = (Preferences)bf.Deserialize (file);
			file.Close ();
		} else {
			string currentResolution = Screen.currentResolution.width.ToString () + "x" + Screen.currentResolution.height.ToString ();
			preferences = new Preferences (resolutionSpinner.values.IndexOf(currentResolution));
		}
		screenshotCounter = preferences.screenshotIndex;
	}

	public void SavePreferences(){
		//Saves prefs to a file
		preferences.screenshotIndex = screenshotCounter;
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create(Application.dataPath + "/userprefs.data");
		bf.Serialize (file, preferences);
		file.Close ();
	}

	public void SetPreferences(){
		preferences.resolution = resolutionSpinner.activeValue;
		preferences.fullscreen = fullscreenSpinner.activeValue;
		preferences.antialiasing = antialiasingSpinner.activeValue;
		preferences.occlusion = occlusionSpinner.activeValue;
		preferences.bloom = bloomSpinner.activeValue;
		preferences.dof = depthOfFieldSpinner.activeValue;
		preferences.vignette = vignetteSpinner.activeValue;
		preferences.vsync = vSyncSpinner.activeValue;

		ApplyPreferences ();
	}

	public void ApplyPreferences(){
		//Performs changes in prefs
		string[] resolutionSplit = resolutionSpinner.values [preferences.resolution%resolutionSpinner.values.Count].Split ('x');
		int width = int.Parse(resolutionSplit[0]);
		int height = int.Parse(resolutionSplit[1]);
		bool setFullscreen = true;
		switch (preferences.fullscreen) {
		case 0:
			setFullscreen = false;
			break;
		case 1:
			setFullscreen = true;
			break;
		}
		Screen.SetResolution(width, height, setFullscreen);
		QualitySettings.vSyncCount = preferences.vsync;
		Camera camera = Camera.main;
		if (SceneManager.GetActiveScene ().buildIndex == 1) {
			//ANTIALIASING
			switch (preferences.antialiasing) {
			case 0:
				camera.GetComponent<Antialiasing>().enabled = false;
				break;
			case 1:
				camera.GetComponent<Antialiasing> ().enabled = true;
				camera.GetComponent<Antialiasing> ().mode = AAMode.SSAA;
				break;
			case 2 :
				camera.GetComponent<Antialiasing> ().enabled = true;
				camera.GetComponent<Antialiasing> ().mode = AAMode.FXAA2;
				break;
			}
			//OCCLUSION
			switch (preferences.occlusion) {
			case 0:
				camera.GetComponent<ScreenSpaceAmbientOcclusion> ().enabled = false;
				break;
			case 1:
				camera.GetComponent<ScreenSpaceAmbientOcclusion> ().enabled = true;
				break;
			}
			//BLOOM
			switch (preferences.bloom) {
			case 0:
				camera.GetComponent<Bloom> ().enabled = false;
				//camera.GetComponent<Tonemapping> ().enabled = false;
				break;
			case 1:
				camera.GetComponent<Bloom> ().enabled = true;
				//camera.GetComponent<Tonemapping> ().enabled = true;
				camera.GetComponent<Bloom> ().quality = Bloom.BloomQuality.Cheap;
				break;
			case 2:
				camera.GetComponent<Bloom> ().enabled = true;
				//camera.GetComponent<Tonemapping> ().enabled = true;
				camera.GetComponent<Bloom> ().quality = Bloom.BloomQuality.High;
				break;
			}
			//DEPTH OF FIELD
			switch (preferences.dof) {
			case 0:
				camera.GetComponent<DepthOfField> ().enabled = false;
				break;
			case 1:
				camera.GetComponent<DepthOfField> ().enabled = true;
				break;
			}
			//VIGNETTE
			switch (preferences.vignette) {
			case 0:
				camera.GetComponent<VignetteAndChromaticAberration> ().enabled = false;
				break;
			case 1:
				camera.GetComponent<VignetteAndChromaticAberration> ().enabled = true;
				break;
			}
		}
		SavePreferences ();
	}

	public void UpdateSpinners(){
		//Syncs values in spinners to actual stored values
		resolutionSpinner.activeValue = preferences.resolution;
		fullscreenSpinner.activeValue = preferences.fullscreen;
		antialiasingSpinner.activeValue = preferences.antialiasing;
		occlusionSpinner.activeValue = preferences.occlusion;
		bloomSpinner.activeValue = preferences.bloom;
		depthOfFieldSpinner.activeValue = preferences.dof;
		vignetteSpinner.activeValue = preferences.vignette;
		vSyncSpinner.activeValue = preferences.vsync;
	}

	//SCREENSHOT
	public void TakeScreenshot(){
		Application.CaptureScreenshot (Application.dataPath + "/Screenshot" + screenshotCounter.ToString("D3") + ".png");
		++screenshotCounter;
		StartCoroutine(DisplayTimedTooltip ("Screenshot has been saved into the game folder."));
	}

	//INITIALIZE
	void Awake () {
		resolutionSpinner.ClearOptions ();
		foreach (Resolution res in Screen.resolutions) {
			resolutionSpinner.AddOption(res.width + "x" + res.height);
		}
		LoadPreferences ();
		ApplyPreferences ();
		UpdateSpinners ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			TakeScreenshot ();
		}
	}
}
