using GlowCore.UI.Menus;

public class FakeSaveDataGateway : ISaveDataGateway
{
    public bool ExistsResult { get; set; }

    public int DeleteCallCount { get; private set; }

    public bool Exists() => ExistsResult;

    public void Delete()
    {
        DeleteCallCount++;
        ExistsResult = false;
    }
}
