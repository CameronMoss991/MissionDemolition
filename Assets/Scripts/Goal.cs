using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(Renderer))]
public class Goal : MonoBehaviour {
// A static field accessible by code anywhere
static public bool goalMet = false;

void OnTriggerEnter ( Collider other ) {
    Projectile proj = other.GetComponent<Projectile>();
    if (proj != null) {
        Goal.goalMet = true;

        // Visual Feedback: Turn the goal Green!
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = Color.green;
    }
}
}