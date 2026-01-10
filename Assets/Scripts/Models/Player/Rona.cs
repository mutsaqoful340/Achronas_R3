using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

[Serializable]
public class Rona
{
    public int Uid;
    public string Name;
    public string Description;
    public int Speed;

    public UnitPoint Stamina;
    public UnitPoint HealthPoint;
    public int DefencePower;
    // Stamina drain/regeneration settings (units per second)
    public float StaminaDrainRate = 10f;
    public float StaminaRegenRate = 5f;
    private float _staminaFraction = 0f;
    private float _staminaRegenFraction = 0f;

    public PlayerState _playerState;
    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Attacking,
        Dead
    }

    // Strenght (HealthPoint, AttackPower, DefencePower)
    // Intelligence (MagicPoint, AttackPower)
    // TotalExp

    // Properties <- Encapsulation (Teori)

    [Serializable]
    public struct UnitPoint
    {
        public int Current;
        public int Maximum;

        public override string ToString()
        {
            return $"{Current} / {Maximum}";
        }
    }

    public Rona()
    {
        Uid = 1;
        Name = "Nobel Knight";
        Description = "A holy knight form a far land";
        Speed = 5;
        Stamina.Maximum = 100;
        Stamina.Current = 100;
        // sensible defaults so HealthPoint has usable values
        HealthPoint.Maximum = 100;
        HealthPoint.Current = 100;
        DefencePower = 0;
    }

    // Whether the character currently has any stamina left to run
    public bool CanRun => Stamina.Current > 0;

    // Call each frame (pass Time.deltaTime). When player state is Running,
    // stamina decreases by StaminaDrainRate units per second.
    public void UpdateStamina(float deltaTime)
    {
        if (Stamina.Maximum <= 0) return;

        if (_playerState == PlayerState.Running)
        {
            // when running, drain stamina
            // reset regen accumulator so we don't mix fractional values
            _staminaRegenFraction = 0f;
            _staminaFraction += StaminaDrainRate * deltaTime;
            int whole = (int)_staminaFraction;
            if (whole > 0)
            {
                Stamina.Current = Math.Max(0, Stamina.Current - whole);
                _staminaFraction -= whole;
            }
        }
        else
        {
            // when not running, regenerate stamina up to maximum
            _staminaFraction = 0f;
            if (Stamina.Current < Stamina.Maximum)
            {
                _staminaRegenFraction += StaminaRegenRate * deltaTime;
                int whole = (int)_staminaRegenFraction;
                if (whole > 0)
                {
                    Stamina.Current = Math.Min(Stamina.Maximum, Stamina.Current + whole);
                    _staminaRegenFraction -= whole;
                }
            }
        }
    }

    public UnitPoint GetStamina()
    {
        return Stamina;
    }

    public float Damage
    {
        set
        {
            var damage = value - DefencePower;
            HealthPoint.Current -= (int)damage;

            if (HealthPoint.Current <= 0)
            {
                HealthPoint.Current = 0; // prevent negative values
                _playerState = PlayerState.Dead;
            }

            if (HealthPoint.Current > HealthPoint.Maximum)
            {
                HealthPoint.Current = HealthPoint.Maximum;
            }
        }
    }
}