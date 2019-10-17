using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Com.Akaita.Android.Circularseekbar;
using static Com.Akaita.Android.Circularseekbar.CircularSeekBar;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{
    public class SeekbarPower : Activity, IOnCenterClickedListener, IOnCircularSeekBarChangeListener
    {

        public SeekbarPower()
        {

 
        }

        public void OnCenterClicked(CircularSeekBar view, float progress)
        {
            Snackbar.Make(view, "Reset", Snackbar.LengthShort).Show();
            view.Progress = 0;
        }

        public void OnProgressChanged(CircularSeekBar view, float progress, bool fromUser)
        {
            if (progress < 33)
                view.RingColor = Color.Red;
            else if (progress < 66)
                view.RingColor = Color.Yellow;
            else 
                view.RingColor = Color.Green;
        }

        public void OnStartTrackingTouch(CircularSeekBar p0)
        {
            //throw new NotImplementedException();
        }

        public void OnStopTrackingTouch(CircularSeekBar p0)
        {
            throw new NotImplementedException();
        }
    }
}