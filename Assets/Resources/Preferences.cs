using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Preferences {

	public int resolution;
	public int fullscreen;
	public int antialiasing;
	public int occlusion;
	public int bloom;
	public int dof;
	public int vignette;
	public int vsync;
	public int screenshotIndex;

	public Preferences(int resolutionOffset){
		resolution = resolutionOffset;
		fullscreen = 1;
		antialiasing = 0;
		occlusion = 0;
		bloom = 0;
		dof = 0;
		vignette = 0;
		vsync = 0;
		screenshotIndex = 0;
	}
}
