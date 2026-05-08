using GlowCore.UI.Menus;
using NUnit.Framework;

namespace Tests.UnitTests.Menus
{
    public class AudioBridgeTests
    {
        private AudioBridge m_bridge;

        [SetUp]
        public void SetUp()
        {
            m_bridge = new AudioBridge();
        }

        [Test]
        public void RaiseMaster_FiresMasterEventWithValue()
        {
            float? received = null;
            m_bridge.OnMasterVolumeChanged += v => received = v;
            m_bridge.RaiseMaster(0.42f);
            Assert.AreEqual(0.42f, received);
        }

        [Test]
        public void RaiseMusic_FiresMusicEventWithValue()
        {
            float? received = null;
            m_bridge.OnMusicVolumeChanged += v => received = v;
            m_bridge.RaiseMusic(0.5f);
            Assert.AreEqual(0.5f, received);
        }

        [Test]
        public void RaiseSfx_FiresSfxEventWithValue()
        {
            float? received = null;
            m_bridge.OnSfxVolumeChanged += v => received = v;
            m_bridge.RaiseSfx(0.9f);
            Assert.AreEqual(0.9f, received);
        }

        [Test]
        public void RaiseMaster_DoesNotFireMusicOrSfx()
        {
            var musicFired = false;
            var sfxFired = false;
            m_bridge.OnMusicVolumeChanged += _ => musicFired = true;
            m_bridge.OnSfxVolumeChanged += _ => sfxFired = true;

            m_bridge.RaiseMaster(0.3f);

            Assert.IsFalse(musicFired);
            Assert.IsFalse(sfxFired);
        }

        [Test]
        public void RaiseMaster_NoSubscribers_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => m_bridge.RaiseMaster(0.7f));
        }

        [Test]
        public void RaiseMaster_MultipleSubscribers_AllReceiveValue()
        {
            float? a = null;
            float? b = null;
            m_bridge.OnMasterVolumeChanged += v => a = v;
            m_bridge.OnMasterVolumeChanged += v => b = v;

            m_bridge.RaiseMaster(0.6f);

            Assert.AreEqual(0.6f, a);
            Assert.AreEqual(0.6f, b);
        }

        [Test]
        public void RaiseMaster_AfterUnsubscribe_DoesNotInvokeHandler()
        {
            var fireCount = 0;
            void Handler(float _) => fireCount++;

            m_bridge.OnMasterVolumeChanged += Handler;
            m_bridge.RaiseMaster(0.1f);
            m_bridge.OnMasterVolumeChanged -= Handler;
            m_bridge.RaiseMaster(0.2f);

            Assert.AreEqual(1, fireCount);
        }
    }
}
