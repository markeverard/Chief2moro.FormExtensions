using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EPiServer.Cms.Shell.UI.ObjectEditing.EditorDescriptors;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Shell.ObjectEditing;

namespace Chief2moro.FormExtensions
{
    [ContentType(GUID = "05A98808-4BFC-43FE-B723-4FD2F7E0FF1B", GroupName = "Conditional Elements", DisplayName = "Conditional Value", Order = 5000)]
    public class ConditionalOveriddenValueElementBlock : PredefinedHiddenElementBlock
    {
        public const string NOTYETSET = "NOT YET SET";

        [SelectOne(SelectionFactoryType = typeof(SiblingFormElementSelectionFactory<SelectionElementBlock>))]
        [Display(Name = "Source form element", Order = 100, GroupName = SystemTabNames.Content)]
        public virtual string SourceElement { get; set; }

        [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<ConditionalDataModel>))]
        [Display(Name = "Source element value mapping", Order = 110, GroupName = SystemTabNames.Content)]
        public virtual IList<ConditionalDataModel> SourceElementValueMapping { get; set; }

        public virtual string GetSourceValues()
        {
            if (SourceElementValueMapping == null || !SourceElementValueMapping.Any())
                return NOTYETSET;
            
            return string.Join(",", SourceElementValueMapping.Select(s => s.SourceElementValue));
        }

        public virtual string GetSourceElementName()
        {
            var parentFormElementsArea = this.FindOwnerForm().ElementsArea;
            if (parentFormElementsArea == null)
                return NOTYETSET;
            
            if (!parentFormElementsArea.Items.Any())
                return NOTYETSET;

            if (string.IsNullOrEmpty(SourceElement))
                return NOTYETSET;

            var sourceElement =  parentFormElementsArea.Items.FirstOrDefault(i => i.ContentGuid.ToString() == SourceElement);
            return sourceElement == null ? NOTYETSET : sourceElement.GetContent().Name;
        }
    }
}