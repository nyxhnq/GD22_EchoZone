using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] public GameObject player;
    [SerializeField] private float damageRange;
    [SerializeField] public float damageSet = 25f;
    [SerializeField] public float minDamage;
    [SerializeField] public float maxDamage;

    [SerializeField] public bool randomDamage;
    [SerializeField] public bool setDamage;

    [SerializeField] public AudioClip[] sounds;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        damageRange = Random.Range(minDamage, maxDamage);
        audioSource = player.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && randomDamage)
        {
            player.GetComponent<PlayerHealth>().health -= damageRange;
            audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.Play();
        }

        if (other.gameObject.tag == "Player" && setDamage)
        {
            player.GetComponent<PlayerHealth>().health -= damageSet;
            audioSource.clip = sounds[Random.Range(0, sounds.Length)];
            audioSource.Play();

        }

        void Update()
        {

        }
    }
}
