namespace IFoxCommerce.BL.Navireo.Extensions
{
    public static class String
    {
        public static string LimitString(this string str, int number)
        {
            if (str != null)
                return str.Length <= number ? str : str.Substring(0, number);
            return "";
        }
    }
}
