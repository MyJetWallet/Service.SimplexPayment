using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.SimplexPayment.Settings
{
    public class SettingsModel
    {
        [YamlProperty("SimplexPayment.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("SimplexPayment.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("SimplexPayment.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("SimplexPayment.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }        
        
        [YamlProperty("SimplexPayment.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
        
        [YamlProperty("SimplexPayment.MyNoSqlReaderHostPort")]
        public string MyNoSqlReaderHostPort { get; set; }
        
        [YamlProperty("SimplexPayment.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("SimplexPayment.SimplexApiKey")]
        public string SimplexApiKey { get; set; }
        
        [YamlProperty("SimplexPayment.SimplexWalletId")]
        public string SimplexWalletId { get; set; }
        
        [YamlProperty("SimplexPayment.PaymentLink")]
        public string PaymentLink { get; set; }
        
        [YamlProperty("SimplexPayment.ProductionMode")]
        public bool ProductionMode { get; set; } 
        // [YamlProperty("SimplexPayment.SimplexBrokerWalletId")]
        // public string SimplexBrokerWalletId { get; set; }
    }
}
