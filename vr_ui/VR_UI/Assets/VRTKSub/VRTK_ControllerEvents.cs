using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTKSub {
	
	public class VRTK_ControllerEvents : MonoBehaviour {

		public enum ButtonAlias
		{
			Undefined,
			Trigger,
			TouchPad
		}

		public bool[] buttonPressed = new bool[] {false, false, false};

		// Use this for initialization
//		void Start () {
//
//		}

		// Update is called once per frame
		void Update () {
			if (Input.GetKeyDown(KeyCode.T)) {
				if (IsButtonPressed(ButtonAlias.TouchPad)) {
					Release(ButtonAlias.TouchPad);
				} else {
					Press(ButtonAlias.TouchPad);
				}
			}
			if (Input.GetKeyDown(KeyCode.P)) {
				StartCoroutine(ClickTrigger());
			}
		}


		IEnumerator ClickTrigger () {
			Press(ButtonAlias.Trigger);
			yield return null;
			yield return new WaitForSeconds(0.2f);
			Release(ButtonAlias.Trigger);
		}



		public bool IsButtonPressed(ButtonAlias button) {
			return buttonPressed[(int)button];
		}

		public Vector2 GetTouchpadAxis() {
			return Vector2.zero;
		}

		void Press(ButtonAlias button) {
			buttonPressed[(int)button] = true;
		}

		void Release(ButtonAlias button) {
			buttonPressed[(int)button] = false;
		}
	}

}