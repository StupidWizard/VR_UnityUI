using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTKSub;

public class DemoBase : MonoBehaviour {

	[SerializeField]
	VRTK_ControllerEvents controller;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.T)) {
			if (controller.IsButtonPressed(VRTKSub.VRTK_ControllerEvents.ButtonAlias.TouchPad)) {
				controller.Release(VRTKSub.VRTK_ControllerEvents.ButtonAlias.TouchPad);
			} else {
				controller.Press(VRTKSub.VRTK_ControllerEvents.ButtonAlias.TouchPad);
			}
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			StartCoroutine(controller.ClickTrigger());
		}
	}

	public void OnButtonClick() {
		Debug.LogError("OnButtonClick");
	}
}
