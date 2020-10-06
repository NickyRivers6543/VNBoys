using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class architectTesting : MonoBehaviour
{
    #region Variables
    public Text text;
    TextArchitect architect;

    [TextArea(5, 10)]
    public string say;
    public int charactersPerFrame = 1;
    public float speed = 1f;
    public bool useEncap = true;

	#endregion

#region Unity Methods



    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) {
            //architect = new TextArchitect(say); //default
            architect = new TextArchitect(say, "", charactersPerFrame, speed, useEncap);
        }

        text.text = architect.currentText;
    }

#endregion
}
