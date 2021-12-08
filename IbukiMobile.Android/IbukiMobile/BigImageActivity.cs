using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content.PM;
using Android.Content.Res;
using FFImageLoading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AndroidX.AppCompat.App;
using Java.Interop;
using Android.Transitions;
using Android.Animation;

namespace IbukiMobile {
    [Activity(ConfigurationChanges = ConfigChanges.Orientation)]
    public class BigImageActivity : AppCompatActivity {
        ImageView previewImageView, largeImageView;
        ProgressBar loadProgressBar;
        JObject Post = null;

        protected override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.big_image_view);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
   
            /// Create your application here
            previewImageView = FindViewById<ImageView>(Resource.Id.previewImageView);
            previewImageView.Visibility = ViewStates.Visible;
            largeImageView = FindViewById<ImageView>(Resource.Id.largeImageView);

            loadProgressBar = FindViewById<ProgressBar>(Resource.Id.loadProgressBar);
            
            Post = JsonConvert.DeserializeObject<JObject>(Intent.Extras.GetString("post"));
         
            SupportActionBar.Title = $"ID: {Post.GetValue($"{MainActivity.Booru.post.ID}").Value<string>()}";

            if (Intent.Extras.GetBoolean("cached")) {
                previewImageView.Visibility = ViewStates.Gone;
                loadProgressBar.Visibility = ViewStates.Gone;
            } else {
                byte[] b = Intent.Extras.GetByteArray("drawable");
                Bitmap bmp = BitmapFactory.DecodeByteArray(b, 0, b.Length);
                previewImageView.SetImageBitmap(bmp);

                largeImageView.Visibility = ViewStates.Invisible;
                previewImageView.Visibility = ViewStates.Visible;
                loadProgressBar.Visibility = ViewStates.Visible;
            }

            ImageService.Instance.LoadUrl(Post.GetValue($"{MainActivity.Booru.post.LargeImageURL}").Value<string>()).Retry(3, 500)
                //.DownloadStarted((info) => { })
                .Success(() => {
                    largeImageView.Visibility = ViewStates.Visible;
                    loadProgressBar.Visibility = ViewStates.Gone;
                })
                .Into(largeImageView);
        }

        public override bool OnOptionsItemSelected(IMenuItem item) {
            if (item.ItemId == Android.Resource.Id.Home) {
                SupportFinishAfterTransition();
            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnConfigurationChanged(Configuration newConfig) {
            base.OnConfigurationChanged(newConfig);

            /// Checks the orientation of the screen
            if (newConfig.Orientation == Android.Content.Res.Orientation.Landscape) {
                previewImageView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
                largeImageView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);
            } else if (newConfig.Orientation == Android.Content.Res.Orientation.Portrait) {
                previewImageView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                largeImageView.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);

            }
        }
    }
}