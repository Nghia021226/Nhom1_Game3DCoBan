using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


namespace EditModeTests
{
    public class MockHealth
    {
        public float MaxHealth;
        public float CurrentHealth;
        public bool Invincible;

        bool m_IsDead;

        public bool IsDead => m_IsDead;

        public MockHealth(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(float damage, GameObject damageSource)
        {
            if (Invincible) return;

            CurrentHealth -= damage;
            CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, MaxHealth);

            HandheldDeath();
        }

        public void Kill()
        {
            CurrentHealth = 0f;
            HandheldDeath();
        }

        public bool canPickup() => CurrentHealth < MaxHealth;

        public float GetRatio() => CurrentHealth / MaxHealth;

        public bool IsCritical() => GetRatio() <= 0.3f;

        void HandheldDeath()
        {
            if (m_IsDead) return;

            if (CurrentHealth <= 0f)
            {
                m_IsDead = true;
            }
        }

    }

    public class MockWeaponController
    {
        public string WeaponName;
        public GameObject SourcePrefab;
        public int ClipSize;
        public int CurrentAmmo;
        public bool IsWeaponActive;
        public GameObject Owner;

        public MockWeaponController(string weaponName, int clipSize, GameObject sourcePrefab)
        {
            WeaponName = weaponName;
            SourcePrefab = sourcePrefab;
            ClipSize = clipSize;
            CurrentAmmo = clipSize;
        }

        public void ShowWeapon(bool show)
        {
            IsWeaponActive = show;
        }
    }

    public class MockPlayerWeaponsManager
    {
        MockWeaponController[] m_WeaponSlots = new MockWeaponController[9];
        public int ActiveWeaponIndex { get; private set; } = -1;

        public bool AddWeapon(MockWeaponController weaponPrefab)
        {
            if(HasWeapon(weaponPrefab) != null)
            {
                return false;
            }

            for (int i = 0; i < m_WeaponSlots.Length; i++)
            {
                if (m_WeaponSlots[i] == null)
                {
                    m_WeaponSlots[i] = weaponPrefab;
                    return true;
                }
            }
            return false;
        }

        public MockWeaponController GetWeaponAtSlotIndex(int index)
        {
            if(index >= 0 && index < m_WeaponSlots.Length)
            {
                return m_WeaponSlots[index];
            }
            return null;
        }

        public MockWeaponController HasWeapon(MockWeaponController weaponPrefab)
        {
            for(int i = 0; i< m_WeaponSlots.Length; i++)
            {
                if (m_WeaponSlots[i] != null && m_WeaponSlots[i].SourcePrefab == weaponPrefab.SourcePrefab)
                {
                    return m_WeaponSlots[i];
                }
            }
            return null;
        }

        public bool RemoveWeapon(MockWeaponController weaponInstance)
        {
            for (int i = 0; i < m_WeaponSlots.Length; i++)
            {
                if (m_WeaponSlots[i] == weaponInstance)
                {
                    m_WeaponSlots[i] = null;
                    return true;
                }
            }
            return false;
        }
    }

    public class MockEnemyKillEvent 
    {
        public GameObject Enemy;
        public int RemainingEnemyCount;
    }

    public class MockObjectiveKillEnemies
    {
        public bool MustKillAllEnemies = true;
        public int KillsToCompleteObjective = 5;

        public bool IsCompleted {  get; private set; }

        int m_KillTotal;

        public int KillTotal => m_KillTotal;

        public void OnEnemyKilled(MockEnemyKillEvent evt)
        {
            if (IsCompleted)
            {
                return;
            }
            m_KillTotal++;

            if (MustKillAllEnemies)
            {
                KillsToCompleteObjective = evt.RemainingEnemyCount + m_KillTotal;
            }

            int targetRemaining = MustKillAllEnemies ? evt.RemainingEnemyCount : KillsToCompleteObjective - m_KillTotal;

            if(targetRemaining == 0)
            {
                IsCompleted = true;
            }
        }
    }

    public class FPSMicrogamesTests
    {
        readonly System.Collections.Generic.List<Object> m_TestObjects = new System.Collections.Generic.List<Object>();

        [TearDown]
        public void TearDowm()
        {
            foreach(Object obj in m_TestObjects)
            {
                if(obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            m_TestObjects.Clear();
        }
    }
}

