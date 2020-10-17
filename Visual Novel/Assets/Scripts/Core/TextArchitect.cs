using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextArchitect : MonoBehaviour
{
	#region Variables
	/// <summary>
	/// The current text built by the text architect up to this point in time.
	/// </summary>
	public string currentText { get { return _currentText; } }
	private string _currentText = "";

	private string preText = "";
	private string targetText = "";

	private int charactersPerFrame = 1;
	[Range(1f, 60f)]
	private float speed = 1f;
	private bool useEncapsulation = true;

	public bool skip = false;

	private bool isTMPro = false;

	public bool isConstructing { get { return buildProcess != null; } }
	Coroutine buildProcess = null;
	#endregion


	#region Unity Methods

	public TextArchitect(string targetText, string preText = "", int charactersPerFrame = 1, float speed = 1f, bool useEncapsulation = true, bool isTMPro = true)
	{
		this.targetText = targetText;
		this.preText = preText;
		this.charactersPerFrame = charactersPerFrame;
		this.speed = speed;
		this.useEncapsulation = useEncapsulation;
		this.isTMPro = isTMPro;

		buildProcess = DialogueSystem.instance.StartCoroutine(Construction());
	}

	public void Stop()
	{
		if (isConstructing) {
			DialogueSystem.instance.StopCoroutine(buildProcess);
		}

		buildProcess = null;
	}


	IEnumerator Construction() {
		int runsThisFrame = 0;
		string[] speechAndTags = useEncapsulation ? TagManager.SplitByTags(targetText) : new string[1] { targetText };

		//Is this additive, make sure we include additive text
		_currentText = preText;
		//make a storage for the building text so we don't change what is already made. so we can sandwich it between tags.
		string curText = "";

		//build the text by moving through each part

		for (int a = 0; a < speechAndTags.Length; a++) {
			string section = speechAndTags[a];
			//tags will always be odd indexed
			bool isATag = (a & 1) != 0; //Sexy bitwise manipulation, this guy knows his stuff

			if(isATag && useEncapsulation)
			{

				if (!isTMPro)
				{

					curText = _currentText;
					ENCAPSULATED_TEXT encapsulation = new ENCAPSULATED_TEXT(string.Format("<{0}>", section), speechAndTags, a);
					while (!encapsulation.isDone)
					{
						bool stepped = encapsulation.Step();

						_currentText = curText + encapsulation.displayText;
						//Only yield if a step was taken in building the string 
						if (stepped)
						{
							runsThisFrame++;
							int maxRunsPerFrame = skip ? 5 : charactersPerFrame;
							if (runsThisFrame == maxRunsPerFrame)
							{
								runsThisFrame = 0;
								yield return new WaitForSeconds(skip ? 0.01f : 0.01f * speed);
							}
						}
					}
					a = encapsulation.speechAndTagsArrayProgress + 1;

				}
				else {
					string tag = string.Format("<{0}>", section);
					_currentText += tag;
					yield return new WaitForEndOfFrame();
				}

				//Increment by one to bypass the text that was used in the encapsulation
			}
			else //not a tag or not using encap. build like regular text.
			{
				for(int i = 0; i < section.Length; i++)
				{
					_currentText += section[i];

					runsThisFrame++;
					int maxRunsPerFrame = skip ? 5 : charactersPerFrame;
					if(runsThisFrame == maxRunsPerFrame)
					{
						runsThisFrame = 0;
						yield return new WaitForSeconds(skip ? 0.01f : 0.01f * speed);
					}
				}
			}
		}
		buildProcess = null;
	}

	private class ENCAPSULATED_TEXT
	{
		//tag precedes text, ending tag trails it.
		private string tag = "";
		private string endingTag = "";
		//current text is the currently built target text without tags. target text is the build target.
		private string currentText = "";
		private string targetText = "";

		public string displayText { get { return _displayText; } }
		private string _displayText = "";

		//contains elements that the encapsulator will attempt to advance to when searcing for sub encapsulators
		private string[] allSpeechAndTagsArray;
		public int speechAndTagsArrayProgress { get { return arrayProgress; } }
		private int arrayProgress = 0;

		public bool isDone {get{return _isDone;} }
		private bool _isDone = false;

		public ENCAPSULATED_TEXT encapsulator = null;
		public ENCAPSULATED_TEXT subEncapsulator = null;

		public ENCAPSULATED_TEXT(string tag, string[] allSpeechAndTagsArray, int arrayProgress) {
			this.tag = tag;
			GenerateEndingTag();
			this.allSpeechAndTagsArray = allSpeechAndTagsArray;
			this.arrayProgress = arrayProgress;

			if (allSpeechAndTagsArray.Length - 1 > arrayProgress) {
				string nextPart = allSpeechAndTagsArray[arrayProgress + 1];
				bool isATag = ((arrayProgress + 1) & 1) != 0;

				//IF the next writable part is not a tag, make it the target a this encapsulator
				if (!isATag)
					targetText = nextPart;
				//IF the next writable part is a tag, add a sub encapsulator to take care of it.
				else
				{
					//Obsolete
					//subEncapsulator = new ENCAPSULATED_TEXT(string.Format("<{0}>", nextPart), allSpeechAndTagsArray, arrayProgress + 1);
				}
				//increment progress so the next attempted part is updated
				this.arrayProgress++;
			}
		}

			void GenerateEndingTag() {
				endingTag = tag.Replace("<", "").Replace(">", "");
				if (endingTag.Contains("="))
				{
					endingTag = string.Format("</{0}>", endingTag.Split('=')[0]);
				}
				else {
					endingTag = string.Format("</{0}>", endingTag);
				}
			}

			
			///<summary>
			/// Take the next step in the construction rocess. returns true if a step was taken. returns false if a step must be made from a lower level encapsulator.
			/// </summary>
			public bool Step() {
				//a completed encapsulation should not step any further. Return true so if there is an error, yielding may occur
				if (isDone)
					return true;
				if (subEncapsulator != null && !subEncapsulator.isDone)
				{
					return subEncapsulator.Step();
				}
				//this encapsulator needs to finish its text
				else {
					//this encapsulator has reached the end of its text
					if (currentText == targetText)
					{
						//If there is still more dialogue to build
						if (allSpeechAndTagsArray.Length > arrayProgress + 1)
						{
							string nextPart = allSpeechAndTagsArray[arrayProgress + 1];
							bool isATag = ((arrayProgress + 1) & 1) != 0;
							if (isATag)
							{
								//if the tag we have just reached is the terminator for this encapsulator, close it
								if (string.Format("<{0}>", nextPart) == endingTag)
								{
									_isDone = true;

									if (encapsulator != null)
									{
										string taggedText = tag + currentText + endingTag;
										encapsulator.currentText += taggedText;
										encapsulator.targetText += taggedText;

										//update array progress to get past the current text AND the ending tag. +2
										UpdateArrayProgress(2);
									}
								}
								//If the tag we reached is not the terminator for this encapsulator, then a sub encapsulator must be created.
								else {
									subEncapsulator = new ENCAPSULATED_TEXT(string.Format("<{0}>", nextPart), allSpeechAndTagsArray, arrayProgress + 1);
									subEncapsulator.encapsulator = this;

									//have the encapsulators keep up with the current progress
									UpdateArrayProgress();

								}
							}
							//If the next part is not a tag, then this is an extension to be added to the encapsulator's target.
							else {
								targetText += nextPart;
								UpdateArrayProgress();
							}
						}
						//Finished dialogue. Close
						else {
							_isDone = true;
						}
					}
					//if there is still more to build
					else {
						currentText += targetText[currentText.Length];
						//update the display text. which means we have to update any encapsulators if this is a sub encapsulator
						UpdateDisplay("");

						return true;
					
					}
				}
				return false;
			}

			

		
		void UpdateArrayProgress(int val = 1)
		{
			arrayProgress += val;

			if (encapsulator != null)
				encapsulator.UpdateArrayProgress(val);
		}


		void UpdateDisplay(string subValue)
		{
			_displayText = string.Format("{0}{1}{2}{3}", tag, currentText, subValue, endingTag);
			if (encapsulator != null)
			{
				encapsulator.UpdateDisplay(displayText);
			}
		}
	}
#endregion
}
