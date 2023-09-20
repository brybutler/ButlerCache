namespace BryanButler.Cache.Models;
public class ItemRemoval : EventArgs {

    public string Key { get; }
    public string Type { get; }
    public ItemRemoval(string key, string type)
    {
        Key = key;
        Type = type;
    }

}

