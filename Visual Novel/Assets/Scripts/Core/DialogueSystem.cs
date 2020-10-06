using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSystem : MonoBehaviour
{
    #region Variables
    #endregion
    string targetSpeech = "";
    #region Unity Methods

    public static DialogueSystem instance;
    public ELEMENTS elements;
    private void Awake()
    {
        instance = this;
    }




    /// <summary>
    /// Say something and show it on the speech box
    /// </summary>
    public void Say(string speech, bool additive = false, string speaker = "")
    {
        StopSpeaking();
        speaking = StartCoroutine(Speaking(speech, additive, speaker));
    }
    public void SayAdd(string speech, string speaker = "") {
        StopSpeaking();
        speechText.text = targetSpeech;
        speaking = StartCoroutine(Speaking(speech, true, speaker));
    }
    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }
        if (textArchitect != null && textArchitect.isConstructing) {
            textArchitect.Stop();
        }


        speaking = null;
    }

    public bool isSpeaking { get { return speaking != null; } }
    [HideInInspector] public bool isWaitingForUserInput = false;

    Coroutine speaking = null;

    TextArchitect textArchitect = null;


    IEnumerator Speaking(string speech, bool additive, string speaker = "") {
        speechPanel.SetActive(true);

        string additiveSpeech = additive ? speechText.text : "";
        
        targetSpeech = additiveSpeech + speech;

        
        TextArchitect textArchitect = new TextArchitect(speech, additiveSpeech);


        speakerNameText.text = DetermineSpeaker(speaker);
        isWaitingForUserInput = false;


        while (textArchitect.isConstructing) {
            if (Input.GetKey(KeyCode.Space))
                textArchitect.skip = true;

            speechText.text = textArchitect.currentText;


            yield return new WaitForEndOfFrame();
        }
        //Is skipping prevented the display text from updating completely, force it to update.
        speechText.text = textArchitect.currentText;
        //text finished;
        isWaitingForUserInput = true;
        while (isWaitingForUserInput)
            yield return new WaitForEndOfFrame();

        StopSpeaking();
    }

    string DetermineSpeaker(string s) {
        string retVal = speakerNameText.text;
        if (s != speakerNameText.text && s != "")
        {
            retVal = (s.ToLower().Contains("narrator")) ? "" : s;
        }
        return retVal;
    }

    public void Close()
	{

        StopSpeaking();
        speechPanel.SetActive(false);
	}

    [System.Serializable]
    public class ELEMENTS {
        /// <summary>
        /// The main panel containing all dialogue related elements on the UI
        /// </summary>
        public GameObject speechPanel;
        public Text speakerNameText;
        public Text speechText;
    }
    public GameObject speechPanel { get { return elements.speechPanel; } }
    public Text speakerNameText { get { return elements.speakerNameText; } }
    public Text speechText {get { return elements.speechText; } }


    #endregion
}
