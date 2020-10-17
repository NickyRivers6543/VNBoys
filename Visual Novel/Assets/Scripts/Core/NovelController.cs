using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    #region Variables
    //The lines of data loaded directly from a chapter file.
    List<string> data = new List<string>();

    //The progress in the current data list.
    int progress = 0;

    /// <summary>
    /// Used as a fallback when no speaker is given.
    /// </summary>
    string cachedLastSpeaker = "";
    #endregion

    #region Unity Methods


    // Start is called before the first frame update
    void Start()
    {
        LoadChapterFile("chapter0_start"); //Change this to whatever our data is. 
        progress = 0;
        cachedLastSpeaker = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            HandleLine(data[progress++]);
        }
    }

    void LoadChapterFile(string fileName) {
        data = FileManager.LoadFile(FileManager.savPath + "Resources/Story/" + fileName);
    }


    /// <summary>
    /// This will be how we handle our line formats. 
    /// 
    /// Example: Avira "Look, a forest" setBackground(forest)
    /// </summary>
    /// <param name="line"></param>
    void HandleLine(string line) {
        string[] dialogueAndActions = line.Split('"');
        //3 objects means there is dialogue
        //1 object means there is no dialogue. only actions

        if (dialogueAndActions.Length == 3) {
            HandleDialogue(dialogueAndActions[0], dialogueAndActions[1]);
            HandleEventsFromLine(dialogueAndActions[2]);

        }
        else
        {
            HandleEventsFromLine(dialogueAndActions[0]);

        }

    }

    void HandleDialogue(string dialogueDetails, string dialogue) {
        string speaker = cachedLastSpeaker;
        bool additive = dialogueDetails.Contains("+"); //If

        //remove the additive sign from the speaker name area
        if (additive)
            dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);
        if (dialogueDetails.Length > 0) {
            //remove the space after the spaker's name is present
            if (dialogueDetails[dialogueDetails.Length - 1] == ' ') {
                dialogueDetails = dialogueDetails.Remove(dialogueDetails.Length - 1);
            }

            speaker = dialogueDetails;
            cachedLastSpeaker = speaker;
        }

        if (speaker != "narrator") {
            Character character = CharacterManager.instance.GetCharacter(speaker);
            character.Say(dialogue, additive);
        } else {
            DialogueSystem.instance.Say(dialogue, additive, speaker);
        }

    }

    void HandleEventsFromLine(string events) {
        string[] actions = events.Split(' '); //Event calls in and of themselves must not contain spaces. Else they will split

        foreach (string action in actions)
        {
            HandleEventsFromLine(action);
        }
    }

    void HandleAction(string action) {
        string[] data = action.Split('(', ')');

        switch (data[0]) {
            case "setBackground":
                Command_SetLayerImage(data[1], BCFC.instance.background);
                break;
            case "setCinematic":
                Command_SetLayerImage(data[1], BCFC.instance.cinematic);
                break;
            case "setForeground":
                Command_SetLayerImage(data[1], BCFC.instance.foreground);
                break;
            case "playSound":
                Command_PlaySound(data[1]);
                break;
            case "playMusic":
                Command_PlayMusic(data[1]);
                break;
            case "move":
                Command_MoveCharacter(data[1]);
                break;
            case "setPosition":
                Command_SetPosition(data[1]);
                break;
            case "changeExpression":
                Command_ChangeExpression(data[1]);
                break;
        
        
        

        
        }

        return;
       
    }


	void Command_SetLayerImage(string data, BCFC.LAYER layer) {
        string texName = data.Contains(",") ? data.Split(',')[0] : data;
        Texture2D tex = texName == "null" ? null : Resources.Load("Images/UI/Backdrops/" + texName) as Texture2D;
        float spd = 2f; //optional speed parameter
        bool smooth = false; //optional smooth transition parameter


        if (data.Contains(",")) {
            string[] parameters = data.Split(',');



            foreach (string p in parameters) {
                float fVal = 0;
                bool bVal = false;
                if (float.TryParse(p, out fVal)) {
                    spd = fVal; continue;
                }
                if (bool.TryParse(p, out bVal)) {
                    smooth = bVal; continue;
                }
            }
        }
        layer.TransitionToTexture(tex, spd, smooth);
    }
    void Command_PlaySound(string data) {
        AudioClip clip = Resources.Load("Audio/SFX/" + data) as AudioClip;

        if (clip != null)
            AudioManager.instance.PlaySFX(clip);
        else
            Debug.LogError("Clip does not exist - " + data);
    }

    void Command_PlayMusic(string data) {
        AudioClip clip = Resources.Load("Audio/Music/" + data) as AudioClip;

        if (clip != null)
            AudioManager.instance.PlaySong(clip);
        else
            Debug.LogError("Clip does not exist - " + data);
    }

    void Command_MoveCharacter(string data) {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float locationX = float.Parse(parameters[1]);
        float locationY = float.Parse(parameters[2]);
        float speed = parameters.Length == 4 ? float.Parse(parameters[3]) : 1f;
        bool smooth = parameters.Length == 5 ? bool.Parse(parameters[4]) : true;

        Character c = CharacterManager.instance.GetCharacter(character);
        c.MoveTo(new Vector2(locationX, locationY), speed, smooth);
    }

     void Command_SetPosition(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        float locationX = float.Parse(parameters[1]);
        float locationY = float.Parse(parameters[2]);

        Character c = CharacterManager.instance.GetCharacter(character);
        c.SetPosition(new Vector2(locationX, locationY));
    }
    void Command_ChangeExpression(string data)
    {
        string[] parameters = data.Split(',');
        string character = parameters[0];
        string region = parameters[1];
        string expression = parameters[2];
        float speed = parameters.Length == 4 ? float.Parse(parameters[3]) : 1f;

        Character c = CharacterManager.instance.GetCharacter(character);
        Sprite sprite = c.GetSprite(expression);
        if (region.ToLower() == "body")
            c.TransitionBody(sprite, speed, false);    
        if (region.ToLower() == "expression")
            c.TransitionExpression(sprite, speed, false);
        

    }


    #endregion
}
