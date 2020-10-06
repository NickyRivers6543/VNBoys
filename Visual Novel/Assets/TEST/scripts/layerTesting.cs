using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class layerTesting : MonoBehaviour
{
    #region Variables    
     BCFC controller;

    public Texture tex;

    public float speed;
    public bool smooth;

    #endregion

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        controller = BCFC.instance;

    }

    // Update is called once per frame
    void Update()
    {
        BCFC.LAYER layer = controller.background;
        if (Input.GetKey(KeyCode.Q))
            layer = controller.background;
        if (Input.GetKey(KeyCode.W))
            layer = controller.cinematic;
         if (Input.GetKey(KeyCode.E))
            layer = controller.foreground;
        if (Input.GetKey(KeyCode.T))
		{
            if (Input.GetKeyDown(KeyCode.A))
                layer.TransitionToTexture(tex, speed, smooth);
        }
		else
		{
            if (Input.GetKeyDown(KeyCode.A))
                layer.setTexture(tex);
		}

    }

    #endregion
}
