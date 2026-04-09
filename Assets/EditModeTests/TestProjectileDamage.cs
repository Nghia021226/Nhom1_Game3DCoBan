using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace EditModeTests
{
    public class TestProjectileDamage
    {
        readonly List<Object> m_TestObjects = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object obj in m_TestObjects) if (obj != null) Object.DestroyImmediate(obj);
            m_TestObjects.Clear();
        }

        [Test]
        public void TestDamage_Logic()
        {
            float health = 100f;
            float damage = 35f;

            health -= damage;

            Assert.AreEqual(65f, health, "Máu phải còn 65 sau khi nhận 35 sát thương");
        }
    }
}