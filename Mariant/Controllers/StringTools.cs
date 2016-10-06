using System.ComponentModel;

namespace WebAsistida.lib
{
    public class StringTools
    {
        static public string Format(string input, object p)
        {
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(p))
                input = input.Replace("{" + prop.Name + "}", (prop.GetValue(p) ?? "(null)").ToString());

            return input;
        }
    }
}