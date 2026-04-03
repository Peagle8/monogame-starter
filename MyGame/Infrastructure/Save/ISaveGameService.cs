namespace MyGame.Infrastructure.Save;

public interface ISaveGameService
{
    bool SaveExists();

    SaveGameData? Load();

    void Save(SaveGameData data);
}
