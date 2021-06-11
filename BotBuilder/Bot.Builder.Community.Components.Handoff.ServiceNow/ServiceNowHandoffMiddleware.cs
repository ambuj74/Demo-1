using System;
using System.Globalization;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Bot.Builder.Community.Components.Handoff.ServiceNow.Models;
using Bot.Builder.Community.Components.Handoff.Shared;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace Bot.Builder.Community.Components.Handoff.ServiceNow
{
    public class ServiceNowHandoffMiddleware : HandoffMiddleware
    {
        private readonly ConversationHandoffRecordMap _conversationHandoffRecordMap;
        private readonly IServiceNowCredentialsProvider _creds;

        public ServiceNowHandoffMiddleware(ConversationHandoffRecordMap conversationHandoffRecordMap, IServiceNowCredentialsProvider creds) : base(conversationHandoffRecordMap)
        {
            _conversationHandoffRecordMap = conversationHandoffRecordMap;
            _creds = creds;
        }

        public override async Task RouteActivityToExistingHandoff(ITurnContext turnContext, HandoffRecord handoffRecord)
        {
            var serviceNowHandoffRecord = handoffRecord as ServiceNowHandoffRecord;

            // Retrieve an oAuth token for ServiceNow which we'll pass on this turn
            if (serviceNowHandoffRecord != null)
            {
                var messageText = turnContext.Activity.Text;



                if (turnContext.Activity.Value != null)
                {
                    var activityValue = JObject.Parse(turnContext.Activity.Value.ToString());
                    if (activityValue.ContainsKey("dateVal") && activityValue.ContainsKey("timeVal"))
                    {
                        var dateTimeStr = $"{activityValue["dateVal"]} {activityValue["timeVal"]}";
                        if (DateTime.TryParse(dateTimeStr, out DateTime dateTime))
                        {
                            var baseDate = new DateTime(1970, 1, 1);
                            var diff = dateTime - baseDate;
                            messageText = diff.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);
                        }
                    }
                }

                var message = ServiceNowConnector.MakeServiceNowMessage(0,
                serviceNowHandoffRecord.RemoteConversationId,
                messageText,
                serviceNowHandoffRecord.ConversationRecord.Timezone,
                serviceNowHandoffRecord.ConversationRecord.UserId,
                serviceNowHandoffRecord.ConversationRecord.EmailId);

                await ServiceNowConnector.SendMessageToConversationAsync(
                    serviceNowHandoffRecord.ConversationRecord.ServiceNowTenant,
                    serviceNowHandoffRecord.ConversationRecord.ServiceNowUsername,
                    serviceNowHandoffRecord.ConversationRecord.ServiceNowPassword,
                    message).ConfigureAwait(false);

                var traceActivity = Activity.CreateTraceActivity("ServiceNowVirtualAgent", label: "ServiceNowHandoff->Activity forwarded to ServiceNow");
                await turnContext.SendActivityAsync(traceActivity);

            }
        }
        public override async Task<HandoffRecord> Escalate(ITurnContext turnContext, IEventActivity handoffEvent)
        {
            var serviceNowTenant = _creds.ServiceNowTenant;
            var userName = _creds.ServiceNowUserName;
            var password = _creds.ServiceNowPassword;

            var conversationRecord = await ServiceNowConnector.EscalateToAgentAsync(turnContext, handoffEvent, serviceNowTenant, userName, password, _conversationHandoffRecordMap);

            // Forward the activating activity onto ServiceNow
            var handoffRecord = new ServiceNowHandoffRecord(turnContext.Activity.GetConversationReference(), conversationRecord);
            await RouteActivityToExistingHandoff(turnContext, handoffRecord);

            var traceActivity = Activity.CreateTraceActivity("ServiceNowVirtualAgent", label: "ServiceNowHandoff->Handoff initiated");
            await turnContext.SendActivityAsync(traceActivity);

            return handoffRecord;
        }

    }
}
