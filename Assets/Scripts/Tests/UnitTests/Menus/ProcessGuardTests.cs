using GlowCore.UI.Menus;
using NUnit.Framework;

public class ProcessGuardTests
{
    [SetUp]
    public void SetUp() => ProcessGuard.ResetForTests();

    [TearDown]
    public void TearDown() => ProcessGuard.ResetForTests();

    [Test]
    public void TryAcquireOnce_ReturnsTrue_WhenGuardAcquires()
    {
        var guard = new FakeSingleInstanceGuard { TryAcquireResult = true };

        bool acquired = ProcessGuard.TryAcquireOnce(guard);

        Assert.IsTrue(acquired);
        Assert.AreEqual(1, guard.TryAcquireCallCount);
        Assert.AreEqual(0, guard.DisposeCallCount);
    }

    [Test]
    public void TryAcquireOnce_ReturnsFalse_AndDisposesGuard_WhenGuardRefuses()
    {
        var guard = new FakeSingleInstanceGuard { TryAcquireResult = false };

        bool acquired = ProcessGuard.TryAcquireOnce(guard);

        Assert.IsFalse(acquired);
        Assert.AreEqual(1, guard.TryAcquireCallCount);
        Assert.AreEqual(1, guard.DisposeCallCount);
    }

    [Test]
    public void TryAcquireOnce_IsIdempotent_AfterFirstSuccess()
    {
        var firstGuard = new FakeSingleInstanceGuard { TryAcquireResult = true };
        ProcessGuard.TryAcquireOnce(firstGuard);

        var secondGuard = new FakeSingleInstanceGuard { TryAcquireResult = false };
        bool acquired = ProcessGuard.TryAcquireOnce(secondGuard);

        Assert.IsTrue(acquired, "Second call must succeed because the process already owns the guard.");
        Assert.AreEqual(0, secondGuard.TryAcquireCallCount, "Second guard must not be queried.");
    }

    [Test]
    public void TryAcquireOnce_HandlesNullGuard_AsAcquired()
    {
        bool acquired = ProcessGuard.TryAcquireOnce(null);
        Assert.IsTrue(acquired);
    }
}
