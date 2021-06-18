using Bot.Builder.Community.Components.Handoff.ServiceNow.Models;
using Bot.Builder.Community.Components.Handoff.Shared;
using Microsoft.Bot.Schema;

namespace Bot.Builder.Community.Components.Handoff.ServiceNow
{
    public class ServiceNowHandoffRecord : HandoffRecord
    {
        public ServiceNowHandoffRecord(ConversationReference conversationReference, ServiceNowConversationRecord conversationRecord) 
            : base(conversationReference, conversationRecord.ConversationId)
        {
            ConversationRecord = conversationRecord +"adding something";
        }

        public ServiceNowConversationRecord ConversationRecord { get; set; }
    }
}
