using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_FPS_Display : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    
    private float pollingTime = 1f;
    private float time;
    private int frameCount;
 
    void Update() 
    {
        time += Time.deltaTime;
        
        frameCount++;

        if (time >= pollingTime) {
            
            int frameRate = Mathf.RoundToInt(frameCount / time);
            fpsText.text = frameRate + " fps";
            
            time -= pollingTime;
            frameCount = 0;
        }
    }
}
