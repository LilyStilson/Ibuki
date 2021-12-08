using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using AndroidX.RecyclerView.Widget;
using IbukiBooruLibrary.Booru;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using IbukiMobile.Utils;
using System.Collections.Generic;
using AndroidX.SwipeRefreshLayout.Widget;
using System.Threading;
using Android.Content;
using Android.Widget;
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using AndroidX.CardView.Widget;

namespace IbukiMobile {
    public class RecyclerScrollListener : RecyclerView.OnScrollListener {
        public delegate void LoadMoreEventHandler(int page, int totalItemCount, RecyclerView view);
        public event LoadMoreEventHandler LoadMoreEvent;

        private int visibleThreshold = 10, currentPage = 1, prevTotalItemCount = 0, startingPageIndex = 1;
        private bool isLoading = true;

        private RecyclerView.LayoutManager LayoutManager { get; set; }

        public RecyclerScrollListener(LinearLayoutManager layoutManager) {
            LayoutManager = layoutManager;    
        }

        public RecyclerScrollListener(GridLayoutManager layoutManager) {
            LayoutManager = layoutManager;
            visibleThreshold *= layoutManager.SpanCount;
        }

        public RecyclerScrollListener(StaggeredGridLayoutManager layoutManager) {
            LayoutManager = layoutManager;
            visibleThreshold *= layoutManager.SpanCount;
        }

        public int GetLastVisibleItem(int[] lastVisibleItemPositions) {
            int maxSize = 0;
            for (int i = 0; i < lastVisibleItemPositions.Length; i++) {
                if (i == 0)
                    maxSize = lastVisibleItemPositions[i];
                else if (lastVisibleItemPositions[i] > maxSize)
                    maxSize = lastVisibleItemPositions[i];
            }
            return maxSize;
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy) {
            base.OnScrolled(recyclerView, dx, dy);

            int lastVisibleItemPosition = 0, totalItemCount = LayoutManager.ItemCount;

            if (LayoutManager is StaggeredGridLayoutManager) {
                int[] lastVisibleItemPositions = (LayoutManager as StaggeredGridLayoutManager).FindLastVisibleItemPositions(null);
                lastVisibleItemPosition = GetLastVisibleItem(lastVisibleItemPositions);
            } else if (LayoutManager is GridLayoutManager)
                lastVisibleItemPosition = (LayoutManager as GridLayoutManager).FindLastVisibleItemPosition();
            else if (LayoutManager is LinearLayoutManager)
                lastVisibleItemPosition = (LayoutManager as LinearLayoutManager).FindLastVisibleItemPosition();
            
            if (totalItemCount < prevTotalItemCount) {
                currentPage = startingPageIndex;
                prevTotalItemCount = totalItemCount;
                if (totalItemCount == 0)
                    isLoading = true;
            }

            if (isLoading && (totalItemCount > prevTotalItemCount)) {
                isLoading = false;
                prevTotalItemCount = totalItemCount;
            }

            if (!isLoading && (lastVisibleItemPosition + visibleThreshold) > totalItemCount) {
                LoadMoreEvent(currentPage, totalItemCount, recyclerView);
                currentPage++;
                isLoading = true;
            }

            //var visibleItemCount = recyclerView.ChildCount;
            //var totalItemCount = recyclerView.GetAdapter().ItemCount;
            //int[] pastVisiblesItems = { 0, 0, 0 };
            //LayoutManager.FindFirstVisibleItemPositions(pastVisiblesItems);

            //if ((visibleItemCount + pastVisiblesItems[2]) >= totalItemCount) {
            //    LoadMoreEvent(this, null);
            //}
        }

        public void ResetState() {
            currentPage = startingPageIndex;
            prevTotalItemCount = 0;
            isLoading = true;
            LoadMoreEvent(currentPage, 0, null);
            currentPage++;
        }
    }

