using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MadaniOstad.Mobile.Extensions
{
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public ImageResourceExtension()
        {
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;
            return Source.ToImageSource();
        }
    }
}
