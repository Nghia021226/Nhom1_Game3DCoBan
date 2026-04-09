using NUnit.Framework;
using UnityEngine;

public class ScoreTest
{
    [Test]
    public void CongDiem_DungKetQua()
    {
        int a = 2;
        int b = 3;
        int result = a + b;

        Assert.AreEqual(5, result);
    }
}