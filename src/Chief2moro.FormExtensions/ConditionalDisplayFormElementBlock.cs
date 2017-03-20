using EPiServer.DataAnnotations;
using EPiServer.Forms.Core;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Forms.Implementation.Elements.BaseClasses;
using EPiServer.Shell.ObjectEditing;
using Newtonsoft.Json;

namespace Cosmos.Web.Ui.Business.Forms
{
    [ContentType(GUID = "95A98808-4BFC-43FE-B723-4FD2F7E0FF1B", GroupName = "Conditional Elements", DisplayName = "Conditional field display", Order = 5000)]
    public class ConditionalDisplayFormElementBlock : HiddenElementBlockBase
    {
     
        [SelectOne(SelectionFactoryType = typeof(SiblingFormElementSelectionFactory<SelectionElementBlock>))]
        public virtual string SourceElement { get; set; }

        [SelectMany(SelectionFactoryType = typeof(SiblingFormElementSelectionFactory<ElementBlockBase>))]
        public virtual string TargetElements { get; set; }
        
        public string ToJson()
        {
            var conditionalData = new ConditionalDisplayInstructionModel
            {
                SourceElement = SourceElement,
                TargetElements = TargetElements?.Split(','),
                Value = PredefinedValue,
            };

            return JsonConvert.SerializeObject(conditionalData);
        }

        public virtual string GetTargetElementNames()
        {
            return FormElementHelper.GetSiblingElementNames(TargetElements, FindOwnerForm().ElementsArea);
        }

        public virtual string GetSourceElementName()
        {
            return FormElementHelper.GetSiblingElementName(SourceElement, FindOwnerForm().ElementsArea);
        }
    }
}