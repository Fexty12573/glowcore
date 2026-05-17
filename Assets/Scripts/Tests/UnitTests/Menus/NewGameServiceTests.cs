using GlowCore.UI.Menus;
using NUnit.Framework;

public class NewGameServiceTests
{
    private FakeSaveDataGateway m_gateway;
    private FakeGameLaunchContext m_launchContext;
    private FakeSceneTransition m_sceneTransition;
    private NewGameService m_service;

    [SetUp]
    public void SetUp()
    {
        m_gateway = new FakeSaveDataGateway();
        m_launchContext = new FakeGameLaunchContext();
        m_sceneTransition = new FakeSceneTransition();
        m_service = new NewGameService(m_gateway, m_launchContext, m_sceneTransition);
    }

    [Test]
    public void SaveExists_DelegatesToGateway()
    {
        m_gateway.ExistsResult = true;
        Assert.IsTrue(m_service.SaveExists);

        m_gateway.ExistsResult = false;
        Assert.IsFalse(m_service.SaveExists);
    }

    [Test]
    public void StartNewGame_DeletesExistingSave()
    {
        m_gateway.ExistsResult = true;
        m_service.StartNewGame("Aurora");
        Assert.AreEqual(1, m_gateway.DeleteCallCount);
    }

    [Test]
    public void StartNewGame_SetsLaunchContextToNewGameWithName()
    {
        m_service.StartNewGame("Aurora");
        Assert.AreEqual(GameLaunchMode.NewGame, m_launchContext.Mode);
        Assert.AreEqual("Aurora", m_launchContext.WorldName);
        Assert.AreEqual(1, m_launchContext.SetNewGameCallCount);
    }

    [Test]
    public void StartNewGame_LoadsMainWorldScene()
    {
        m_service.StartNewGame("Aurora");
        Assert.AreEqual(1, m_sceneTransition.LoadMainWorldCount);
        Assert.AreEqual(0, m_sceneTransition.LoadTitleCount);
    }

    [Test]
    public void StartNewGame_OrderIsDeleteThenContextThenScene()
    {
        m_gateway.ExistsResult = true;
        m_service.StartNewGame("Aurora");
        Assert.AreEqual(1, m_gateway.DeleteCallCount);
        Assert.AreEqual(GameLaunchMode.NewGame, m_launchContext.Mode);
        Assert.AreEqual(1, m_sceneTransition.LoadMainWorldCount);
    }
}
