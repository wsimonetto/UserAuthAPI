namespace UserAuthAPI.Data
{
    public class DataBaseSettings
    {
        public string ConnectionURI { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UserCollectionName { get; set; } = null!;
        public string AppName { get; set; } = "UserAuthAPI";

    }

}
