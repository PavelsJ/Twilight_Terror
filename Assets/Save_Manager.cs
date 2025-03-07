using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Save_Manager : MonoBehaviour
{
    public void DeleteAllSaves()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("Deleted all saves");
    }
}
