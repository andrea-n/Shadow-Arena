using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

[Serializable]
public class Settings{
	public int width;
	public int height;
	public bool fullscreen;
	public int resolution;
	public bool antialiasing;
	public bool bloom;
	public bool depthOfField;
	public bool ambientOcclusion;
	public bool postprocessing;
	public int screenshotIndex;

	public string defaultResolution = Screen.currentResolution.width.ToString() + "x" + Screen.currentResolution.height.ToString();

	/*public Settings(){
		width = Screen.currentResolution.width;
		height = Screen.currentResolution.height;
		fullscreen = true;
		resolution = -1;
		antialiasing = true;
		bloom = true;
		depthOfField = true;
		screenshotIndex = 0;
	}*/

	public Settings(Dropdown dropdown){
		width = Screen.currentResolution.width;
		height = Screen.currentResolution.height;
		fullscreen = true;
		for(int i=0; i<Screen.resolutions.GetLength(0); i++){
			if(dropdown.options[i].text == defaultResolution){
				resolution = i;
			}
		}
		antialiasing = true;
		bloom = true;
		depthOfField = true;
		ambientOcclusion = true;
		postprocessing = true;
		screenshotIndex = 0;
	}
}
