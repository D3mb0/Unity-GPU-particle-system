using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityParticleSystem : MonoBehaviour
{
    public ParticleSystem particleSystem;
    void OnGUI()
    {
        GUI.Label(new Rect(25, 20, 200, 30), "Particles Count: " +particleSystem.particleCount.ToString());
        GUI.Label(new Rect(25, 50, 200, 30), "fps: " + (1 / Time.deltaTime));
        GUI.Label(new Rect(25, 80, 200, 30), "fps: " + (1 / Time.smoothDeltaTime));
        GUI.Label(new Rect(25, 110, 200, 30), "fps: " + (1 / Time.unscaledDeltaTime));

        //particleCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)particleCount, 1.0f, 50000.0f);
    }
}
