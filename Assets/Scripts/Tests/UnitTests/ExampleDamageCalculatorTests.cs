using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;

public class ExampleDamageCalculatorTests
{
    private const float kEpsilon = 0.001f;
        
    private GameObject m_DamageGO;
    private DamageCalculator m_DamageCalculator;

    [SetUp]
    public void Setup()
    {
        m_DamageGO = new();
        m_DamageCalculator = m_DamageGO.AddComponent<DamageCalculator>();
        m_DamageCalculator.Weapon = new MockWeapon();
        m_DamageCalculator.DamageMultiplier = 1.5f;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(m_DamageGO);
    }

    [Test]
    public void CalculateDamage_Works()
    {
        // Arrange
        float expectedDamage = m_DamageCalculator.Weapon.BaseDamage * m_DamageCalculator.DamageMultiplier;
        
        // Act
        float damage = m_DamageCalculator.CalculateDamage();
        
        // Assert
        Assert.AreEqual(expectedDamage, damage, kEpsilon);
    }
    
    [Test]
    public void UnitTestThatShouldFail() // Comment out to test if testing works
    {
        Assert.AreEqual(true, false);
    }
}
