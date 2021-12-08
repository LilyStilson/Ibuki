using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using FFImageLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace IbukiMobile {
    class BooruAdapter : RecyclerView.Adapter {
        public event EventHandler<int> ItemClick;
        public List<JObject> album;

        public BooruAdapter(List<JObject> album) {
            this.album = album;
        }

        public override int ItemCount => album.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            BooruViewHolder viewHolder = holder as BooruViewHolder;

            ImageService.Instance.LoadUrl(album[position].GetValue($"{MainActivity.Booru.post.PreviewImageURL}").Value<string>()).Into(viewHolder.Image);
            viewHolder.ID = album[position].GetValue($"{MainActivity.Booru.post.ID}").Value<string>();
            //viewHolder.Image.Click += OnClick(position);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType) {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.booru_view_holder, parent, false);

            return new BooruViewHolder(itemView, OnClick);
        }

        private void OnClick(object sender, int position) {
            if (ItemClick != null) {
                ItemClick(sender, position);
                (sender as ImageView).ContentDescription = "visited";
            }
        }
    }

    class BooruViewHolder : RecyclerView.ViewHolder {
        public ImageView Image { get; private set; }
        public string ID { get; set; }
        public string Tags { get; set; }

        public BooruViewHolder(View itemView, Action<object, int> listener) : base (itemView) {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            ID = Image.Tag.ToString();
            Tags = Image.ContentDescription;

            itemView.Click += (sender, e) => listener(Image, base.LayoutPosition);
        }
    }
}