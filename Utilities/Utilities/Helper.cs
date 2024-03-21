namespace Utilities.Utilities
{
    public static class Helper
    {
        public static string GetDaySuffix(DateTime date)
        {
            int day = date.Day;
            switch (day % 10)
            {
                case 1 when day != 11:
                    return "ˢᵗ"; // Small superscript "st"
                case 2 when day != 12:
                    return "ⁿᵈ"; // Small superscript "nd"
                case 3 when day != 13:
                    return "ʳᵈ"; // Small superscript "rd"
                default:
                    return "ᵗʰ"; // Small superscript "th"
            }
        }
    }
}
