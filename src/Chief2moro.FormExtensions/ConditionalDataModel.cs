using System.ComponentModel.DataAnnotations;

namespace Chief2moro.FormExtensions
{
    public class ConditionalDataModel
    {
        [Display(Name = "Source element value")]
        public string SourceElementValue { get; set; }
        [Display(Name = "Resulting value")]
        public string Value { get; set; }
    }
}