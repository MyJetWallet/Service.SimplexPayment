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
    }
}
