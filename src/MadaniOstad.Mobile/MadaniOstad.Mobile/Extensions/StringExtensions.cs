using System.Reflection;
using Xamarin.Forms;

namespace MadaniOstad.Mobile.Extensions
{
    public static class StringExtensions
    {
        public static ImageSource ToImageSource(this string resouceId) => 
            ImageSource.FromResource(resouceId, typeof(StringExtensions).GetTypeInfo().Assembly);
    }
}
