using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpinnerHorizontal : MonoBehaviour {

	public List<string> values;
	public int activeValue;
	public Text label;
	public MenuManager manager;

	public void SpinLeft(){
		activeValue = (values.Count + activeValue - 1) % values.Count;
		UpdateLabel ();
	}

	public void SpinRight(){
		activeValue = (values.Count + activeValue + 1) % values.Count;
		UpdateLabel ();
	}

	public void UpdateLabel(){
		label.text = values [activeValue];
	}

	public void SetActiveOption(int index){
		activeValue = index;
		UpdateLabel ();
	}

	public void ClearOptions(){
		values.Clear ();
	}

	public void AddOption(string option){
		values.Add (option);
	}

	public void AddOptions(List<string> options){
		values.AddRange (options);
	}

	void Awake(){
		UpdateLabel ();
	}

	void OnGUI(){
		UpdateLabel ();
	}
}
