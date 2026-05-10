using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    [SerializeField] public float CurrentHealth;
    [SerializeField] private float MaxHealth = 100f;
    PlayerHealth player;

    void Start()
    {
        healthBar = GetComponent<Image>();
        player = FindObjectOfType<PlayerHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentHealth = player.health;
        healthBar.fillAmount = CurrentHealth / MaxHealth; 
    }
}
