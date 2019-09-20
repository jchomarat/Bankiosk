namespace BankioskAPI.Models
{
    public class AppSettings
    {
        public string StorageConnectionString { get; set; }
        public string AzureStorageAccountContainer { get; set; }
        public string FaceKey { get; set; }
        public string FaceEndPoint { get; set; }
        public string FaceGroupID { get; set; }
    }
}
