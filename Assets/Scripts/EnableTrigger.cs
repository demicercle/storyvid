using System;
using UnityEngine;
using UnityEngine.Events;

public class EnableTrigger : MonoBehaviour
{
        public UnityEvent onEnable, onDisable;

        private void OnEnable()
        {
                onEnable.Invoke();
        }

        void OnDisable()
        {
                onDisable.Invoke();
        }
}