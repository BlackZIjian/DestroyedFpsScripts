﻿using UnityEngine;

public class bl_UpdateManager : MonoBehaviour
{

    private int regularUpdateArrayCount = 0;
    private int fixedUpdateArrayCount = 0;
    private int lateUpdateArrayCount = 0;
    private bl_MonoBehaviour[] regularArray = new bl_MonoBehaviour[0];
    private bl_MonoBehaviour[] fixedArray = new bl_MonoBehaviour[0];
    private bl_MonoBehaviour[] lateArray = new bl_MonoBehaviour[0];
    private bool Initializate = false;

    private static bl_UpdateManager _instance;
    public static bl_UpdateManager instance
    {
        get
        {
            if (_instance == null) { _instance = FindObjectOfType<bl_UpdateManager>(); }
            return _instance;
        }
    }

    public static void AddItem(bl_MonoBehaviour behaviour)
    {
        instance.AddItemToArray(behaviour);
    }
    private void Start()
    {
        Initializate = true;
    }

    public static void RemoveSpecificItem(bl_MonoBehaviour behaviour)
    {
        if (instance != null)
        {
            instance.RemoveSpecificItemFromArray(behaviour);
        }

    }

    public static void RemoveSpecificItemAndDestroyIt(bl_MonoBehaviour behaviour)
    {
        instance.RemoveSpecificItemFromArray(behaviour);

        Destroy(behaviour.gameObject);
    }

    private void OnDestroy()
    {
        regularArray = new bl_MonoBehaviour[0];
        fixedArray = new bl_MonoBehaviour[0];
        lateArray = new bl_MonoBehaviour[0];
    }

    private void AddItemToArray(bl_MonoBehaviour behaviour)
    {
        if (behaviour.GetType().GetMethod("OnUpdate").DeclaringType != typeof(bl_MonoBehaviour))
        {
            regularArray = ExtendAndAddItemToArray(regularArray, behaviour);
            regularUpdateArrayCount++;
        }

        if (behaviour.GetType().GetMethod("OnFixedUpdate").DeclaringType != typeof(bl_MonoBehaviour))
        {
            fixedArray = ExtendAndAddItemToArray(fixedArray, behaviour);
            fixedUpdateArrayCount++;
        }

        if (behaviour.GetType().GetMethod("OnLateUpdate").DeclaringType == typeof(bl_MonoBehaviour))
            return;

        lateArray = ExtendAndAddItemToArray(lateArray, behaviour);
        lateUpdateArrayCount++;
    }

    public bl_MonoBehaviour[] ExtendAndAddItemToArray(bl_MonoBehaviour[] original, bl_MonoBehaviour itemToAdd)
    {
        int size = original.Length;
        bl_MonoBehaviour[] finalArray = new bl_MonoBehaviour[size + 1];
        for (int i = 0; i < size; i++)
        {
            finalArray[i] = original[i];
        }
        finalArray[finalArray.Length - 1] = itemToAdd;
        return finalArray;
    }

    private void RemoveSpecificItemFromArray(bl_MonoBehaviour behaviour)
    {
        if (CheckIfArrayContainsItem(regularArray, behaviour))
        {
            regularArray = ShrinkAndRemoveItemToArray(regularArray, behaviour);
            regularUpdateArrayCount--;
        }

        if (CheckIfArrayContainsItem(fixedArray, behaviour))
        {
            fixedArray = ShrinkAndRemoveItemToArray(fixedArray, behaviour);
            fixedUpdateArrayCount--;
        }

        if (!CheckIfArrayContainsItem(lateArray, behaviour)) return;

        lateArray = ShrinkAndRemoveItemToArray(lateArray, behaviour);
        lateUpdateArrayCount--;
    }

    public bool CheckIfArrayContainsItem(bl_MonoBehaviour[] arrayToCheck, bl_MonoBehaviour objectToCheckFor)
    {
        int size = arrayToCheck.Length;

        for (int i = 0; i < size; i++)
        {
            if (objectToCheckFor == arrayToCheck[i]) return true;
        }

        return false;
    }

    public bl_MonoBehaviour[] ShrinkAndRemoveItemToArray(bl_MonoBehaviour[] original, bl_MonoBehaviour itemToRemove)
    {
        int size = original.Length;
        int fix = 0;
        bl_MonoBehaviour[] finalArray = new bl_MonoBehaviour[size - 1];
        for (int i = 0; i < size; i++)
        {
            if (original[i] != itemToRemove)
            {
                finalArray[i - fix] = original[i];
            }
            else
            {
                fix++;
            }
        }
        return finalArray;
    }

    private void Update()
    {
        if (!Initializate)
            return;

        if (regularUpdateArrayCount == 0) return;

        for (int i = 0; i < regularUpdateArrayCount; i++)
        {
            if (regularArray[i] != null && regularArray[i].enabled)
            {
                regularArray[i].OnUpdate();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!Initializate)
            return;

        if (fixedUpdateArrayCount == 0) return;

        for (int i = 0; i < fixedUpdateArrayCount; i++)
        {
            if (fixedArray[i] == null) continue;

            fixedArray[i].OnFixedUpdate();
        }
    }

    private void LateUpdate()
    {
        if (!Initializate)
            return;

        if (lateUpdateArrayCount == 0) return;

        for (int i = 0; i < lateUpdateArrayCount; i++)
        {
            if (lateArray[i] == null) continue;

            lateArray[i].OnLateUpdate();
        }
    }
}