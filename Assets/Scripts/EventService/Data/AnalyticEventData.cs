namespace EventService.Data
{
    public class AnalyticEventData
    {
        public string Type;
        public string Data;

        public AnalyticEventData(string type, string data)
        {
            Type = type;
            Data = data;
        }
    }
}