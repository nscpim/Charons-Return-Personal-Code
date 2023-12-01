using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class Item : VRTK_InteractableObject
{
    //Startposition for the items 
    public Vector3 startPosition;
    //Check if name has been changed of the item
    private bool nameChanged;

    public int playerNetId;

    /// <summary>
    /// Dont destroy on load for all the items that spawn in with SpawnWeapons.cs
    /// </summary>
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// Removes (clone) from the item name, because the items we use don't have clone in the name in the items Dictionary
    /// </summary>
    protected override void Update()
    {
        base.Update();
        if (!nameChanged && name.Length > 7 && name.Substring((name.Length - 7), 7) == "(Clone)")
        {
            nameChanged = true;
            name = name.Substring(0, name.Length - 7);
        }
    }
}
