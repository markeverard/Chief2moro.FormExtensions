using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;

namespace Chief2moro.FormExtensions
{
    public static class FormElementHelper
    {
        public const string NOTYETSET = "NOT YET SET";

        public static string GetSiblingElementNames(string elementGuids, ContentArea parentFormElementsArea)
        {
            if (string.IsNullOrEmpty(elementGuids))
                return NOTYETSET;

            var elementNames = new List<string>();
            var elementGuidArray = elementGuids.Split(',');

            if (parentFormElementsArea == null)
                return NOTYETSET;

            foreach (var guid in elementGuidArray)
            {
                var sourceElement = parentFormElementsArea.Items.FirstOrDefault(i => i.ContentGuid.ToString() == guid);
                if (sourceElement == null)
                    continue;

                elementNames.Add(sourceElement.GetContent().Name);
            }

            return string.Join(",", elementNames);
        }

        public static string GetSiblingElementName(string element, ContentArea parentFormElementsArea)
        {
            if (string.IsNullOrEmpty(element))
                return NOTYETSET;

            if (parentFormElementsArea == null)
                return NOTYETSET;

            if (!parentFormElementsArea.Items.Any())
                return NOTYETSET;

            var sourceElement = parentFormElementsArea.Items.FirstOrDefault(i => i.ContentGuid.ToString() == element);
            return sourceElement == null ? NOTYETSET : sourceElement.GetContent().Name;
        }
    }
}
