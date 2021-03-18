using Serilog.Formatting.Elasticsearch;

namespace OwnID.Server.IAS
{
    public class OwnIdFormatter : ElasticsearchJsonFormatter
    {
        // TODO: scope decoupling
        public OwnIdFormatter() : base(renderMessageTemplate: false, inlineFields: true)
        {
        }
    }
}