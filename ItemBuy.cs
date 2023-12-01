using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class ItemBuy : VRTK_InteractableObject
{
    
    //ammo is the amount of ammo this point gives if the player already has the gun this point sells.
    //Cost is the amount of points the player needs to have in order to buy this gun or weapon.
    [Header("Wallbuy Attributes")]
    [Tooltip("The amount of ammo this wallbuy gives, Leave this empty when this wallbuy/machine doesnt sell a gun")]
    public int ammo;
    [Tooltip("The cost of this wallbuy")]
    public int cost;
    [Tooltip("The cost of the ammo for this weapon if the player already owns the weapon for it")]
    public int ammoCost;

    
    [Header("Types")]
    //The Type of item this wallbuy sells.
    [Tooltip("Type of item this wallbuy sells")]
    public ItemType type;
    //The Type of ammo this wallbuy sells for the weapons, if this wallbuy doesnt sell a weapon set this to NULL.
    [Tooltip("The Type of ammo this wallbuy sells for the weapon, if you don't sell a weapon here set this to NULL")]
    public AmmoType ammoType;

    //Player GameObject Reference
    private GameObject playerObject;

    public GameObject pickupHint;
    public GameObject buyHint;

    /// <summary>
    /// When the trigger is pulled within the collider of this object.
    /// </summary>
    /// <param name="usingObject"></param>
    public override void StartUsing(VRTK_InteractUse usingObject = null)
    {
        base.StartUsing(usingObject);

        if (usingObject == null || GameManager.instance.localPlayer.GetComponent<Player>().inElevator)
        {
            return;
        }

        //Check if the item is already in the player inventory.
        if (!GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Contains(gameObject.name))
        {
            //Check if the player has enough points to purchase the item.
            if (GameManager.instance.scoreManager.playerScores[(int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value] >= cost || type == ItemType.Item || type == ItemType.MeleeWeapon || name == "minigun")
            {
                //Switching item on Type, we add the item that will be purchased into the player inventory, and set the startposition for the item/weapon.
                switch (type)
                {
                    case ItemType.RangedWeapon:
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add(gameObject.name);
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].GetComponent<Weapon>().startPosition = GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].transform.position;
                        break;
                    case ItemType.MeleeWeapon:
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add(gameObject.name);
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].GetComponent<Weapon>().startPosition = GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].transform.position;
                        break;
                    case ItemType.Token:
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add(gameObject.name);
                        break;
                    case ItemType.Drone:
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add(gameObject.name);
                        break;
                    case ItemType.Item:
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Add(gameObject.name);
                        GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].GetComponent<Item>().startPosition = GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].transform.position;
                        break;
                    default:
                        break;
                }
                //Check if its a weapon or item since you can equip all these items
                if (type == ItemType.RangedWeapon || type == ItemType.MeleeWeapon || type == ItemType.Item)
                {
                    //If the player has no weapon it will make it equip, if it doesnt it will go to the inventory.
                    if (GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon == null)
                    {
                        BuyItem(gameObject.name, cost, false);
                    }
                    else
                    {
                        BuyItem(gameObject.name, cost, true);
                    }
                }
                //Other items, like turret tokens or bomb items for level 3 
                else
                {
                    BuyMiscItem(gameObject.name, cost);
                }
            }
            else
            {
                Debug.Log("You Don't have enough points to buy this, you only have: " + GameManager.instance.scoreManager.playerScores[(int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value]);
            }
        }
        else
        {
            //If the player already has the item into the inventory it buys ammo instead, also when the item doesnt use ammo it will just do nothing
            if (GameManager.instance.scoreManager.playerScores[(int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value] >= ammoCost)
            {
                BuyAmmo(ammoCost);
            }
            else
            {
                Debug.Log("You Don't have enough points to buy this, you only have: " + GameManager.instance.scoreManager.playerScores[(int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value]);
            }
        }
    }
    /// <summary>
    /// Buys other items.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cost"></param>
    private void BuyMiscItem(string item, int cost)
    {
        //Decreasing the points of the player
        GameManager.instance.scoreManager.DecreaseScore((int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value, cost, false);
    }

    /// <summary>
    /// Called when someone buys an item
    /// </summary>
    /// <param name="buyingItem"></param>
    /// <param name="itemCost"></param>
    public void BuyItem(string buyingItem, int itemCost, bool equipped)
    {
        if (pickupHint != null)
        {
            pickupHint.SetActive(false);
            if (buyHint != null)
            {
                buyHint.SetActive(true);
            }
        }
        //Decreasing the player points
        GameManager.instance.scoreManager.DecreaseScore((int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value, itemCost, false);

        //Setting the localplayer
        playerObject = GameManager.instance.localPlayer.gameObject;


        //If the player doesn't have a weapon in his hands
        if (!equipped)
        {
            //This turns off the model of the hand, so you can see the weapon
            GameManager.instance.VRTKSDKManager.transform.Find("SDKSetups").transform.Find("SteamVR").transform.Find("[CameraRig]").transform.Find("Controller (right)").transform.Find("RightController").transform.Find("hand_1").gameObject.SetActive(false);
            //Set the item as parent on the localplayer
            GameManager.instance.localPlayer.GetComponent<Player>().CmdSetAParent(GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name], false, playerObject, (int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value);
            //the weapon bought will be set as the Equipped weapon from the Dictionary from itemsInLevel 
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon = GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].gameObject;
        }
        //if the player does have a weapon in his hands it will equip the weapon the player bought and put the other weapon or item in the inventory
        else
        {
            if (GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon.GetComponent<RangedWeapon>())
            {
                GameManager.instance.localPlayer.GetComponent<Player>().CmdUnSetAParent(GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon, GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon.GetComponent<RangedWeapon>().startPosition);
            }
            else if (GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon.GetComponent<BatWeapon>())
            {
                GameManager.instance.localPlayer.GetComponent<Player>().CmdUnSetAParent(GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon, GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon.GetComponent<BatWeapon>().startPosition);
            }
            else if (GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon.GetComponent<Item>())
            {
                GameManager.instance.localPlayer.GetComponent<Player>().CmdUnSetAParent(GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon, GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon.GetComponent<Item>().startPosition);
            }

            GameManager.instance.localPlayer.GetComponent<Player>().CmdSetAParent(GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name], false, playerObject, (int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value);
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.equippedWeapon = GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].gameObject;
        }
        //Sets the item in inactive if its part of a puzzle or item that doesnt sell anything
        if (gameObject.name == "Bat" || gameObject.name == "minigun" ||
            gameObject.name == "Portal_gun" || gameObject.name == "energy_gun" ||
            gameObject.name == "speargun" || gameObject.name == "GolfClub")
        {
            gameObject.SetActive(false);
        }
        if (gameObject.name == "Bat")
        {
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].transform.Find("Controller hint touchpad").gameObject.SetActive(true);
            GameManager.instance.achievementManager.AdvanceAchievement("Bat", 1);
        }
        if (SceneManager.GetActiveScene().name == "Level 1")
        {
            if (gameObject.name == "Bat" && Level1Manager.instance.pickedUpBat != " ✔")
            {
                Level1Manager.instance.pickedUpBat = " ✔";
                GameManager.instance.scoreManager.UpdateResearchScore(100);
                Level1Manager.instance.SetInfoboardText();
            }
            else if (gameObject.name == "Pistol" && Level1Manager.instance.pickedUpPistol != " ✔")
            {
                Level1Manager.instance.pickedUpPistol = " ✔";
                GameManager.instance.scoreManager.UpdateResearchScore(100);
                Level1Manager.instance.SetInfoboardText();
            }
            else if (gameObject.name == "M4_Carbine" && Level1Manager.instance.pickedUpM4 != " ✔")
            {
                Level1Manager.instance.pickedUpM4 = " ✔";
                GameManager.instance.scoreManager.UpdateResearchScore(100);
                Level1Manager.instance.SetInfoboardText();
            }
            else if (gameObject.name == "Shotgun" && Level1Manager.instance.pickedUpShotgun != " ✔")
            {
                Level1Manager.instance.pickedUpShotgun = " ✔";
                GameManager.instance.scoreManager.UpdateResearchScore(100);
                Level1Manager.instance.SetInfoboardText();
            }
        }
        if (gameObject.name == "Timer" || gameObject.name == "C41" ||
            gameObject.name == "C42" || gameObject.name == "C43" ||
            gameObject.name == "C44")
        {
            GameManager.instance.localPlayer.GetComponent<InteractionsLevel3>().CmdDisablePickedUpBombParts(gameObject);
        }
        if (gameObject.name == "Reel1" || gameObject.name == "Reel2" || gameObject.name == "Reel3" || gameObject.name == "ProjectorLens" || gameObject.name == "Key")
        {
            GameManager.instance.localPlayer.GetComponent<InteractionsLevel2>().CmdDeactivateForEveryone(gameObject);
        }
        if (gameObject.name == "minigun")
        {
            GameManager.instance.VRTKSDKManager.transform.Find("SDKSetups").transform.Find("SteamVR").transform.Find("[CameraRig]").transform.Find("Controller (left)").transform.Find("LeftController").GetComponent<VRTK_BezierPointerRenderer>().maximumLength.x = 1;
            foreach (GameObject item in Level2Manager.instance.minigunTeleportAreas)
            {
                item.SetActive(false);
            }
        }
        if (gameObject.name == "GolfClub")
        {
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].transform.Find("CalculateGolfclubBallCollision").GetComponent<FindGolfClubCollisions>().enabled = true;
        }
    }
    /// <summary>
    /// Called when someone buys ammo
    /// </summary>
    /// <param name="ammoCost"></param>
    public void BuyAmmo(int ammoCost)
    {
        if (buyHint != null)
        {
            buyHint.SetActive(false);
        }
        //Checks if the item uses ammo and if the item is already in the inventory that uses ammo.
        if (ammoType != AmmoType.NULL && GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInInventory.Contains(gameObject.name))
        {
            if (GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].gameObject.GetComponent<RangedWeapon>().reloadHint != null &&
                GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].gameObject.GetComponent<RangedWeapon>().reloadHints < 3 &&
                GameManager.instance.localPlayer.GetComponent<Player>().inventory.weaponsAndCurrentAmmo[gameObject.name] == 0)
            {
                GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].gameObject.GetComponent<RangedWeapon>().reloadHints++;
                GameManager.instance.localPlayer.GetComponent<Player>().inventory.itemsInLevel[gameObject.name].gameObject.GetComponent<RangedWeapon>().reloadHint.SetActive(true);
            }
            //Decreasing Points for the player that buys the ammo.
            GameManager.instance.scoreManager.DecreaseScore((int)GameManager.instance.localPlayer.GetComponent<Player>().netId.Value, ammoCost, false);
            //Increase the ammo for the ammotype and the amount of ammo.
            GameManager.instance.localPlayer.GetComponent<Player>().inventory.IncreaseTotalAmmo(ammoType.ToString(), ammo);
        }
        else
        {
            Debug.Log("This item doesn't need ammo");
        }
    }

    public void DisablePickedUpBombParts()
    {
        gameObject.SetActive(false);
    }
}
//Every different item type
public enum ItemType
{
    RangedWeapon,
    MeleeWeapon,
    Token,
    Drone,
    Item
}
//Every different ammo type
public enum AmmoType
{
    Pistol,
    M4_Carbine,
    Shotgun,
    energy_gun,
    minigun,
    speargun,
    Portal_gun,
    NULL
}