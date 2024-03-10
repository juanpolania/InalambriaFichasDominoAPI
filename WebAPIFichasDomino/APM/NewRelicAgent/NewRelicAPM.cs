using NewRelic.Api.Agent;

namespace WebAPIFichasDomino.APM.NewRelicAgent
{
    public class NewRelicAPM : IApmHandler
    {
        ITransaction transaction;

        public NewRelicAPM()
        {
            IAgent agent = NewRelic.Api.Agent.NewRelic.GetAgent();

            if (agent != null)
            {
                transaction = agent.CurrentTransaction;
            }
        }

        public void AddCustomAttribute(string key, string value)
        {
            if (transaction != null)
                transaction.AddCustomAttribute(key, value);
        }

        public void NoticeError(string message)
        {
            IDictionary<String, String> data = new Dictionary<string, string>();
            NewRelic.Api.Agent.NewRelic.NoticeError(message, data);
        }

        public void NoticeError(Exception ex)
        {
            NewRelic.Api.Agent.NewRelic.NoticeError(ex);
        }
    }
}
