using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using FairyGUI;

public class TestEffects : MonoBehaviour
{

    UIPanel uIPanel;
    GComponent ui;




    public GameObject effectA;
    public GameObject effectB;
    public GameObject effectC;

    GGraph graphA;
    GGraph graphB;
    GGraph graphC;


    // Start is called before the first frame update
    void Start()
    {
        uIPanel = GetComponent<UIPanel>();
        if(uIPanel != null)
        {
            ui = uIPanel.ui;
            if(ui.GetChild("effectA") != null)
            {
                graphA = ui.GetChild("effectA").asGraph;

                GoWrapper goWrapper = new GoWrapper(GameObject.Instantiate( effectA));
                graphA.SetNativeObject(goWrapper);
            }


            if (ui.GetChild("effectB") != null)
            {
                graphB = ui.GetChild("effectB").asGraph;
                GoWrapper goWrapper = new GoWrapper(GameObject.Instantiate(effectB));
                graphB.SetNativeObject(goWrapper);
            }



            if (ui.GetChild("effectC") != null)
            {
                graphC = ui.GetChild("effectC").asGraph;
                GoWrapper goWrapper = new GoWrapper(GameObject.Instantiate(effectC));
                graphC.SetNativeObject(goWrapper);
            }
 
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
