using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JJ
{
    public class StaminaBar : MonoBehaviour
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
        public void SetMaxStamina(int maxStamina)
        {
            slider.maxValue = maxStamina;
            slider.value = maxStamina;
        }
        public void SetCurrentStamina(int currentStamina)
        {
            slider.value = currentStamina;
        }
    }
}

