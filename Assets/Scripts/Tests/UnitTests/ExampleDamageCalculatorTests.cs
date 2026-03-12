using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class ExampleDamageCalculatorTests
{
    private const float kEpsilon = 0.001f;
        
    private GameObject m_damageGameObj;
    private DamageCalculator m_damageCalculator;

    [SetUp]
    public void Setup()
    {
        m_damageGameObj = new();
        m_damageCalculator = m_damageGameObj.AddComponent<DamageCalculator>();
        m_damageCalculator.Weapon = new MockWeapon();
        m_damageCalculator.DamageMultiplier = 1.5f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(m_damageGameObj);
    }

    [Test]
    public void CalculateDamage_Works()
    {
        float expectedDamage = m_damageCalculator.Weapon.BaseDamage * m_damageCalculator.DamageMultiplier;
        
        float damage = m_damageCalculator.CalculateDamage();
        
        Assert.AreEqual(expectedDamage, damage, kEpsilon);
    }
}
