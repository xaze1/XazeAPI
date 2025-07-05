// Copyright (c) 2025 xaze_
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// 
// I <3 🦈s :3c

namespace EclipsePlugin.API.CustomModules
{
    using PlayerRoles;
    using PlayerStatsSystem;
    using UnityEngine;

    public class CustomHealthStat : HealthStat
    {
        public override float MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (value != _maxValue)
                {
                    MaxValueDirty = true;
                    if (_maxValue < value)
                    {
                        float healthPercentage = Mathf.Max(CurValue / _maxValue, 0);

                        CurValue = healthPercentage * value;
                    }
                    _maxValue = value;
                    CurValue = Mathf.Min(CurValue, _maxValue);
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (CurValue <= 0 && Hub.IsAlive() && Hub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Scp079)
            {
                Hub.playerStats.KillPlayer(new UniversalDamageHandler(-1f, DeathTranslations.Unknown));
                return;
            }

            if (CurValue < MaxValue)
            {
                return;
            }

            CurValue -= Mathf.Max(((CurValue - MaxValue) / 2.5f), 1) * Time.deltaTime;

            if (CurValue > MaxValue)
            {
                return;
            }

            CurValue = MaxValue;
        }

        public void ResetMaxHp(bool resetHp = true)
        {
            float defaultValue = 100;

            if (Hub.roleManager.CurrentRole is IHealthbarRole healthbar)
            {
                defaultValue = healthbar.MaxHealth;
            }

            float prevHp = CurValue;
            MaxValue = defaultValue;

            if (resetHp)
            {
                return;
            }

            CurValue = prevHp;
        }
    }
}
