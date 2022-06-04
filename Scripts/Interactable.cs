using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JJ
{
    public class Interactable : MonoBehaviour
    {
        public float radius = 0.6f;
        public string interactableText;
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        
        public virtual void Interact(PlayerManager playerManager)
        {
            //Called when player Interacts
            Debug.Log("You have interacted with an object!");
        }
    }
}

