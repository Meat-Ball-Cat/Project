using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.HealthSystem
{
    public class Damage
    {
        private readonly Dictionary<DamageType, float> _damages;

        public Damage(params (DamageType, float)[] damages)
        {
            _damages = damages
                .ToDictionary(damage => damage.Item1, damage => damage.Item2);
        }

        public float GetDamageValue(params DamageType[] type)
            => type.Sum(dt => _damages[dt]);

        public float GetDamageValueExcept(params DamageType[] except)
            => ((DamageType[])Enum.GetValues(typeof(DamageType)))
                .Where(dt => !except.Contains(dt))
                .Sum(dt => _damages[dt]);
    }

    public enum DamageType
    {
        Explosion,
        Collision,
    }
}