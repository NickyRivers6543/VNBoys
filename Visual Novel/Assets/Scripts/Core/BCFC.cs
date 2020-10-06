using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BCFC : MonoBehaviour
{



	#region Variables

	public static BCFC instance;

	public LAYER background = new LAYER();
	public LAYER cinematic = new LAYER();
	public LAYER foreground = new LAYER();


	#endregion

	#region Unity Methods

	public void Awake()
	{
		instance = this;
	}

	[System.Serializable]
	public class LAYER
	{
		public GameObject root;
		public GameObject newImageObjectReference;

		public RawImage activeImage;
		public List<RawImage> allImages = new List<RawImage>();




		public void setTexture(Texture texture, bool ifMovieThenLoop = true)
		{
			if(activeImage != null && activeImage.texture != null)
			{
				VideoPlayer mov = new VideoPlayer();
				mov.targetTexture = (RenderTexture)texture;

				if (mov.texture != null)
				{					
					mov.Stop();
				}
			}

			if(texture != null)
			{
				if (activeImage == null)
					createNewActiveImage();
				activeImage.texture = texture;
				activeImage.color = GlobalF.SetAlpha(activeImage.color, 1f);

				VideoPlayer mov = new VideoPlayer();
				mov.targetTexture = (RenderTexture)texture;
				
				if (mov.texture != null) {

					mov.isLooping = ifMovieThenLoop;
					mov.Play();
				}
			}
			else
			{
				if(activeImage != null)
				{
					allImages.Remove(activeImage);
					GameObject.DestroyImmediate(activeImage.gameObject);
					activeImage = null;
				}
			}
		}

		public void createNewActiveImage()
		{

			GameObject ob = Instantiate(newImageObjectReference, root.transform) as GameObject;
			ob.SetActive(true);
			RawImage raw = ob.GetComponent<RawImage>();
			activeImage = raw;
			allImages.Add(raw);
		}

		public void TransitionToTexture(Texture texture, float speed = 2f, bool smooth = false, bool ifMovieThenLoop = true) {
			if (activeImage != null && activeImage.texture == texture) {
				return;
			}
			
			StopTransitioning();
			transitioning = BCFC.instance.StartCoroutine(Transitioning(texture, speed, smooth, ifMovieThenLoop));
		}

		void StopTransitioning() {
			if (isTransitioning)
			{
				BCFC.instance.StopCoroutine(transitioning);

			}
			transitioning = null;
		}

		public bool isTransitioning { get { return transitioning != null; } }
		Coroutine transitioning = null;
		IEnumerator Transitioning(Texture texture, float speed = 2f, bool smooth = false, bool ifMovieThenLoop = true)
		{

			if (texture != null)
			{
				for (int i = 0; i < allImages.Count; i++)
				{
					RawImage image = allImages[i];
					if (image.texture == texture)
					{
						activeImage = image;
						break;
					}
				}
				if (activeImage == null || activeImage.texture != texture)
				{
					createNewActiveImage();
					activeImage.texture = texture;
					activeImage.color = GlobalF.SetAlpha(activeImage.color, 0f);

					//VideoPlayer mov = new VideoPlayer();
					//mov.targetTexture = (RenderTexture)texture;

					//if (mov != null)
					//{

					//	mov.isLooping = ifMovieThenLoop;
					//	mov.Play();
					//}
				}
			}
			else {
				activeImage = null;
			}
			while (GlobalF.TransitionRawImages(ref activeImage, ref allImages, speed, smooth))
				yield return new WaitForEndOfFrame();

			StopTransitioning();
		}

	}

	#endregion
}
