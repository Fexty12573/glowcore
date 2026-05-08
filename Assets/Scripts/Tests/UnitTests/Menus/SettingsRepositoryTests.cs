using GlowCore.UI.Menus;
using NUnit.Framework;
using Tests.Mocks.Menus;

namespace Tests.UnitTests.Menus
{
    public class SettingsRepositoryTests
    {
        private FakePlayerPrefsBackend m_backend;
        private PlayerPrefsSettingsRepository m_repository;

        [SetUp]
        public void SetUp()
        {
            m_backend = new FakePlayerPrefsBackend();
            m_repository = new PlayerPrefsSettingsRepository(m_backend);
        }

        [Test]
        public void GetFloat_NoKey_ReturnsFallback()
        {
            Assert.AreEqual(0.42f, m_repository.GetFloat("missing", 0.42f));
        }

        [Test]
        public void SetFloat_ThenGet_RoundTrips()
        {
            m_repository.SetFloat("vol", 0.75f);
            Assert.AreEqual(0.75f, m_repository.GetFloat("vol", 0f));
        }

        [Test]
        public void GetInt_NoKey_ReturnsFallback()
        {
            Assert.AreEqual(7, m_repository.GetInt("missing", 7));
        }

        [Test]
        public void SetInt_ThenGet_RoundTrips()
        {
            m_repository.SetInt("res", 3);
            Assert.AreEqual(3, m_repository.GetInt("res", 0));
        }

        [Test]
        public void GetBool_NoKey_ReturnsFallback()
        {
            Assert.IsTrue(m_repository.GetBool("missing", true));
            Assert.IsFalse(m_repository.GetBool("missing", false));
        }

        [Test]
        public void SetBool_True_StoredAsOne()
        {
            m_repository.SetBool("fs", true);
            Assert.AreEqual(1, m_backend.GetInt("fs", -1));
            Assert.IsTrue(m_repository.GetBool("fs", false));
        }

        [Test]
        public void SetBool_False_StoredAsZero()
        {
            m_repository.SetBool("fs", false);
            Assert.AreEqual(0, m_backend.GetInt("fs", -1));
            Assert.IsFalse(m_repository.GetBool("fs", true));
        }

        [Test]
        public void Save_DelegatesToBackend()
        {
            m_repository.Save();
            m_repository.Save();
            Assert.AreEqual(2, m_backend.SaveCallCount);
        }
    }
}
