using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJ
{
    public class PlayerInventory : MonoBehaviour
    {
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;
        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[2];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[2];

        public int currRightWeaponIdx = 0;
        public int currLeftWeaponIdx = 0;

        public List<WeaponItem> weaponsInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }
        private void Start()
        {

            rightWeapon = weaponsInRightHandSlots[0];
            leftWeapon = weaponsInLeftHandSlots[0];
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeRightWeapon()
        {
            currRightWeaponIdx++;
            if(currRightWeaponIdx == 0 && weaponsInRightHandSlots[0] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currRightWeaponIdx];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
            else if(currRightWeaponIdx == 0 && weaponsInRightHandSlots[0] == null)
            {
                currRightWeaponIdx++;
            }
            else if(currRightWeaponIdx == 1 && weaponsInRightHandSlots[1] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currRightWeaponIdx];
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
            else
            {
                currRightWeaponIdx++;
            }
            if (currRightWeaponIdx > weaponsInRightHandSlots.Length - 1)
            {
                currRightWeaponIdx = -1;
                rightWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            }
        }

        public void ChangeLeftWeapon()
        {
            currLeftWeaponIdx++;
            if (currLeftWeaponIdx == 0 && weaponsInLeftHandSlots[0] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currLeftWeaponIdx];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }
            else if (currLeftWeaponIdx == 0 && weaponsInLeftHandSlots[0] == null)
            {
                currLeftWeaponIdx++;
            }
            else if (currLeftWeaponIdx == 1 && weaponsInLeftHandSlots[1] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currLeftWeaponIdx];
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }
            else
            {
                currLeftWeaponIdx++;
            }
            if (currLeftWeaponIdx > weaponsInLeftHandSlots.Length - 1)
            {
                currLeftWeaponIdx = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
            }

        }
    }
}

