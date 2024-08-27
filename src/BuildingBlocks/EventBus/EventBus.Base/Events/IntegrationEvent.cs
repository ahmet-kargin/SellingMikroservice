using Newtonsoft.Json;

namespace EventBus.Base.Events;

public class IntegrationEvent
{
    [JsonProperty]
    public Guid Id { get; private set; }
    [JsonProperty]
    public DateTime CreatedDate { get; private set; }

    [JsonConstructor]
    public IntegrationEvent(Guid id, DateTime createdDate)
    {
        CreatedDate = createdDate;
        Id = id;
    }
     
    public IntegrationEvent()
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }
}
