using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour
{
   
}

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public GameObject hud;
    [SerializeField] public GameObject inv;
    [SerializeField] public GameObject deathScreen;
    [SerializeField] public GameObject player;

    [SerializeField] public float health = 100f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            // Ensure PlayerMovement is a valid component
            var playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            hud.SetActive(false);
            inv.SetActive(false);
            deathScreen.SetActive(true);
        }

        if (health > 100)
        {
            health = 100f;
        }
    }
}
