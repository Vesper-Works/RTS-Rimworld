using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart
{
    private float coverage;
    private float sight;
    private float consciousness;
    private float breathing;
    private float manipulation;
    private float moving;

    private bool vital;
    private string name;
    private float health;
    private float currentHealth;
    public List<BodyPart> attachedBodyParts = new List<BodyPart>();

    public float Health { get => health; set { currentHealth = value; health = value; } }
    public float Coverage
    {
        get => coverage;
        set
        {
            if (value > 1) { coverage = Mathf.Clamp01(value / 100f); }
            else { coverage = value; }
        }
    }

    public string Name { get => name; set => name = value; }
    public float Sight { get => sight; set => sight = value; }
    public float Consciousness { get => consciousness; set => consciousness = value; }
    public float Breathing { get => breathing; set => breathing = value; }
    public float Manipulation { get => manipulation; set => manipulation = value; }
    public float Moving { get => moving; set => moving = value; }
    public bool Vital { get => vital; set => vital = value; }

    public bool TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            return true;
        }
        return false;
    }
}
