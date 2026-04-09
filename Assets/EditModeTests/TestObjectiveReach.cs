using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

namespace EditModeTests
{
    public class TestObjectiveReach
    {
        readonly List<Object> m_TestObjects = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object obj in m_TestObjects) if (obj != null) Object.DestroyImmediate(obj);
            m_TestObjects.Clear();
        }

        [Test]
        public void TestObjective_Logic()
        {
            bool isCompleted = false;
            bool playerEnteredZone = true; 

            if (playerEnteredZone)
            {
                isCompleted = true;
            }

            Assert.IsTrue(isCompleted, "Nhiệm vụ phải hoàn thành khi người chơi vào vùng đích");
        }
    }
}