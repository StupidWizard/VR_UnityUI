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

		// Use this for initialization
//		void Start () {
//
//		}

		// Update is called once per frame
//		void Update () {
//
//		}

		public bool IsButtonPressed(ButtonAlias button) {
			return true;
		}

		public Vector2 GetTouchpadAxis() {
			return Vector2.zero;
		}
	}

}