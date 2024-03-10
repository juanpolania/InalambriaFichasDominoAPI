namespace WebAPIFichasDomino.APM.NewRelicAgent
{
    public interface IApmHandler
    {
        public void AddCustomAttribute(string key, string value);

        public void NoticeError(string message);

        public void NoticeError(Exception ex);
    }
}
