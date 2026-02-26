using System.ComponentModel;

namespace DeltaFour.Domain.Enum
{
    public static class MessageEnum
    {
        public static string Message(this PunchInResponse val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString())!
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}
