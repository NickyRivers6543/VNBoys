using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Responsible for adding and maintaining characters in the scene.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    #region Variables
    public static CharacterManager instance;

	/// <summary>
	/// All characters must be attached to the character panel
	/// </summary>
	public RectTransform characterPanel;
	/// <summary>
	/// A list of all characters currently in our scene.
	/// </summary>
	public List<Character> characters = new List<Character>();

	/// <summary>
	/// Easy lokup for our characters.
	/// </summary>
	public Dictionary<string, int> characterDictionary = new Dictionary<string, int>();
	#endregion


	#region Unity Methods


	 void Awake()
	{
		instance = this;
	}
	/// <summary>
	/// Try to get a character by the name provided from the character list
	/// </summary>
	/// <param name="characterName"></param>
	public Character GetCharacter(string characterName, bool createCharacterIfDoesNotExist = true, bool enableCreatedCharacteronStart = true) {
		int index = -1;
		//search our dictionary to find the character quickly if it is already in our scene
		if (characterDictionary.TryGetValue(characterName, out index))
		{
			return characters[index];
		}
		else if(createCharacterIfDoesNotExist){
			return CreateCharacter(characterName, enableCreatedCharacteronStart);
		}

		return null;
	}
	/// <summary>
	/// Creates the character
	/// </summary>
	/// <param name="characterName"></param>
	/// <returns></returns>
	public Character CreateCharacter(string characterName, bool enableOnStart = true) {
		Character newCharacter = new Character(characterName, enableOnStart);

		characterDictionary.Add(characterName, characters.Count);
		characters.Add(newCharacter);

		return newCharacter;
	}

	public class CHARACTERPOSITIONS { 
		public Vector2 bottomLeft = new Vector2(0, 0);
		public Vector2 topRight = new Vector2(1f, 1f);
		public Vector2 center = new Vector2(0.5f, 0.5f);
		public Vector2 bottomRight = new Vector2(1f, 0);
		public Vector2 topleft = new Vector2(0, 1f);
	}
	public static CHARACTERPOSITIONS characterPositions = new CHARACTERPOSITIONS();

	public class CHARACTEREXPRESSIONS {
		public int normal = 0;
		public int shy = 1;
	}

	public static CHARACTEREXPRESSIONS characterExpressions = new CHARACTEREXPRESSIONS();
		
	public class CHARACTERBODIES {
		public int normal = 0;
		public int shy = 1;
	}

	public static CHARACTERBODIES characterBodies = new CHARACTERBODIES();



	#endregion
}
