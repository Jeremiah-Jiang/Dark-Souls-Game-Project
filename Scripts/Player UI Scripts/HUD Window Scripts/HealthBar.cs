using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JJ
{
    public class HealthBar : MonoBehaviour
    {
        private Slider slider;

        private void Awake()
        {
            slider = GetComponent<Slider>();
            if (slider == null)
            {
                Debug.LogError("Slider is NULL");
            }
        }
        public void SetMaxHealth(int maxHealth)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
        public void SetCurrentHealth(int currentHealth)
        {
            slider.value = currentHealth;
        }
    }
}

