using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Extra_Steps : MonoBehaviour
{
    public int extraSteps;

    public int GetSteps()
    {
        RemoveItem();
        return extraSteps;
    }

    private void RemoveItem()
    {
        UI_Inventory.Instance.RemoveItem();
    }
}
