using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Room_Script : MonoBehaviour
{
    public int nextRoomIndex;
    public float changeRoomTime;
    
    public Grid_Manager gridInteraction;
    
    public bool isStart = false;
    private bool isActive = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isActive)
        {
            if (isStart)
            {
                gridInteraction.OnStart(0, changeRoomTime);
            }
            else
            {
                gridInteraction.OnActive(nextRoomIndex, changeRoomTime);
            }
            
            Audio_Manager.PlaySound(SoundType.Shake, 1);

            StartCoroutine(Delay());
            isActive = true;
        }
    }

    private IEnumerator Delay()
    {
        Player_Movement.Instance.isDisable = true;
        yield return new WaitForSeconds(changeRoomTime);
        Player_Movement.Instance.isDisable = false;
    }
}
