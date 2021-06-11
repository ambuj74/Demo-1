using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Bot.Builder.Community.Components.Handoff.ServiceNow;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Bot.Builder.Community.Components.Handoff.Shared;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var testActivity = new Microsoft.Bot.Schema.Activity() { ChannelId = Channels.Directline, Locale = "en-us", Conversation = new Microsoft.Bot.Schema.ConversationAccount(id: System.Guid.NewGuid().ToString())};
            testActivity.Type = ActivityTypes.Event;
            testActivity.Value = JObject.Parse("{\"timeZone\":\"America/New_York\"}");
                
            var testContext = new TurnContext(new BotFrameworkAdapter(new SimpleCredentialProvider()), testActivity);
            var conversationHandoffRecordMap = new ConversationHandoffRecordMap();

            string serviceNowTenant = "dev103771.service-now.com";
            string userName = "admin";
            string password = "Han6kv2EGKNq";

            //string serviceNowTenant = "unabotpov.service-now.com";
            //string userName = "darren.jefford";
            //string password = "Corp123!";

            ServiceNowConnector.EscalateToAgentAsync(testContext, testActivity, serviceNowTenant, userName, password, conversationHandoffRecordMap).Wait();

            var message = ServiceNowConnector.MakeServiceNowMessage(
                1,
                System.Guid.NewGuid().ToString(),
                "check case status",
                "America/New_York",
                testActivity.Locale
                );

            ServiceNowConnector.SendMessageToConversationAsync(serviceNowTenant,userName, password, message).Wait();
        }
    }
}
