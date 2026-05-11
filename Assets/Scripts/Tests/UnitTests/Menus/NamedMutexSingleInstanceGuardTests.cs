using System;
using System.Threading;
using GlowCore.UI.Menus;
using NUnit.Framework;

public class NamedMutexSingleInstanceGuardTests
{
    private string m_mutexName;

    [SetUp]
    public void SetUp() => m_mutexName = "GlowCore.Test." + Guid.NewGuid().ToString("N");

    [Test]
    public void TryAcquire_Succeeds_OnFreshName()
    {
        using var guard = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(guard.TryAcquire());
    }

    [Test]
    public void TryAcquire_Fails_WhenAnotherGuardOwnsTheName()
    {
        using var first = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(first.TryAcquire(), "first guard must own the mutex");

        // Named mutexes are re-entrant per thread — to reproduce cross-process behavior the
        // contending acquire must run on a different thread.
        var secondAcquired = true;
        var contender = new Thread(() =>
        {
            using var second = new NamedMutexSingleInstanceGuard(m_mutexName);
            secondAcquired = second.TryAcquire();
        });
        contender.Start();
        contender.Join();

        Assert.IsFalse(secondAcquired, "second guard must be refused while first holds the mutex");
    }

    [Test]
    public void TryAcquire_Succeeds_AfterPreviousOwnerReleases()
    {
        var first = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(first.TryAcquire());
        first.Release();

        using var second = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(second.TryAcquire(), "second guard must acquire once first has released");
    }

    [Test]
    public void TryAcquire_Succeeds_AfterPreviousOwnerDisposes()
    {
        var first = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(first.TryAcquire());
        first.Dispose();

        using var second = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(second.TryAcquire());
    }

    [Test]
    public void TryAcquire_IsIdempotent_OnSameInstance()
    {
        using var guard = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.IsTrue(guard.TryAcquire());
        Assert.IsTrue(guard.TryAcquire(), "calling TryAcquire again on the same owning instance must remain true");
    }

    [Test]
    public void Release_IsSafe_WhenNeverAcquired()
    {
        using var guard = new NamedMutexSingleInstanceGuard(m_mutexName);
        Assert.DoesNotThrow(() => guard.Release());
    }

    [Test]
    public void Dispose_IsSafe_WhenCalledTwice()
    {
        var guard = new NamedMutexSingleInstanceGuard(m_mutexName);
        guard.TryAcquire();
        guard.Dispose();
        Assert.DoesNotThrow(() => guard.Dispose());
    }
}
