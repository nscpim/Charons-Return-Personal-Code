using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class LobbyItemBuy : VRTK_InteractableObject
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
    public override void StartUsing(VRTK_InteractUse usingObject)
    {
        base.StartUsing(usingObject);

        //Check if the item is already in the player inventory.
        if (!LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.itemsInInventory.Contains(gameObject.name))
        {
            //Check if the player has enough points to purchase the item.
            if (LobbyManager.instance.lobbyScoreManager.playerScore >= cost || type == ItemType.Item || type == ItemType.MeleeWeapon)
            {
                //Switching item on Type, we add the item that will be purchased into the player inventory, and set the startposition for the item/weapon.
                switch (type)
                {
                    case ItemType.RangedWeapon:
                        LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.itemsInInventory.Add(gameObject.name);
                        LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.itemsInLevel[gameObject.name].GetComponent<Weapon>().startPosition = LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol.transform.position;
                        break;
                    case ItemType.MeleeWeapon:
                        LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.itemsInInventory.Add(gameObject.name);
                        LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.itemsInLevel[gameObject.name].GetComponent<Weapon>().startPosition = LobbyManager.instance.player.GetComponent<LobbyPlayer>().bat.transform.position;
                        break;
                    default:
                        break;
                }
                //Check if its a weapon or item since you can equip all these items
                if (type == ItemType.RangedWeapon || type == ItemType.MeleeWeapon || type == ItemType.Item)
                {
                    //If the player has no weapon it will make it equip, if it doesnt it will go to the inventory.
                    if (LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon == null)
                    {
                        switch (type)
                        {
                            case ItemType.RangedWeapon:
                                BuyItem(gameObject.name, cost, false, LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol);
                                break;
                            case ItemType.MeleeWeapon:
                                BuyItem(gameObject.name, cost, false, LobbyManager.instance.player.GetComponent<LobbyPlayer>().bat);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case ItemType.RangedWeapon:
                                BuyItem(gameObject.name, cost, true, LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol);
                                break;
                            case ItemType.MeleeWeapon:
                                BuyItem(gameObject.name, cost, true, LobbyManager.instance.player.GetComponent<LobbyPlayer>().bat);
                                break;
                            default:
                                break;
                        }
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
            if (LobbyManager.instance.lobbyScoreManager.playerScore - 100 >= ammoCost)
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
        LobbyManager.instance.lobbyScoreManager.DecreaseScore(cost);
    }

    /// <summary>
    /// Called when someone buys an item
    /// </summary>
    /// <param name="buyingItem"></param>
    /// <param name="itemCost"></param>
    public void BuyItem(string buyingItem, int itemCost, bool equipped, GameObject weapon)
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
        LobbyManager.instance.lobbyScoreManager.DecreaseScore(itemCost);

        //Setting the localplayer
        playerObject = LobbyManager.instance.player.gameObject;


        //If the player doesn't have a weapon in his hands
        if (!equipped)
        {
            //This turns off the model of the hand, so you can see the weapon
            LobbyManager.instance.VRTKSDKManager.transform.Find("SDKSetups").transform.Find("SteamVR").transform.Find("[CameraRig]").transform.Find("Controller (right)").transform.Find("RightController").transform.Find("hand_1").gameObject.SetActive(false);
            //Set the item as parent on the localplayer
            LobbyManager.instance.player.GetComponent<LobbyPlayer>().SetAParent(weapon);
            //the weapon bought will be set as the Equipped weapon from the Dictionary from itemsInLevel 
            LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon = weapon;
        }
        //if the player does have a weapon in his hands it will equip the weapon the player bought and put the other weapon or item in the inventory
        else
        {
            if (LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon.GetComponent<LobbyRangedWeapon>())
            {
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().UnSetAParent(LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon, LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon.GetComponent<LobbyRangedWeapon>().startPosition);
            }
            else if (LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon.GetComponent<LobbyBatWeapon>())
            {
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().UnSetAParent(LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon, LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon.GetComponent<LobbyBatWeapon>().startPosition);
            }
            else if (LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon.GetComponent<Item>())
            {
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().UnSetAParent(LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon, LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon.GetComponent<Item>().startPosition);
            }

            LobbyManager.instance.player.GetComponent<LobbyPlayer>().SetAParent(weapon);
            LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.equippedWeapon = weapon;
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
            LobbyManager.instance.pickedUpBat = " ✔";
            LobbyManager.instance.lobbyScoreManager.GetComponent<LobbyScoreManager>().researchPoints += 100;
            LobbyManager.instance.lobbyScoreManager.GetComponent<LobbyScoreManager>().UpdateScore();
            weapon.transform.Find("Controller hint touchpad").gameObject.SetActive(true);
            LobbyManager.instance.SetInfoboardText();
        }
        if (gameObject.name == "Pistol")
        {
            LobbyManager.instance.pickedUpPistol = " ✔";
            LobbyManager.instance.lobbyScoreManager.GetComponent<LobbyScoreManager>().researchPoints += 100;
            LobbyManager.instance.lobbyScoreManager.GetComponent<LobbyScoreManager>().UpdateScore();
            LobbyManager.instance.SetInfoboardText();
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
        if (ammoType != AmmoType.NULL && LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.itemsInInventory.Contains(gameObject.name))
        {
            if (LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol.GetComponent<LobbyRangedWeapon>().reloadHint != null &&
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol.GetComponent<LobbyRangedWeapon>().reloadHints < 3 &&
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.weaponsAndCurrentAmmo[gameObject.name] == 0)
            {
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol.GetComponent<LobbyRangedWeapon>().reloadHints++;
                LobbyManager.instance.player.GetComponent<LobbyPlayer>().pistol.GetComponent<LobbyRangedWeapon>().reloadHint.SetActive(true);
            }
            //Decreasing Points for the player that buys the ammo.
            LobbyManager.instance.lobbyScoreManager.DecreaseScore(ammoCost);
            //Increase the ammo for the ammotype and the amount of ammo.
            LobbyManager.instance.player.GetComponent<LobbyPlayer>().inventory.IncreaseTotalAmmo(ammoType.ToString(), ammo);
            if (LobbyManager.instance.boughtAmmoForPistol != " ✔")
            {
                LobbyManager.instance.boughtAmmoForPistol = " ✔";
                LobbyManager.instance.lobbyScoreManager.GetComponent<LobbyScoreManager>().researchPoints += 100;
                LobbyManager.instance.lobbyScoreManager.GetComponent<LobbyScoreManager>().UpdateScore();
                LobbyManager.instance.SetInfoboardText();
            }
        }
        else
        {
            Debug.Log("This item doesn't need ammo");
        }
    }
}