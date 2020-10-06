using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
	#region Variables
	#endregion

	#region Unity Methods

	public static string[] SplitByTags(string targetText)
	{
		return targetText.Split(new char[2] { '<', '>' });
	}
  

#endregion
}
