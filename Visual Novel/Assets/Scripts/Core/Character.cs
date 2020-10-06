using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

[System.Serializable]
public class Character
{
	#region Variables
	public string characterName;
	/// <summary>
	/// The space between the anchors of this character. Defines how much space a character takes up on the screen.
	/// </summary>
	public Vector2 anchorPadding { get { return root.anchorMax - root.anchorMin; } }

	/// <summary>
	/// The root is the container for all images related to the character in the scene. The root object.
	/// </summary>
	[HideInInspector] public RectTransform root;

	public bool isMultiLayerCharacter { get { return renderers.renderer == null; } }

	public bool enabled { get { return root.gameObject.activeInHierarchy; } set { root.gameObject.SetActive(value); } }

	#endregion

	#region Unity Methods

	private DialogueSystem dialogue;
	/// <summary>
	/// Make this character say something.
	/// </summary>
	/// <param name="speech"></param>
	public void Say(string speech, bool add = false)
	{
		if (!enabled)
			enabled = true;
		if (!add)
			dialogue.Say(speech, false, characterName);
		else
			dialogue.SayAdd(speech, characterName);
	}

	Vector2 targetPosition;
	Coroutine moving;
	bool isMoving { get { return moving != null; } }
	public void MoveTo(Vector2 Target, float speed, bool smooth = true)
	{
		//if we are moving stop moving
		StopMoving();
		moving = CharacterManager.instance.StartCoroutine(Moving(Target, speed, smooth));

	}

	public void StopMoving(bool arriveAtTargetPositionImmediately = false)
	{
		if (isMoving)
		{
			CharacterManager.instance.StopCoroutine(moving);
			if (arriveAtTargetPositionImmediately)
				SetPosition(targetPosition);

		}
			moving = null;

	}
	/// <summary>
	/// Immediately set the position of the character
	/// </summary>
	/// <param name="target"></param>
	public void SetPosition(Vector2 target) {
		Vector2 padding = anchorPadding;
		float maxX = 1f - padding.x;
		float maxY = 1f - padding.y;
		Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
		root.anchorMin = minAnchorTarget;
		root.anchorMax = root.anchorMin + padding;
	}

	IEnumerator Moving(Vector2 target, float speed, bool smooth)
	{
		targetPosition = target;

		//now we want to get the padding for our character
		Vector2 padding = anchorPadding;
		//now get the limitations for 0 to 100% movement. The farthest a character can move to the right before 100% should be the 1 value
		float maxX = 1f -padding.x;
		float maxY = 1f -padding.y;

		//now get the actal position target for the minimum anchors(left/bottom bounds) fo the character. because maxX and maxY is just a percentage.
		Vector2 minAnchorTarget = new Vector2(maxX * targetPosition.x, maxY * targetPosition.y);
		//move until we reach end of frame
		speed *= Time.deltaTime;
		while(root.anchorMin != minAnchorTarget)
		{
			root.anchorMin = (!smooth) ? Vector2.MoveTowards(root.anchorMin, minAnchorTarget, speed) : Vector2.Lerp(root.anchorMin, minAnchorTarget, speed);
			root.anchorMax = root.anchorMin + padding;
			yield return new WaitForEndOfFrame();
		}
		StopMoving();
	}



	//Begin transitioning image -- only if we have a texture file we are grabbing multiple sprites from\\\\\\\\\
	public Sprite GetSprite(int index = 0) {
		Sprite[] sprites = Resources.LoadAll<Sprite>("images/character/" + characterName);
		Debug.Log(sprites.Length);
		Debug.Log(sprites[index]);
		return sprites[index];
	}	
	
	public Sprite GetSprite(string type) {
		Sprite sprite = Resources.Load<Sprite>("/images/character/" + characterName + "/" + type);
		//Debug.Log("images/character/" + characterName + "/" + type);
		Debug.Log(sprite);
		return sprite;
	}
	public void SetBody(int index) {
		renderers.bodyRenderer.sprite = GetSprite(index);
	}
	public void SetBody(Sprite sprite) {
		renderers.bodyRenderer.sprite = sprite;
	}	
	public void SetBody(string  type) {
		renderers.bodyRenderer.sprite = GetSprite(type);
	}


	public void SetExpression(int index) {
		renderers.expressionRenderer.sprite = GetSprite(index);
	}
	public void SetExpression(Sprite sprite) {
		renderers.expressionRenderer.sprite = sprite;
	}	
	public void SetExpression(string type) {
		renderers.expressionRenderer.sprite = GetSprite(type);
	}

	bool isTransitioningBody { get { return transitioningBody != null; } }
	Coroutine transitioningBody = null;

	public void TransitionBody(Sprite sprite, float speed, bool smooth) {
		if (renderers.bodyRenderer.sprite == sprite)
			return;
		StopTransitioningBody();
		transitioningBody = CharacterManager.instance.StartCoroutine(TransitioningBody(sprite, speed, smooth));
	}

