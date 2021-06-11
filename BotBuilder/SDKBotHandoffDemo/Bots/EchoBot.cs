// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.12.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SDKBotHandoffDemo.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text != null)
            {
                if (turnContext.Activity.Text.Contains("handoff", System.StringComparison.OrdinalIgnoreCase))
                {
                    var context = JObject.Parse("{\"timeZone\":\"America/New_York\",\"emailId\":\"abraham.lincoln@example.com\",\"userId\":\"abraham.lincoln\"}");
                    var handoffEvent = EventFactory.CreateHandoffInitiation(turnContext, context, null);
                    await turnContext.SendActivityAsync(handoffEvent, cancellationToken);
                }
                else
                {
                    var replyText = $"Echo: {turnContext.Activity.Text}";
                    await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
                }
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
