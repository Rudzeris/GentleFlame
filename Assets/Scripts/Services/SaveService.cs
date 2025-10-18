namespace Assets.Scripts.Services
{
    public interface ISaveService
    {
        void SaveGame();
        void LoadGame();
        void DeleteSave();
    }
    public class SaveService : ISaveService
    {
        public void DeleteSave()
        {
            throw new System.NotImplementedException();
        }

        public void LoadGame()
        {
            throw new System.NotImplementedException();
        }

        public void SaveGame()
        {
            throw new System.NotImplementedException();
        }
    }
}
