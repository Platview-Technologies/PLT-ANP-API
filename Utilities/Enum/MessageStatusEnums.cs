using System.ComponentModel.DataAnnotations;


namespace Utilities.Enum
{
    public enum MessageStatusEnums
    {
        Pending,
        Sent,
        Failed,
        [Display(Name = "Sent Partially")]
        SentPartially,
    }
}
