using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJ
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject modelPrefab;
        public bool isUnarmed;


        [Header("Idle Animations")]
        public string right_Hand_Idle;
        public string left_Hand_Idle;
        public string th_Idle;
        [Header("Attack Animations")]
        public string OH_Light_Attack_01;
        public string OH_Light_Attack_02;
        public string OH_Heavy_Attack_01;

        public string TH_Light_Attack_01;
        public string TH_Light_Attack_02;
        public string TH_Heavy_Attack_01;

        [Header("Stamina Costs")]
        public int baseStaminaCost;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;
    }
}

