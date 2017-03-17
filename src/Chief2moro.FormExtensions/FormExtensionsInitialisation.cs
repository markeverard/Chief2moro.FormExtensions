using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.Forms.Core.Events;
using EPiServer.Forms.Implementation.Elements;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Chief2moro.FormExtensions
{
    /// <summary>
    /// Set up a FormSubmission finalised event that replace a 
    /// </summary>
    /// <seealso cref="EPiServer.Framework.IInitializableModule" />
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class FormExtensionsInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var formsEvents = ServiceLocator.Current.GetInstance<FormsEvents>();
            formsEvents.FormsSubmitting += FormsEvents_FormsSubmissionFinalized;
        }

        private void FormsEvents_FormsSubmissionFinalized(object sender, FormsEventArgs e)
        {
            var submitEventArgs = e as FormsSubmittingEventArgs;
            if (submitEventArgs == null)
                return;

            //get all form elements in parent form
            var formElements = e.FormsContent.Property["ElementsArea"].Value as ContentArea;
            if (formElements == null)
                return;
            
            //if form contains a conditional form value block - pick the first
            var conditionalValuesField = formElements.Items.Select(s => s.GetContent())
                                                    .FirstOrDefault(s => s is ConditionalOveriddenValueElementBlock);

            if (conditionalValuesField == null)
                return;
            
            var conditionalValuesBlock  = conditionalValuesField as ConditionalOveriddenValueElementBlock;

            //get source element set by editor
            var sourceElementGuid = conditionalValuesBlock.SourceElement;
            if (sourceElementGuid == null)
                return;

            //find sourceElement from form
            var sourceElement = formElements.Items.FirstOrDefault(s => s.ContentGuid.ToString() == sourceElementGuid);
            if (sourceElement == null)
                return;

            //get value from submitted form with soureElement
            var sourceElementData = submitEventArgs.SubmissionData.Data.FirstOrDefault(i => i.Key.EndsWith("_" + sourceElement.ContentLink.ID.ToString()));
            var elementValue = conditionalValuesBlock.SourceElementValueMapping.FirstOrDefault(c => c.SourceElementValue == sourceElementData.Value.ToString());

            //get this fields submitted data
            var conditionalValuesDataField = submitEventArgs.SubmissionData.Data.FirstOrDefault(
                        i => i.Key.EndsWith("_" + conditionalValuesField.ContentLink.ID.ToString()));

            //if there is a match from the source element, then set this fields value from the matching SourceElementValueMapping
            if (elementValue != null)
                submitEventArgs.SubmissionData.Data[conditionalValuesDataField.Key] = elementValue.Value;
        }

        public void Uninitialize(InitializationEngine context)
        {
          
        }

        public void Preload(string[] parameters)
        {
        }
    
    }
}
