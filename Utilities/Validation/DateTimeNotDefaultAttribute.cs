using System;
using System.ComponentModel.DataAnnotations;

public class DateTimeNotDefaultAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        if (value is DateTime dateTime)
        {
            return dateTime != default;
        }
        return false;
    }
}
