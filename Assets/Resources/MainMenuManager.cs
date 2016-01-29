using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour {

	private static Settings settings;

	public string mainScene = "";

	public List<Dropdown.OptionData> availableResolutions;
	public Dropdown resolutionControl;
	public Toggle fullscreenControl;
	public Toggle antialiasingControl;
	public Toggle bloomControl;
	public Toggle depthOfFieldControl;
	public Toggle ambientOcclusionControl;
	public Toggle postprocessingControl;

	public void QuitGame(){
		Application.Quit ();
	}

	public void StartGame(){
		SceneManager.LoadScene(mainScene, LoadSceneMode.Single);
	}

	public void DisplayTooltip(GameObject tooltip){
		tooltip.SetActive (true);
	}

	public void HideTooltip(GameObject tooltip){
		tooltip.SetActive (false);
	}

	public void Screenshot(){
		Application.CaptureScreenshot(Application.dataPath + "/Screenshot-"+settings.screenshotIndex.ToString("D3")+".png");
		++settings.screenshotIndex;
	}

	public void SaveSettings(){
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create(Application.dataPath + "/options.gd");
		bf.Serialize (file, settings);
		file.Close ();
	}

	public void LoadSettings(){
		if (File.Exists (Application.dataPath + "/options.gd")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.dataPath + "/options.gd", FileMode.Open);
			settings = (Settings)bf.Deserialize (file);
			file.Close ();
		} else {
			settings = new Settings (resolutionControl);
		}
		ApplySettings ();
	}

	public void ApplySettings(){
		Screen.SetResolution (settings.width, settings.height, settings.fullscreen);
		SaveSettings ();
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

	void Start(){
		resolutionControl.ClearOptions ();
		foreach (Resolution res in Screen.resolutions) {
			Dropdown.OptionData resolutionOption = new Dropdown.OptionData ();
			resolutionOption.text = res.width + "x" + res.height;
			availableResolutions.Add (resolutionOption);
		}
		resolutionControl.AddOptions(availableResolutions);
		LoadSettings ();
		UpdateSettings ();
	}

	void Update(){
		if (Input.GetKeyDown (KeyCode.P)) {
			Screenshot ();
		}
	}
}
