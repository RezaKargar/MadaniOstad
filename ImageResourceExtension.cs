using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace MadaniOstad.Mobile.MarkupExtensions
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
            if(Source == null) return null;
            Source = ImageSource.FromResource(imageSource, this.GetType().GetTypeInfo().Assembly);

            return Source;
        }
    }
}
