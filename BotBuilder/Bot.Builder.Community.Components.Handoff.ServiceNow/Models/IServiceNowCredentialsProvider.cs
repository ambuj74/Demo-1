namespace Bot.Builder.Community.Components.Handoff.ServiceNow.Models
{
    public interface IServiceNowCredentialsProvider
    {
        string ServiceNowTenant { get; }
        string ServiceNowUserName { get; }
        string ServiceNowPassword { get; }
        string MsAppId { get; }
    }
}