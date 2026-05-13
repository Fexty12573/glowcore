namespace GlowCore.UI.Menus
{
    public interface ISaveDataGateway
    {
        bool Exists();
        void Delete();
    }

    public class SaveDataGateway : ISaveDataGateway
    {
        public bool Exists() => SaveData.Exists();
        public void Delete() => SaveData.Delete();
    }
}
