using System.Text;

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
        public static ICollection<string> ConvertToList(string input)
        {
            
            List<string> resultList = new List<string>();

            // Check if the input starts with "[" and ends with "]"
            if (input.StartsWith("[") && input.EndsWith("]"))
            {
                // Remove "[" and "]" from the input
                input = input.Substring(1, input.Length - 2);

                // Split the input string by ","
                string[] items = input.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                // Trim each item and add it to the list
                foreach (var item in items)
                {
                    resultList.Add(item.Trim());
                }
            }
            else
            {
                // If it's not a stringified list, add the input string to the list
                resultList.Add(input.Trim());
            }

            return resultList;
        }

        public static string ConvertToString(ICollection<string> inputList)
        {
            if (inputList.Count == 0)
            {
                throw new Exception("Emails must be specified");
            }
            // Initialize a StringBuilder to construct the string
            StringBuilder sb = new();

            // Append "[" to the string
            sb.Append("[");

            // Append each element of the list to the string
            foreach (var item in inputList)
            {
                // Append the item followed by a comma and space
                sb.Append(item);
                sb.Append(", ");
            }

            // Remove the trailing comma and space if the list is not empty
            if (inputList.Count > 0)
            {
                sb.Length -= 2; // Remove the last 2 characters (", ")
            }

            // Append "]" to the string
            sb.Append("]");

            // Convert StringBuilder to string and return
            return sb.ToString();
        }
    }
}