	void StopTransitioningBody()
	{
		if (isTransitioningBody)
		{
			CharacterManager.instance.StopCoroutine(transitioningBody);

		}
		transitioningBody = null;

	}

	public IEnumerator TransitioningBody(Sprite sprite, float speed, bool smooth) 
	{
		for (int i = 0; i < renderers.allBodyRenderers.Count; i++) {
			Image image = renderers.allBodyRenderers[i];
			if (image.sprite == sprite) {
				renderers.bodyRenderer = image;
				break;
			}
		}

		if (renderers.bodyRenderer.sprite != sprite) {
			Image image = GameObject.Instantiate(renderers.bodyRenderer.gameObject, renderers.bodyRenderer.transform.parent).GetComponent<Image>();
			renderers.allBodyRenderers.Add(image);
			renderers.bodyRenderer = image;
			image.color = GlobalF.SetAlpha(image.color, 0f);
			image.sprite = sprite;
		}

		while(GlobalF.TransitionImages(ref renderers.bodyRenderer, ref renderers.allBodyRenderers, speed, smooth))
		{
			Debug.Log("done");

			yield return new WaitForEndOfFrame();
		}
		StopTransitioningBody();
	}


	bool isTransitioningExpression { get { return transitioningExpression != null; } }
	Coroutine transitioningExpression = null;

	public void TransitionExpression(Sprite sprite, float speed, bool smooth)
	{
		if (renderers.expressionRenderer.sprite == sprite)
			return;
		StopTransitioningExpression();
		transitioningExpression = CharacterManager.instance.StartCoroutine(TransitioningExpression(sprite, speed, smooth));
	}

	void StopTransitioningExpression()
	{
		if (isTransitioningExpression)
		{
			CharacterManager.instance.StopCoroutine(transitioningExpression);

		}
		transitioningExpression = null;

	}

	public IEnumerator TransitioningExpression(Sprite sprite, float speed, bool smooth)
	{
		for (int i = 0; i < renderers.allExpressionRenderers.Count; i++)
		{
			Image image = renderers.allExpressionRenderers[i];
			if (image.sprite == sprite)
			{
				renderers.expressionRenderer = image;
				break;
			}
		}

		if (renderers.expressionRenderer.sprite != sprite)
		{
			Image image = GameObject.Instantiate(renderers.expressionRenderer.gameObject, renderers.expressionRenderer.transform.parent).GetComponent<Image>();
			renderers.allExpressionRenderers.Add(image);
			renderers.expressionRenderer = image;
			image.color = GlobalF.SetAlpha(image.color, 0f);
			image.sprite = sprite;
		}

		while (GlobalF.TransitionImages(ref renderers.expressionRenderer, ref renderers.allExpressionRenderers, speed, smooth))
		{

			yield return new WaitForEndOfFrame();
		}
		StopTransitioningExpression();
	}


	//end transition image\\\\\\\\\\\\\\\\\\\\\\\\



	/// <summary>
	/// Create a new character
	/// </summary>
	/// <param name="_name"></param>
	public Character(string _name, bool enabledOnStart = true)
	{
		CharacterManager cm = CharacterManager.instance;
		//locate the character prefab
		GameObject prefab = Resources.Load("Character/Character_" + _name) as GameObject;
		//spawn an instance of the prefab directly on the character panel
		GameObject ob = GameObject.Instantiate(prefab, cm.characterPanel);

		root = ob.GetComponent<RectTransform>();
		characterName = _name;


		//get the renderer(s) 
		renderers.renderer = ob.GetComponentInChildren<RawImage>();
		if (isMultiLayerCharacter)
		{
			renderers.bodyRenderer = ob.transform.Find("bodyLayer").GetComponentInChildren<Image>();
			renderers.expressionRenderer = ob.transform.Find("expressionLayer").GetComponentInChildren<Image>();
			renderers.allBodyRenderers.Add(renderers.bodyRenderer);
			renderers.allExpressionRenderers.Add(renderers.expressionRenderer);

		}

		dialogue = DialogueSystem.instance;

		enabled = enabledOnStart;

	}

	[System.Serializable]
	public class Renderers
	{
		/// <summary>
		/// Used as the only image for a single layer character
		/// </summary>
		public RawImage renderer;
		/// <summary>
		/// The body renderer for a multi layer character
		/// </summary>
		public Image bodyRenderer;
		/// <summary>
		/// The expression renderer for a multi layer character
		/// </summary>
		public Image expressionRenderer;

		public List<Image> allBodyRenderers = new List<Image>();
		public List<Image> allExpressionRenderers = new List<Image>();

	}
	public Renderers renderers = new Renderers();



	#endregion
}