    public class RecyclerClickListener : RecyclerView.SimpleOnItemTouchListener {

    }

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity {

        public static BooruData Booru { get; set; }
        //public static int Page { get; set; } = 1;
        public static string Search { get; set; } = "";
        public const int Limit = 20;

        private int PreviousPosition = 0;

        RecyclerView recyclerView;
        SwipeRefreshLayout refreshView;
        StaggeredGridLayoutManager layoutManager;
        RecyclerScrollListener rvScrollListener;
        BooruAdapter adapter;
        List<JObject> Posts = new List<JObject>();

        protected async override void OnCreate(Bundle savedInstanceState) {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Booru = new BooruData { 
                BaseURL = "https://danbooru.donmai.us/",
                post = new Post(),
                postEndpoints = new PostEndpoints()
            };

            //Posts = JsonConvert.DeserializeObject<List<JObject>>(await Networking.GetHTML(new Uri(Booru.BaseURI, BooruEndpoints.ParseEndpointLink(Booru.postEndpoints.GET_Posts, new Dictionary<string, string> {
            //    { "limit", $"{Limit}" },
            //    { "page", $"0" },
            //    { "tags", Search }
            //}))));

            //for (int i = Posts.Count - 1; i >= 0; i--)
            //    if (Posts[i].GetValue(Booru.post.PreviewImageURL) == null)
            //        Posts.RemoveAt(i);

            adapter = new BooruAdapter(Posts);
            adapter.ItemClick += OnItemClick;

            layoutManager = new StaggeredGridLayoutManager(3, StaggeredGridLayoutManager.Vertical);
            rvScrollListener = new RecyclerScrollListener(layoutManager);
            rvScrollListener.LoadMoreEvent += RvScrollListener_LoadMoreEvent;

            recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            recyclerView.SetAdapter(adapter);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddOnScrollListener(rvScrollListener);

            refreshView = FindViewById<SwipeRefreshLayout>(Resource.Id.refreshLayout);
            refreshView.Refresh += RefreshView_Refresh;

            rvScrollListener.ResetState();


            //FindViewById<FloatingActionButton>(Resource.Id.fab).Click += delegate (object sender, EventArgs e) { };
        }

        private void OnItemClick(object sender, int position) {           
            Bitmap bpm = ((sender as ImageView).Drawable as BitmapDrawable).Bitmap;

            MemoryStream baos = new MemoryStream();
            bpm.Compress(Bitmap.CompressFormat.Png, 100, baos);
            byte[] b = baos.ToArray();

            Intent intent = new Intent(this, typeof(BigImageActivity));
            intent.PutExtra("drawable", b);
            intent.PutExtra("post", Posts[position].ToString());            

            ActivityOptions options;
            if (PreviousPosition == position || (sender as ImageView).ContentDescription == "visited") {
                intent.PutExtra("cached", true);
                options = ActivityOptions.MakeSceneTransitionAnimation(this, 
                    Pair.Create(sender as ImageView, "largeImage")
                );   
            } else {
                intent.PutExtra("cached", false);
                options = ActivityOptions.MakeSceneTransitionAnimation(this, 
                    Pair.Create((sender as ImageView), "previewImage")
                );
            }
             
            StartActivity(intent, options.ToBundle());

            PreviousPosition = position;
        }

        private void RefreshView_Refresh(object sender, EventArgs e) {
            Posts.Clear();
            adapter.NotifyDataSetChanged();
            rvScrollListener.ResetState();
        }

        private async void RvScrollListener_LoadMoreEvent(int page, int totalItemCount, RecyclerView view) {
            System.Diagnostics.Debug.WriteLine($"CurrentPage: {page}");
            List<JObject> postsToAdd = JsonConvert.DeserializeObject<List<JObject>>(await Networking.GetHTML(new Uri(Booru.BaseURI, BooruEndpoints.ParseEndpointLink(Booru.postEndpoints.GET_Posts, new Dictionary<string, string> {
                { "limit", $"{Limit}" },
                { "page", $"{page}" },
                { "tags", Search }
            }))));

            for (int i = postsToAdd.Count - 1; i >= 0; i--) /// O(1)
                if (postsToAdd[i].GetValue(Booru.post.PreviewImageURL) == null) /// O(1) + GetValue()
                    postsToAdd.RemoveAt(i); /// O(n)
                                
            /// this takes 3 times longer than for loop
            //postsToAdd.RemoveAll(item => item.GetValue(Booru.post.PreviewImageURL) != null);

            Posts.AddRange(postsToAdd);

            adapter.NotifyDataSetChanged();
            refreshView.Refreshing = false;
        }

        //public override bool OnCreateOptionsMenu(IMenu menu) {
        //    MenuInflater.Inflate(Resource.Menu.menu_main, menu);
        //    return true;
        //}

        //public override bool OnOptionsItemSelected(IMenuItem item) {
        //    int id = item.ItemId;
        //    if (id == Resource.Id.action_settings) {
        //        return true;
        //    }

        //    return base.OnOptionsItemSelected(item);
        //}


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
