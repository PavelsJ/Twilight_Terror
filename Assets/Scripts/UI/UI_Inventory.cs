using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory Instance { get; private set; }
    
    public Sprite defaultSlotSprite;
    public Sprite selectedSlotSprite;
    
    public Image[] itemSlots;
    
    private int currentSlotIndex = 0; 
    private int selectedSlotIndex = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (currentSlotIndex > 1)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SelectItem(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                SelectItem(Vector3.right);
            }
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            RemoveItem();
        }
    }

    private void SelectItem(Vector3 direction)
    {
        SetSelection(selectedSlotIndex, defaultSlotSprite);
        
        if (direction == Vector3.left)
        {
            selectedSlotIndex = (selectedSlotIndex - 1 + currentSlotIndex) % currentSlotIndex;
        }
        else if (direction == Vector3.right)
        {
            selectedSlotIndex = (selectedSlotIndex + 1) % currentSlotIndex;
        }
        
        SetSelection(selectedSlotIndex, selectedSlotSprite);
    }
    
    public void AddItem(GameObject itemPrefab, int amount = 1)
    {
        for (int i = 0; i < amount; i++)
        {
            if (currentSlotIndex < itemSlots.Length)
            {
                if (selectedSlotIndex < currentSlotIndex)
                {
                    SetSelection(selectedSlotIndex, defaultSlotSprite);
                }
                
                Instantiate(itemPrefab, itemSlots[currentSlotIndex].transform);
                selectedSlotIndex = currentSlotIndex;
                currentSlotIndex++;
                
                SetSelection(selectedSlotIndex, selectedSlotSprite);
            }
            else
            {
                Debug.LogWarning("Недостаточно слотов для добавления света!");
                break; 
            }
        }
    }
    
    public void RemoveItem()
    {
        if (selectedSlotIndex < itemSlots.Length && itemSlots[selectedSlotIndex].transform.childCount > 0)
        {
            GameObject child = itemSlots[selectedSlotIndex].transform.GetChild(0).gameObject;
            var steps = child.GetComponent<UI_Extra_Steps>().extraSteps;
            Player_Steps.Instance.AddSteps(steps);
            
            Destroy(child);
            
            // Смещение всех элементов справа от удаленного слота влево
            for (int i = selectedSlotIndex; i < currentSlotIndex - 1; i++)
            {
                SetSelection(selectedSlotIndex, defaultSlotSprite);
                
                if (itemSlots[i + 1].transform.childCount > 0)
                {
                    Transform movingItem = itemSlots[i + 1].transform.GetChild(0);
                    movingItem.SetParent(itemSlots[i].transform);
                    movingItem.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                }
            }
            
            currentSlotIndex--;
            
            if (currentSlotIndex < itemSlots.Length)
            {
                foreach (Transform childTransform in itemSlots[currentSlotIndex].transform)
                {
                    Destroy(childTransform.gameObject);
                    SetSelection(currentSlotIndex, defaultSlotSprite);
                }
            }
            
            if (currentSlotIndex > 0)
            {
                selectedSlotIndex = currentSlotIndex - 1;
                SetSelection(selectedSlotIndex, selectedSlotSprite);
            }
        }
        else
        {
            Debug.LogWarning("Выбранный слот пуст или некорректен!");
        }
    }
    
    private void SetSelection(int index, Sprite image)
    {
        itemSlots[index].sprite = image;
    }
    
    public bool IsInventoryEmpty()
    {
        return currentSlotIndex == 0;
    }

    public bool isInventoryFull()
    {
        return currentSlotIndex >= itemSlots.Length;
    }
}
