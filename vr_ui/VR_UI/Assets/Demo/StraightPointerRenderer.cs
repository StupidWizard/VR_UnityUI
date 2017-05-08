using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class StraightPointerRenderer : MonoBehaviour {

	protected const float BEAM_ADJUST_OFFSET = 0.00001f;

	[Tooltip("The layers for the pointer's raycasts to ignore.")]
	public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer;

	[Header("Straight Pointer Appearance Settings")]

	[Tooltip("The maximum length the pointer tracer can reach.")]
	public float maximumLength = 100f;
	[Tooltip("The scale factor to scale the pointer tracer object by.")]
	public float scaleFactor = 0.002f;
	[Tooltip("The scale multiplier to scale the pointer cursor object by in relation to the `Scale Factor`.")]
	public float cursorScaleMultiplier = 25f;

	[Tooltip("The cursor will be rotated to match the angle of the target surface if this is true, if it is false then the pointer cursor will always be horizontal.")]
	public bool cursorMatchTargetRotation = false;
	[Tooltip("Rescale the cursor proportionally to the distance from the tracer origin.")]
	public bool cursorDistanceRescale = false;

	[Tooltip("The colour to change the pointer materials when the pointer collides with a valid object. Set to `Color.clear` to bypass changing material colour on valid collision.")]
	public Color validCollisionColor = Color.green;
	[Tooltip("The colour to change the pointer materials when the pointer is not colliding with anything or with an invalid object. Set to `Color.clear` to bypass changing material colour on invalid collision.")]
	public Color invalidCollisionColor = Color.red;


	protected RaycastHit destinationHit = new RaycastHit();

	protected GameObject actualContainer;
	protected GameObject actualTracer;
	protected GameObject actualCursor;

	protected Color currentColor;

	protected Vector3 cursorOriginalScale = Vector3.one;

	[SerializeField]
	Transform originPoint;

	[SerializeField]
	Material defaultMaterial;

	// Use this for initialization
	void Start () {
		CreatePointerObjects();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRenderer();
	}


	public void UpdateRenderer()
	{
		if (IsVisible())
		{
			float tracerLength = CastRayForward();
			SetPointerAppearance(tracerLength);
		}
	}


	protected void CreatePointerObjects()
	{
		actualContainer = new GameObject(string.Format("[{0}]StraightPointerRenderer_Container", gameObject.name));
		actualContainer.transform.localPosition = Vector3.zero;
		VRTK_PlayerObject.SetPlayerObject(actualContainer, VRTK_PlayerObject.ObjectTypes.Pointer);

		CreateTracer();
		CreateCursor();
		Toggle(false, false);
	}




	protected void CreateTracer()
	{
		actualTracer = GameObject.CreatePrimitive(PrimitiveType.Cube);
		actualTracer.GetComponent<BoxCollider>().isTrigger = true;
		actualTracer.AddComponent<Rigidbody>().isKinematic = true;
		actualTracer.layer = LayerMask.NameToLayer("Ignore Raycast");

		SetupMaterialRenderer(actualTracer);

		actualTracer.transform.name = string.Format("[{0}]StraightPointerRenderer_Tracer", gameObject.name);
		actualTracer.transform.SetParent(actualContainer.transform);

		VRTK_PlayerObject.SetPlayerObject(actualTracer, VRTK_PlayerObject.ObjectTypes.Pointer);
	}

	protected void CreateCursor()
	{
		actualCursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		actualCursor.transform.localScale = Vector3.one * (scaleFactor * cursorScaleMultiplier);
		actualCursor.GetComponent<Collider>().isTrigger = true;
		actualCursor.AddComponent<Rigidbody>().isKinematic = true;
		actualCursor.layer = LayerMask.NameToLayer("Ignore Raycast");

		SetupMaterialRenderer(actualCursor);

		cursorOriginalScale = actualCursor.transform.localScale;
		actualCursor.transform.name = string.Format("[{0}]StraightPointerRenderer_Cursor", gameObject.name);
		actualCursor.transform.SetParent(actualContainer.transform);
		VRTK_PlayerObject.SetPlayerObject(actualCursor, VRTK_PlayerObject.ObjectTypes.Pointer);
	}


	protected void SetupMaterialRenderer(GameObject givenObject)
	{
		if (givenObject)
		{
			var pointerRenderer = givenObject.GetComponent<MeshRenderer>();
			pointerRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			pointerRenderer.receiveShadows = false;
			pointerRenderer.material = defaultMaterial;
		}
	}


	public void Toggle(bool pointerState, bool actualState)
	{
		// TODO - on off renderer
	}


	public bool logRayHit;
	protected float CastRayForward()
	{
		Transform origin = GetOrigin();
		Ray pointerRaycast = new Ray(origin.position, origin.forward);
		RaycastHit pointerCollidedWith;
		var rayHit = Physics.Raycast(pointerRaycast, out pointerCollidedWith, maximumLength, ~layersToIgnore);

		logRayHit = rayHit;
		CheckRayMiss(rayHit, pointerCollidedWith);
		CheckRayHit(rayHit, pointerCollidedWith);

		float actualLength = maximumLength;
		if (rayHit && pointerCollidedWith.distance < maximumLength)
		{
			actualLength = pointerCollidedWith.distance;
		}

		return actualLength;
	}

	protected void CheckRayMiss(bool rayHit, RaycastHit pointerCollidedWith)
	{
		if (!rayHit || (destinationHit.collider && destinationHit.collider != pointerCollidedWith.collider))
		{
			if (destinationHit.collider != null)
			{
//				PointerExit(destinationHit);
			}

			destinationHit = new RaycastHit();
			ChangeColor(invalidCollisionColor);
		}
	}

	protected void CheckRayHit(bool rayHit, RaycastHit pointerCollidedWith)
	{
		if (rayHit)
		{
//			PointerEnter(pointerCollidedWith);

			destinationHit = pointerCollidedWith;
			ChangeColor(validCollisionColor);
		}
	}


	protected void ChangeColor(Color givenColor)
	{
		if (givenColor != Color.clear)
		{
			currentColor = givenColor;
		}

		ChangeMaterialColor(actualTracer, currentColor);
		ChangeMaterialColor(actualCursor, currentColor);
	}

	protected virtual void ChangeMaterialColor(GameObject givenObject, Color givenColor)
	{
		if (givenObject)
		{
			Renderer[] foundRenderers = givenObject.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < foundRenderers.Length; i++)
			{
				Renderer foundRenderer = foundRenderers[i];
				if (foundRenderer.material)
				{
					foundRenderer.material.EnableKeyword("_EMISSION");

					if (foundRenderer.material.HasProperty("_Color"))
					{
						foundRenderer.material.color = givenColor;
					}

//					if (foundRenderer.material.HasProperty("_EmissionColor"))
//					{
//						foundRenderer.material.SetColor("_EmissionColor", VRTK_SharedMethods.ColorDarken(givenColor, 50));
//					}
				}
			}
		}
	}



	protected Transform GetOrigin()
	{
		return originPoint;
	}


	public virtual bool IsVisible()
	{
		return actualContainer.activeInHierarchy;
	}







	protected virtual void SetPointerAppearance(float tracerLength)
	{
		if (!actualContainer) {
			return;
		}
		//if the additional decimal isn't added then the beam position glitches
		var beamPosition = tracerLength / (2f + BEAM_ADJUST_OFFSET);

		actualTracer.transform.localScale = new Vector3(scaleFactor, scaleFactor, tracerLength);
		actualTracer.transform.localPosition = Vector3.forward * beamPosition;
		actualCursor.transform.localScale = Vector3.one * (scaleFactor * cursorScaleMultiplier);
		actualCursor.transform.localPosition = new Vector3(0f, 0f, tracerLength);

		Transform origin = GetOrigin();
		actualContainer.transform.position = origin.position;
		actualContainer.transform.rotation = origin.rotation;

//		ScaleObjectInteractor(actualCursor.transform.localScale * 1.05f);

		if (destinationHit.transform)
		{
			if (cursorMatchTargetRotation)
			{
				actualCursor.transform.forward = -destinationHit.normal;
			}
			if (cursorDistanceRescale)
			{
				float collisionDistance = Vector3.Distance(destinationHit.point, origin.position);
				actualCursor.transform.localScale = cursorOriginalScale * collisionDistance;
			}
		}
		else
		{
			if (cursorMatchTargetRotation)
			{
				actualCursor.transform.forward = origin.forward;
			}
			if (cursorDistanceRescale)
			{
				actualCursor.transform.localScale = cursorOriginalScale * tracerLength;
			}
		}

//		ToggleRenderer(controllingPointer.IsPointerActive(), false);
//		UpdateDependencies(actualCursor.transform.position);
	}


	protected void ToggleRenderer(bool pointerState, bool actualState)
	{
		// TODO 
//		ToggleElement(actualTracer, pointerState, actualState, tracerVisibility, ref tracerVisible);
//		ToggleElement(actualCursor, pointerState, actualState, cursorVisibility, ref cursorVisible);
	}
}
