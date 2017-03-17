using EPiServer.Core;
using EPiServer.Framework.Serialization;
using EPiServer.Framework.Serialization.Internal;
using EPiServer.PlugIn;
using EPiServer.ServiceLocation;

namespace Chief2moro.FormExtensions
{
    [PropertyDefinitionTypePlugIn]
    public class PropertyConditionalDataModelList : PropertyList<ConditionalDataModel>
    {
        public PropertyConditionalDataModelList()
        {
            _objectSerializer = this._objectSerializerFactory.Service.GetSerializer("application/json");
        }

        private Injected<ObjectSerializerFactory> _objectSerializerFactory;

        public IObjectSerializer _objectSerializer;
        protected override ConditionalDataModel ParseItem(string value)
        {
            return _objectSerializer.Deserialize<ConditionalDataModel>(value);
        }

        public override PropertyData ParseToObject(string value)
        {
            ParseToSelf(value);
            return this;
        }
    }
}
