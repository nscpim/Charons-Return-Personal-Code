using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    //Points the players get when this puzzle is complete
    [Tooltip("The amount of points this puzzle gives upon completions")]
    public int pointsReward;

    /// <summary>
    /// When the player inserts the key in the chest it will get the key out of the inventory and the hand and adds points to all players, also sets the key back to its startposition
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.name == "Key")
        {
            GameManager.instance.VRTKSDKManager.transform.Find("SDKSetups").transform.Find("SteamVR").transform.Find("[CameraRig]").transform.Find("Controller (right)").transform.Find("RightController").transform.Find("hand_1").gameObject.SetActive(true);
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Remove(other.name);
            GameManager.instance.localPlayer.GetComponent<Player>().CmdUnSetAParent(other.gameObject, other.gameObject.GetComponent<Item>().startPosition);
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon = null;

            GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdOpenChest(gameObject);
            //GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add("Turret Token");
            GameManager.instance.scoreManager.IncreaseScore((int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value, pointsReward);

        }  
    }
    /// <summary>
    /// Plays the animation for opening the chest
    /// </summary>
    public void OpenChest()
    {
        GetComponent<Animator>().SetTrigger("Open");
    }
}
  

