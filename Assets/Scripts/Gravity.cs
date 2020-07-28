using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{

    //Simple UI sliders for gravity and friction
    public UnityEngine.UI.Slider slider;

    public UnityEngine.UI.Text me;

    public UnityEngine.UI.Text FrictionText;

    public UnityEngine.UI.Slider FrictionBox;

    public MainTest GravityController;

    public float MaxGravity = 9.0f;




    void Start()
    {
        slider.maxValue = MaxGravity;

        slider.minValue = -MaxGravity;

        slider.value = 0;

        FrictionBox.maxValue = 1;

        FrictionBox.minValue = 0.9f;

        FrictionBox.value = 1;

    }

    // Update is called once per frame
    void Update()
    {
        GravityController.Gravity = slider.value;
        me.text = "Gravity: " + GravityController.Gravity;
        FrictionText.text ="Friction: " + (GravityController.friction).ToString();

        GravityController.friction = FrictionBox.value;
        
    }
}
