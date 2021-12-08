using Android.Animation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Transitions;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IbukiMobile.Transitions {
    public class OutlineRadiusTransition : Transition {
        private int startRadius, endRadius;

        //private class Companion {
        //    private static string RADIUS = "OutlineRadiusTransition:radius";
        //    private static string[] PROPERTIES = { "OutlineRadiusTransition:radius" };
        //    private static Companion companion = new Companion();

        //    private 
        //}

        private static string RADIUS = "OutlineRadiusTransition:radius";
        private static string[] PROPERTIES = { "OutlineRadiusTransition:radius" };

        private int outlineRadius;

        //private class Companion : Property {
        //    public Companion(Java.Lang.Class? type, string? name) : base(type, name) { }

        //    public override Java.Lang.Object Get(Java.Lang.Object @object) {
        //        throw new NotImplementedException();
        //    }
        //}


        //private static Companion OutlineRadiusProperty { get; set; } = new Companion(Int32, "outlineRadius");

        public OutlineRadiusTransition() : base() {
        }

        public OutlineRadiusTransition(IntPtr pointer, JniHandleOwnership transfer) : base(pointer, transfer) {
        }

        public OutlineRadiusTransition(View target, int startRadius, int endRadius) : base() {
            AddTarget(target);
            this.startRadius = startRadius;
            this.endRadius = endRadius;
        }

        public OutlineRadiusTransition(Context context, IAttributeSet attrs) : base(context, attrs) {
            TypedArray attributes = context.ObtainStyledAttributes(attrs, Resource.Styleable.OutlineRadiusTransition);

            startRadius = attributes.GetDimensionPixelSize(Resource.Styleable.OutlineRadiusTransition_startRadius, 0);
            endRadius = attributes.GetDimensionPixelSize(Resource.Styleable.OutlineRadiusTransition_endRadius, 0);

            attributes.Recycle();
        }

        public override string[] GetTransitionProperties() {
            return PROPERTIES;
        }

        private bool HasOutlineRadius(View view) {
            Outline outline = new Outline();
            view.OutlineProvider.GetOutline(view, outline);
            return outline.IsEmpty;
        }

        public override void CaptureStartValues(TransitionValues transitionValues) {
            transitionValues.Values[RADIUS] = HasOutlineRadius(transitionValues.View) ? startRadius : endRadius;
        }

        public override void CaptureEndValues(TransitionValues transitionValues) {
            transitionValues.Values[RADIUS] = HasOutlineRadius(transitionValues.View) ? endRadius : startRadius;
        }

        public override Animator CreateAnimator(ViewGroup sceneRoot, TransitionValues startValues, TransitionValues endValues) {
            if (startValues == null || endValues == null) return null;

            View view = endValues.View;
            view.ClipToOutline = true;

            int sR = int.Parse(startValues.Values[RADIUS].ToString());
            int eR = int.Parse(endValues.Values[RADIUS].ToString());

            return ObjectAnimator.OfInt(view, "outlineRadius", sR, eR);
        }


    }
}