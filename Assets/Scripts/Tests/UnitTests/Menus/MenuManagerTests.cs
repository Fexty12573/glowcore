using GlowCore.UI.Menus;
using NUnit.Framework;
using Tests.Mocks.Menus;

namespace Tests.UnitTests.Menus
{
    public class MenuManagerTests
    {
        private MenuScreenLocator m_locator;
        private MenuManager m_manager;
        private FakeMenuScreen m_title;
        private FakeMenuScreen m_newGame;
        private FakeMenuScreen m_settings;

        [SetUp]
        public void SetUp()
        {
            m_locator = new MenuScreenLocator();
            m_title = new FakeMenuScreen(MenuScreenId.Title);
            m_newGame = new FakeMenuScreen(MenuScreenId.NewGame);
            m_settings = new FakeMenuScreen(MenuScreenId.Settings);
            m_locator.Register(m_title);
            m_locator.Register(m_newGame);
            m_locator.Register(m_settings);
            m_manager = new MenuManager(m_locator);
        }

        [Test]
        public void Open_FirstScreen_ShowsAndSetsCurrent()
        {
            m_manager.Open(MenuScreenId.Title);
            Assert.AreSame(m_title, m_manager.Current);
            Assert.AreEqual(1, m_title.ShowCallCount);
            Assert.AreEqual(0, m_title.HideCallCount);
        }

        [Test]
        public void Open_UnregisteredScreen_DoesNothing()
        {
            m_manager.Open(MenuScreenId.Pause);
            Assert.IsNull(m_manager.Current);
        }

        [Test]
        public void Open_SecondScreen_HidesPreviousAndShowsNew()
        {
            m_manager.Open(MenuScreenId.Title);
            m_manager.Open(MenuScreenId.NewGame);

            Assert.AreSame(m_newGame, m_manager.Current);
            Assert.AreEqual(1, m_title.HideCallCount);
            Assert.AreEqual(1, m_newGame.ShowCallCount);
        }

        [Test]
        public void Open_FiresScreenChangedEvent()
        {
            IMenuScreen received = null;
            m_manager.OnScreenChanged += s => received = s;

            m_manager.Open(MenuScreenId.Title);

            Assert.AreSame(m_title, received);
        }

        [Test]
        public void Back_AfterTwoOpens_ReturnsToFirst()
        {
            m_manager.Open(MenuScreenId.Title);
            m_manager.Open(MenuScreenId.NewGame);

            m_manager.Back();

            Assert.AreSame(m_title, m_manager.Current);
            Assert.AreEqual(2, m_title.ShowCallCount); // initial open + return
            Assert.AreEqual(1, m_newGame.HideCallCount);
        }

        [Test]
        public void Back_NoHistory_ClosesAll()
        {
            m_manager.Open(MenuScreenId.Title);
            m_manager.Back();
            Assert.IsNull(m_manager.Current);
            Assert.AreEqual(1, m_title.HideCallCount);
        }

        [Test]
        public void Back_ThreeOpensThenTwoBack_ReturnsToFirst()
        {
            m_manager.Open(MenuScreenId.Title);
            m_manager.Open(MenuScreenId.NewGame);
            m_manager.Open(MenuScreenId.Settings);

            m_manager.Back();
            Assert.AreSame(m_newGame, m_manager.Current);

            m_manager.Back();
            Assert.AreSame(m_title, m_manager.Current);
        }

        [Test]
        public void CloseAll_HidesCurrentAndClearsHistory()
        {
            m_manager.Open(MenuScreenId.Title);
            m_manager.Open(MenuScreenId.NewGame);

            m_manager.CloseAll();

            Assert.IsNull(m_manager.Current);
            Assert.AreEqual(1, m_newGame.HideCallCount);

            // After CloseAll, Back should be a no-op (history is empty, nothing to hide).
            m_manager.Back();
            Assert.IsNull(m_manager.Current);
        }

        [Test]
        public void OpenWithArgs_ForwardsArgsToReceiver()
        {
            var dialog = new FakeArgsMenuScreen<string>(MenuScreenId.ConfirmDialog);
            m_locator.Register(dialog);

            m_manager.OpenWithArgs(MenuScreenId.ConfirmDialog, "Are you sure?");

            Assert.AreSame(dialog, m_manager.Current);
            Assert.AreEqual("Are you sure?", dialog.LastArgs);
            Assert.AreEqual(1, dialog.SetArgsCallCount);
            Assert.AreEqual(1, dialog.ShowCallCount);
        }

        [Test]
        public void OpenSameScreenTwice_DoesNotPushHistory()
        {
            m_manager.Open(MenuScreenId.Title);
            m_manager.Open(MenuScreenId.Title);

            // Back should now close (no real history).
            m_manager.Back();
            Assert.IsNull(m_manager.Current);
        }
    }
}
