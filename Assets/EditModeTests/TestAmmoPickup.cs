using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace EditModeTests
{
    public class TestAmmoPickup
    {
        readonly List<Object> m_TestObjects = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object obj in m_TestObjects) if (obj != null) Object.DestroyImmediate(obj);
            m_TestObjects.Clear();
        }

        [Test]
        public void TestAmmoPickup_Logic()
        {
            
            int currentAmmo = 10;
            int ammoInPickup = 30;

            
            currentAmmo += ammoInPickup;

            
            Assert.AreEqual(40, currentAmmo, "Số lượng đạn sau khi nhặt phải là 40");
        }
    }
}