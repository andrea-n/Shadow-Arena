using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour {

	public Canvas pauseMenu;
	public Canvas regularUI;
	public List<Dropdown.OptionData> availableResolutions;
	public Dropdown resolutionControl;
	public Toggle fullscreenControl;
	public Toggle antialiasingControl;
	public Toggle bloomControl;
	public Toggle depthOfFieldControl;
	public Toggle ambientOcclusionControl;
	public Toggle postprocessingControl;
	private static Settings settings;
	private bool isPaused = false;

	public void LoadSettings(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.dataPath + "/options.gd", FileMode.Open);
		settings = (Settings)bf.Deserialize (file);
		file.Close ();
		ApplySettings ();
		UpdateSettings ();
	}

	public void ApplySettings(){
		Screen.SetResolution (settings.width, settings.height, settings.fullscreen);
		Camera.main.GetComponent<Antialiasing> ().enabled = settings.antialiasing;
		Camera.main.GetComponent<BloomOptimized> ().enabled = settings.bloom;
		Camera.main.GetComponent<DepthOfField> ().enabled = settings.depthOfField;
		Camera.main.GetComponent<ScreenSpaceAmbientOcclusion> ().enabled = settings.ambientOcclusion;
		Camera.main.GetComponent<VignetteAndChromaticAberration> ().enabled = settings.postprocessing;
		Camera.main.GetComponent<NoiseAndGrain> ().enabled = settings.postprocessing;
		SaveSettings ();
	}

	public void SaveSettings(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create(Application.dataPath + "/options.gd");
		bf.Serialize (file, settings);
		file.Close ();
	}

	public void Screenshot(){
		Application.CaptureScreenshot(Application.dataPath + "/Screenshot-"+settings.screenshotIndex.ToString("D3")+".png");
		++settings.screenshotIndex;
	}

	public void TogglePause(){
		isPaused = !isPaused;
		pauseMenu.gameObject.SetActive (isPaused);
		regularUI.gameObject.SetActive (!isPaused);
		//Camera.main.gameObject.GetComponent<Blur> ().enabled = isPaused;
		switch (isPaused) {
		case true:
			Time.timeScale = 0f;
			break;
		case false:
			Time.timeScale = 1.0f;
			break;
		}
	}

	public void QuitToMenu(){
		SceneManager.LoadScene ("menu", LoadSceneMode.Single);
	}

	public void QuitToDesktop(){
		Application.Quit ();
	}

	public void UpdateSettings(){
		resolutionControl.value = settings.resolution;
		fullscreenControl.isOn = settings.fullscreen;
		antialiasingControl.isOn = settings.antialiasing;
		bloomControl.isOn = settings.bloom;
		depthOfFieldControl.isOn = settings.depthOfField;
		ambientOcclusionControl.isOn = settings.ambientOcclusion;
		postprocessingControl.isOn = settings.postprocessing;
	}

	public void SetResolution(Dropdown dropdown){
		string[] resolution = dropdown.options[dropdown.value].text.Split('x');
		settings.width = int.Parse(resolution [0]);
		settings.height = int.Parse(resolution [1]);
		settings.resolution = dropdown.value;
		//ApplySettings ();
	}

	public void ToggleFullscreen(bool state){
		settings.fullscreen = state;
		//ApplySettings ();
	}

	public void ToggleAntialiasing(bool state){
		settings.antialiasing = state;
		//ApplySettings ();
	}

	public void ToggleBloom(bool state){
		settings.bloom = state;
		//ApplySettings ();
	}

	public void ToggleDepthOfField(bool state){
		settings.depthOfField = state;
		//ApplySettings ();
	}

	public void ToggleSSAO(bool state){
		settings.ambientOcclusion = state;
		//ApplySettings ();
	}

	public void TogglePostProcessing(bool state){
		settings.postprocessing = state;
		//ApplySettings ();
	}

	// Use this for initialization
	void Start () {
		LoadSettings ();
		resolutionControl.ClearOptions ();
		foreach (Resolution res in Screen.resolutions) {
			Dropdown.OptionData resolutionOption = new Dropdown.OptionData ();
			resolutionOption.text = res.width + "x" + res.height;
			availableResolutions.Add (resolutionOption);
		}
		resolutionControl.AddOptions(availableResolutions);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			Screenshot ();
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			TogglePause ();
		}
	}
}
