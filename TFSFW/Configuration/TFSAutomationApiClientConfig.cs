namespace Demo.TFSAutomationFramework.Configuration
{
    public static class TFSAutomationApiClientConfig
    {
        /// <summary>
        /// Personal access token <see cref="https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page"/>
        /// </summary>
        public static string Pat { get; set; } = "n2h7dadn2yxzrpfr5cclvxb7g2u7stwl6a4ougimrkysd6p72toa";

        /// <summary>
        /// Host including protocol e.g "http://tfslife:8080"
        /// </summary>
        public static string Host { get; set; } = "http://tfslife:8080";
        public static string ProjectId { get; set; }
        public static string ApiVersion { get; set; }
        public static string Collection { get; set; } = "MIG01";
        public static string BaseTFSUrl => Host + "/tfs/" + Collection + "/";
    }
}
