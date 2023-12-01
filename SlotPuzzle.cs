using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotPuzzle : MonoBehaviour
{
    [Tooltip("All the slotmachines")]
    public GameObject[] slotMachines;

    //oneTime is used so it only fires one
    private bool oneTime;

    /// <summary>
    /// Rewards the player with items and points
    /// </summary>
    public void Reward()
    {
        if (GameManager.instance.localPlayer.GetComponent<Player>().isServer)
        {
            GameManager.instance.scoreManager.UpdateResearchScore(500);
        }
        Level2Manager.instance.solvedSlotMachinePuzzle = " ✔";
        Level2Manager.instance.SetInfoboardText();
        //GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add("Bomb");
        GameManager.instance.scoreManager.IncreaseScore((int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value, 5000);
        Level2Manager.instance.minigun.SetActive(true);
    }
    /// <summary>
    /// Checks if all slotmachine's have "777" on their screens
    /// </summary>
    private void Update()
    {
        if (!oneTime && 
            slotMachines[0].GetComponent<SpinCounter>().GetBool() &&
            slotMachines[1].GetComponent<SpinCounter>().GetBool() &&
            slotMachines[2].GetComponent<SpinCounter>().GetBool())
        {
            Reward();
            oneTime = true;
        }
    }
}
