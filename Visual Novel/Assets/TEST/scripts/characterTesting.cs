using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterTesting : MonoBehaviour
{
    #region Variables
    public Character Sachi;
    public int bodyIndex, expressionIndex = 0;

	#endregion

#region Unity Methods


    // Start is called before the first frame update
    void Start()
    {
        Sachi = CharacterManager.instance.GetCharacter("sachi", enableCreatedCharacteronStart:true);   
    }

    public string[] speech;
    int i = 0;

    public Vector2 moveTarget;
    public float moveSpeed;
    public bool smooth;

    public float speed = 5f;
    public bool smoothTransitions = false;

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Space))
		{
            if (i < speech.Length)
                Sachi.Say(speech[i]);
            else
                DialogueSystem.instance.Close();

            i++;
		}

        if (Input.GetKey(KeyCode.M)) {
            Sachi.MoveTo(moveTarget, moveSpeed, smooth);
        }

		if (Input.GetKey(KeyCode.S))
		{
            Sachi.StopMoving(true);
		}


        //This is if our character has a single texture file with indexed sprites
        if(Input.GetKey(KeyCode.B)){
            if (Input.GetKeyDown(KeyCode.T)) { 
                                Sachi.TransitionBody(Sachi.GetSprite(CharacterManager.characterBodies.normal), speed, smoothTransitions);
                Sachi.SetBody(bodyIndex);


            } //If indexed sprite file
              //Sachi.TransitionBody(Sachi.GetSprite(bodyIndex), speed, smoothTransitions);
            if (Input.GetKeyDown(KeyCode.L)) { 
                            Sachi.TransitionBody(Sachi.GetSprite("body"), speed, smoothTransitions);        
                Sachi.SetBody(bodyIndex);


            }
        }
        if(Input.GetKey(KeyCode.E)){
            
               // Sachi.TransitionExpression(Sachi.GetSprite(CharacterManager.characterExpressions.normal), speed, smoothTransitions);
            
                //Sachi.SetExpression(expressionIndex);

                //Sachi.TransitionBody(Sachi.GetSprite(bodyIndex), speed, smoothTransitions);
            if (Input.GetKeyDown(KeyCode.T))
			{
                Sachi.SetExpression(Sachi.GetSprite("normal"));
                //Sachi.TransitionExpression(Sachi.GetSprite("angry"), speed, smoothTransitions);


            }
            else
			{               
                Sachi.TransitionExpression(Sachi.GetSprite(0), speed, smoothTransitions);

                //Sachi.SetExpression(Sachi.GetSprite(0));

            }
        }      


         

    }

#endregion
}
