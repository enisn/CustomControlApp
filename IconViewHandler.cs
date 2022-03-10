using Microsoft.Maui.Handlers;

#if __IOS__ || MACCATALYST
using NativeView = UIKit.UIImageView;

#elif ANDROID
using NativeView = Android.Widget.ImageView;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics;

#elif WINDOWS
using NativeView = Microsoft.UI.Xaml.Controls.Image;

#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID && !WINDOWS)
using NativeView = System.Object;
#endif

namespace CustomControlApp;

public interface IIconViewHandler : IViewHandler
{
    IIconView TypedVirtualView { get; }
    NativeView TypedNativeView { get; }
}

public partial class IconViewHandler : IIconViewHandler
{
    public static IPropertyMapper<IIconView, IIconViewHandler> IconViewMapper => new PropertyMapper<IIconView, IIconViewHandler>()
    {
        [nameof(IIconView.Source)] = MapSource,
        [nameof(IIconView.FillColor)] = MapFillColor,
    };

    public IconViewHandler() : base(IconViewMapper)
    {

    }

    public IconViewHandler(IPropertyMapper? mapper = null) : base(mapper ?? IconViewMapper)
    {
    }

    IIconView IIconViewHandler.TypedVirtualView => VirtualView;

    NativeView IIconViewHandler.TypedNativeView => NativeView;
}

#if ANDROID
public partial class IconViewHandler : ViewHandler<IIconView, ImageView>
{
    protected override ImageView CreateNativeView()
    {
        return new ImageView(Context);
    }

    public static void MapSource(IIconViewHandler handler, IIconView view)
    {
        UpdateBitmap(handler, view);
    }

    public static void MapFillColor(IIconViewHandler handler, IIconView view)
    {
        UpdateBitmap(handler, view);
    }

    public static void UpdateBitmap(IIconViewHandler handler, IIconView view)
    {
        if (view.Source == null)
            return;

        Drawable d = default;

        if (view.Source is StreamImageSource streamImageSource)
        {
            var cTokenSource = new CancellationTokenSource(30000);
            var stream = streamImageSource.Stream(cTokenSource.Token).Result;
            d = Drawable.CreateFromStream(stream, "inputkit_check");
        }
        else if (view.Source is FileImageSource fileImageSource)
        {
            d = handler.MauiContext.Context.GetDrawable(fileImageSource.File);
        }
        else
        {
            d = handler.MauiContext.Context.GetDrawable(view.Source.ToString());
        }

        if (d == null)
            return;

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            d.SetTint(view.FillColor.ToAndroid());
        else
            d.SetColorFilter(new LightingColorFilter(Colors.Black.ToAndroid(), view.FillColor.ToAndroid()));

        d.Alpha = view.FillColor.ToAndroid().A;
        handler.TypedNativeView.SetImageDrawable(d);
    }
}
#endif
