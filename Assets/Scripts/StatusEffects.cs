using System;
using System.Collections.Generic;

public class StatusEffect
{
    public string EffectType;
    private DateTime LastApplied;
    private float DPS;
    // the number of seconds the status should last for
    private float Duration;
    // threshold is in seconds
    // when the level exceeds the threshold, damage will be applied until the level is below the threshold again
    public float Threshold = 1.0f;
    // the amount of the status effect that has been accumulated
    public float Level = 0.0f;
    // the level will not decrease while exposed is true
    private bool Exposed = false;

    public StatusEffect(string effectType, int duration, float dps, float threshold = 1.0f)
    {
        EffectType = effectType;
        DPS = dps;
        Duration = duration;
        Threshold = threshold;
    }

    public void Apply() {
        LastApplied = DateTime.Now;
        Exposed = true;
    }

    public float Update(float dt) {
        if ((DateTime.Now - LastApplied).TotalSeconds > 3.0f) {
            Exposed = false;
        }
        if (Exposed) {
            Level += dt;
            if (Level > Threshold) {
                Level = Threshold;
            }
        } else if (!Exposed && Level > 0) {
            Level -= dt;
            if (Level < 0) {
                Level = 0;
            }
        }
        if (IsActive()) {
            return DPS * dt;
        }
        return 0;
    }

    private bool IsActive() {
        if (LastApplied != null) {
            return Level >= Threshold && (DateTime.Now - LastApplied).TotalSeconds < Duration; 
        }
        return false;
    }
}

public class StatusEffectsManager
{
    public Dictionary<string, StatusEffect> statusEffects = new Dictionary<string, StatusEffect>();

    public StatusEffectsManager() {
        statusEffects["fire"] = new StatusEffect("fire", 10, 1, 0.1f);
        statusEffects["poison"] = new StatusEffect("poison", 5, 2, 3.0f);
    }
}