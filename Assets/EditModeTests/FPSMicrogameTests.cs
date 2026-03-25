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
            foreach (Object obj in m_TestObjects)
            {
                if (obj != null)
                {
                    Object.DestroyImmediate(obj);
                }
            }
            m_TestObjects.Clear();
        }

        [Test]
        public void TestScore()
        {
            MockObjectiveKillEnemies objective = new MockObjectiveKillEnemies();
            objective.MustKillAllEnemies = false;
            objective.KillsToCompleteObjective = 3;

            Assert.AreEqual(0, objective.KillTotal);
            Assert.IsFalse(objective.IsCompleted);

            MockEnemyKillEvent evt1 = new MockEnemyKillEvent { Enemy = null, RemainingEnemyCount = 2 };
            objective.OnEnemyKilled(evt1);
            Assert.AreEqual(1, objective.KillTotal);
            Assert.IsFalse(objective.IsCompleted);

            MockEnemyKillEvent evt2 = new MockEnemyKillEvent { Enemy = null, RemainingEnemyCount = 1 };
            objective.OnEnemyKilled(evt2);
            Assert.AreEqual(2, objective.KillTotal);
            Assert.IsFalse(objective.IsCompleted);

            MockEnemyKillEvent evt3 = new MockEnemyKillEvent { Enemy = null, RemainingEnemyCount = 0 };
            objective.OnEnemyKilled(evt3);
            Assert.AreEqual(3, objective.KillTotal);
            Assert.IsTrue(objective.IsCompleted);
        }

        [Test]
        public void TestPlayerHealth()
        {
            MockHealth health = new MockHealth(100f);
            Assert.AreEqual(100f,health.CurrentHealth);
            Assert.IsFalse(health.IsDead);

            health.TakeDamage(30f,null);
            Assert.AreEqual(70f, health.CurrentHealth);
            Assert.IsFalse(health.IsDead);

            health.TakeDamage(80f, null);
            Assert.AreEqual(0f, health.CurrentHealth);
            Assert.IsTrue(health.IsDead);

            health.TakeDamage(10f, null);
            Assert.AreEqual(0f,health.CurrentHealth);
            Assert.IsTrue(health.IsDead);
        }

        [Test]
        public void TestWeaponInventory()
        {
            MockPlayerWeaponsManager weaponsManager = new MockPlayerWeaponsManager();
            Assert.IsNull(weaponsManager.GetWeaponAtSlotIndex(0));

            GameObject blasterPrefab = new GameObject("BlasterPrefab");
            m_TestObjects.Add(blasterPrefab);
            MockWeaponController balster = new MockWeaponController("Blaster", 7, blasterPrefab);
            bool added = weaponsManager.AddWeapon(balster);
            Assert.IsTrue(added);
            Assert.IsNotNull(weaponsManager.GetWeaponAtSlotIndex(0));
            Assert.AreEqual("Blaster",weaponsManager.GetWeaponAtSlotIndex(0).WeaponName);
            Assert.IsNotNull(weaponsManager.HasWeapon(balster));

            bool duplicate = weaponsManager.AddWeapon(balster);
            Assert.IsFalse(duplicate);

            GameObject shotgunPrefab = new GameObject("ShotgunPrefab");
            m_TestObjects.Add(shotgunPrefab);
            MockWeaponController shotgun = new MockWeaponController("Shotgun", 5, shotgunPrefab);
            Assert.IsNull(weaponsManager.HasWeapon(shotgun));
            weaponsManager.AddWeapon(shotgun);
            Assert.IsNotNull(weaponsManager.HasWeapon(shotgun));
            Assert.AreEqual("Shotgun",weaponsManager.GetWeaponAtSlotIndex(1).WeaponName);

            bool removed = weaponsManager.RemoveWeapon(balster);
            Assert.IsTrue(removed);
            Assert.IsNull(weaponsManager.GetWeaponAtSlotIndex(0));
        }
    }
}

