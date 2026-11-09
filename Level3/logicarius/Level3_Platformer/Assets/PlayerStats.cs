using System;
using UnityEngine;

// Put this on the Player. Keeps the rock count and the score. Never reset on death.
public class PlayerStats : MonoBehaviour {
    public int maxRocks = 3;
    public bool showDebug = true;      // on-screen text until the real HUD exists (Part 5)

    public int Rocks { get; private set; }
    public int Score { get; private set; }
    public event Action Changed;       // the HUD listens to this

    public bool TryAddRock() {
        if (Rocks >= maxRocks) return false;
        Rocks++;
        if (Changed != null) Changed();
        return true;
    }

    public void AddBonus(int points) {
        Score += points;
        if (Changed != null) Changed();
    }

    // burned: all rocks are lost (the score stays). Returns how many were lost.
    public int LoseAllRocks() {
        int lost = Rocks;
        Rocks = 0;
        if (Changed != null) Changed();
        return lost;
    }

    // worms only give points when the stomach is full of rocks; rocks are NOT used up
    public bool TryEatWorm(int points) {
        if (Rocks < maxRocks) return false;
        Score += points;
        if (Changed != null) Changed();
        return true;
    }

    void OnGUI() {
        if (!showDebug) return;
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 500, 40), "Rocks: " + Rocks + "/" + maxRocks + "     Score: " + Score, style);
    }
}