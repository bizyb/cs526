using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {


    static PlayerHealth instance;
    public static PlayerHealth Instance { get { return instance; } }

    public SimpleHealthBar healthBar;
    public SimpleHealthBar sheildBar;
    private GameManager game;
    float maxHealth = 100;
    float currentHealth = 0;



    void Awake()
    {
        // If the instance variable is already assigned, then there are multiple player health scripts in the scene. Inform the user.
        if (instance != null)
            Debug.LogError("There are multiple instances of the Player Health script. Assigning the most recent one to Instance.");

        // Assign the instance variable as the Player Health script on this object.
        instance = GetComponent<PlayerHealth>();
    }

    void Start()
    {
        // Set the current health max
        currentHealth = maxHealth;

        // Update the Simple Health Bar with the updated value of Health
        healthBar.UpdateBar(currentHealth, maxHealth);
        sheildBar.UpdateBar(currentHealth, maxHealth);
        game = GameManager.Instance;

    }

    // Update is called once per frame
    void Update() {}

    public void UpdateHealth(float points, Tap tapInstance) {

        currentHealth += points;
        if (currentHealth > maxHealth) { currentHealth = maxHealth; }
        if (currentHealth <= 0f) {
            currentHealth = 0f;
            tapInstance.Dead();
        }
        healthBar.UpdateBar(currentHealth, maxHealth);
        sheildBar.UpdateBar(currentHealth, maxHealth);

    }

}
