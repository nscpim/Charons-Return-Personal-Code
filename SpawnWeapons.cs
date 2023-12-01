using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnWeapons : NetworkBehaviour
{
   /// <summary>
   /// Command to spawn the weapons and items for each client, so when you equip an item or weapon it is placed in the player hand from beneath the map so other players can see it aswell.
   /// </summary>
   /// <param name="level"></param>
    [Command]
    public void CmdSpawnWeapon(int level, int playerNetId)
    {
        if (level == 1)
        {
            for (int i = 0; i < Enum.GetNames(typeof(Inventory.RangedWeapons)).Length; i++)
            {
                Vector3 weaponPosition = new Vector3(i, -10 - (Mathf.Abs(netId.Value * 10)), 0);
                GameObject weaponObject = Instantiate((GameObject)Resources.Load("Prefabs/Weapons/" + Enum.GetName(typeof(Inventory.RangedWeapons), i)), weaponPosition, Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(weaponObject, connectionToClient);
                if (Enum.GetName(typeof(Inventory.RangedWeapons), i) != "Bat" &&
                    Enum.GetName(typeof(Inventory.RangedWeapons), i) != "minigun" &&
                    Enum.GetName(typeof(Inventory.RangedWeapons), i) != "speargun" &&
                    Enum.GetName(typeof(Inventory.RangedWeapons), i) != "Portal_gun")
                {
                    TargetPutItemInInventoryDictionary(connectionToClient, weaponObject, true, false, 0);
                }
                else
                {
                    TargetPutItemInInventoryDictionary(connectionToClient, weaponObject, false, false, 0);
                }
               
            }
        }
        if (level == 2)
        {
            for (int i = 0; i < Enum.GetNames(typeof(Inventory.Items)).Length; i++)
            {
                Vector3 itemPosition = new Vector3(i + Enum.GetNames(typeof(Inventory.RangedWeapons)).Length, -10 - (Mathf.Abs(netId.Value * 10)), 0);
                GameObject itemObject = Instantiate((GameObject)Resources.Load("Prefabs/Items/" + Enum.GetName(typeof(Inventory.Items), i)), itemPosition, Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(itemObject, connectionToClient);
                TargetPutItemInInventoryDictionary(connectionToClient, itemObject, false, false, 0);
            }
        }
        if (level == 3)
        {
            Vector3 itemPosition = new Vector3(Enum.GetNames(typeof(Inventory.RangedWeapons)).Length + Enum.GetNames(typeof(Inventory.Items)).Length, -10 - (Mathf.Abs(netId.Value * 10)), 0);
            GameObject itemObject = Instantiate((GameObject)Resources.Load("Prefabs/Items/Timer"), itemPosition, Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(itemObject, connectionToClient);
            TargetPutItemInInventoryDictionary(connectionToClient, itemObject, false, false, 0);
            RpcSetPlayerNetId(itemObject, playerNetId);
            for (int i = 0; i < 4; i++)
            {
                itemPosition = new Vector3(1 + i + Enum.GetNames(typeof(Inventory.RangedWeapons)).Length + Enum.GetNames(typeof(Inventory.Items)).Length, -10 - (Mathf.Abs(netId.Value * 10)), 0);
                itemObject = Instantiate((GameObject)Resources.Load("Prefabs/Items/C4" + (i + 1)), itemPosition, Quaternion.identity);
                NetworkServer.SpawnWithClientAuthority(itemObject, connectionToClient);
                TargetPutItemInInventoryDictionary(connectionToClient, itemObject, false, false, 0);
                RpcSetPlayerNetId(itemObject, playerNetId);
            }
            //Golfclub
            itemPosition = new Vector3(5 + Enum.GetNames(typeof(Inventory.RangedWeapons)).Length + Enum.GetNames(typeof(Inventory.Items)).Length, -10 - (Mathf.Abs(netId.Value * 10)), 0);
            itemObject = Instantiate((GameObject)Resources.Load("Prefabs/Items/GolfClub"), itemPosition, Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(itemObject, connectionToClient);
            TargetPutItemInInventoryDictionary(connectionToClient, itemObject, false, false, 0);
            RpcSetPlayerNetId(itemObject, playerNetId);
        }
    }
    /// <summary>
    /// TargetRpc calls for the client it gets fired from, adds items and weapons in the ItemsInLevel List
    /// </summary>
    /// <param name="target"></param>
    /// <param name="weapon"></param>
    /// <param name="UI"></param>
    [TargetRpc]
    private void TargetPutItemInInventoryDictionary(NetworkConnection target, GameObject weapon, bool UI, bool respawn, int respawnNumber)
    {
        string weaponName = weapon.name;
        if (weaponName.Length > 7 && weaponName.Substring((weaponName.Length - 7), 7) == "(Clone)")
        {
            weaponName = weaponName.Substring(0, weaponName.Length - 7);
        }
        if (!respawn)
        {
            GetComponent<Player>().inventory.itemsInLevel.Add(weaponName, weapon);
        }
        else
        {
            GetComponent<Player>().inventory.itemsInLevel[weaponName] = weapon;
        }
        if(UI)
        {
            if (!respawn)
            {
                GameManager.instance.uiManager.totalAmmoText.Add(weaponName, weapon.transform.Find("Canvas").transform.Find("TotalAmmo").transform.Find("TotalAmmoCount").GetComponent<Text>());
                GameManager.instance.uiManager.currentAmmoText.Add(weaponName, weapon.transform.Find("Canvas").transform.Find("CurrentAmmo").transform.Find("CurrentCount").GetComponent<Text>());
            }
            else
            {
                GameManager.instance.uiManager.totalAmmoText[weaponName] = weapon.transform.Find("Canvas").transform.Find("TotalAmmo").transform.Find("TotalAmmoCount").GetComponent<Text>();
                GameManager.instance.uiManager.currentAmmoText[weaponName] = weapon.transform.Find("Canvas").transform.Find("CurrentAmmo").transform.Find("CurrentCount").GetComponent<Text>();
            }
        }
        if (respawn)
        {
            if (GameManager.instance.localPlayer.GetComponent<Player>().inventory.respawnNumber == respawnNumber)
            {
                GameManager.instance.localPlayer.GetComponent<Player>().inventory.QuickChangeWeapon();
            }
        }

    }

    [ClientRpc]
    private void RpcSetPlayerNetId(GameObject theObject, int playerNetId)
    {
        theObject.GetComponent<Item>().playerNetId = playerNetId;
    }

    [Command]
    public void CmdRespawnWeapon(string weaponName, float xPos, int playerNetId, int respawnNumber)
    {
        Vector3 weaponPosition = new Vector3(xPos, -10 - (Mathf.Abs(netId.Value * 10)), 0);
        GameObject weaponObject = Instantiate((GameObject)Resources.Load("Prefabs/Weapons/" + weaponName), weaponPosition, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(weaponObject, connectionToClient);
        if (weaponName != "Bat" && weaponName != "minigun" && weaponName != "speargun" &&
            weaponName != "Portal_gun" && SceneManager.GetActiveScene().name != "Level 1")
        {
            TargetPutItemInInventoryDictionary(connectionToClient, weaponObject, true, true, respawnNumber);
        }
        else
        {
            TargetPutItemInInventoryDictionary(connectionToClient, weaponObject, false, true, respawnNumber);
        }
        if (SceneManager.GetActiveScene().name == "Level 3")
        {
            RpcSetPlayerNetId(weaponObject, playerNetId);
        }
    }
}