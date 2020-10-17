using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class architectTesting : MonoBehaviour
{
    #region Variables
    public Text text;
    public TextMeshProUGUI tmproText;
    TextArchitect architect;

    [TextArea(5, 10)]
    public string say;
    public int charactersPerFrame = 1;
    public float speed = 1f;
    public bool useEncap = true;
    public bool useTMPro = true;

	#endregion

#region Unity Methods



    // Start is called before the first frame update
    void Start()
    {
        architect = new TextArchitect(say, "", charactersPerFrame, speed, useEncap, useTMPro);


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space)) {
            //architect = new TextArchitect(say); //default
            architect = new TextArchitect(say, "", charactersPerFrame, speed, useEncap, useTMPro);
        }
        if (useTMPro)
            tmproText.text = architect.currentText;
        else
            text.text = architect.currentText;
    }

#endregion
}
