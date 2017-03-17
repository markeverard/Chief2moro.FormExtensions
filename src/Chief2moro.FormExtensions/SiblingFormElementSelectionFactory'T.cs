using System.Collections.Generic;
using System.Linq;
using EPiServer;
using EPiServer.Core;
using EPiServer.Forms.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace Chief2moro.FormExtensions
{
    public class SiblingFormElementSelectionFactory<T> : ISelectionFactory where T : ElementBlockBase
    {
        public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
        {
            dynamic contentMetadata = metadata;
            var ownerContent = contentMetadata.OwnerContent as IContent;

            //this content must be a form element
            if (!(ownerContent is ElementBlockBase))
                return new List<SelectItem>() {new SelectItem {Text = "Empty", Value = "Empty"}};

            var ownerContentElement = ownerContent as ElementBlockBase;

            //this content must have a parent form
            var ownerForm = ownerContentElement.FindOwnerForm();
            if (ownerForm == null)
                return new List<SelectItem>() { new SelectItem { Text = "Empty", Value = "Empty" } };

            //the parent form must have other form elements
            if (ownerForm.ElementsArea == null)
                return new List<SelectItem>() {new SelectItem {Text = "Empty", Value = "Empty"}};

            //get all sibling form elements of type T
            var contentLoader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var formItems = ownerForm.ElementsArea.Items;
            var selectItemsOfType = formItems.Select(item => contentLoader.Get<IContent>(item.ContentLink)).OfType<T>().Select(content => content as T).ToList();

            var selectItems = selectItemsOfType.Select(i => new SelectItem()
            {
                Value = i.Content.ContentGuid,
                Text = i.Content.Name
            });

            return selectItems;
        }
    }
}