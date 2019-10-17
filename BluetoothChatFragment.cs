
using System;
using System.Text;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Com.Akaita.Android.Circularseekbar;
using static Com.Akaita.Android.Circularseekbar.CircularSeekBar;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{
    public partial class BluetoothChatFragment : Fragment, IOnCenterClickedListener, IOnCircularSeekBarChangeListener
    {
        const string TAG = "BluetoothChatFragment";

        const int REQUEST_CONNECT_DEVICE_SECURE = 1;
        const int REQUEST_CONNECT_DEVICE_INSECURE = 2;
        const int REQUEST_ENABLE_BT = 3;

        ListView conversationView;
        EditText outEditText;
        Button   sendButton;
        Button fake1, fake2, fake3, fake4, fake5, fake6;

        TextView outDeviceName;
        TextView schudle1, schudle2, schudle3;
        TextView power1, power2, power3;
        TextView Txtprogress1, Txtprogress2, Txtprogress3, Txtprogress4, Txtprogress5, Txtprogress6;
        CircularSeekBar seekbarPower, seekbarPower2, seekbarPower3;
        LinearLayout linlyt1, linlyt2, linlyt3, linlyt4, linlyt5, linlyt6;
        ProgressBar progress1, progress2, progress3, progress4, progress5, progress6;

        Android.Widget.Toolbar toolbarBottom;

        String connectedDeviceName = "";
        ArrayAdapter<String> conversationArrayAdapter;
        StringBuilder outStringBuffer;
        BluetoothAdapter bluetoothAdapter = null;
        BluetoothChatService chatService = null;

        bool requestingPermissionsSecure, requestingPermissionsInsecure;

        DiscoverableModeReceiver receiver;
        ChatHandler handler;
        WriteListener writeListener;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetHasOptionsMenu(true);

            bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            receiver = new DiscoverableModeReceiver();
            receiver.BluetoothDiscoveryModeChanged += (sender, e) =>
            {
                Activity.InvalidateOptionsMenu();
            };

            if (bluetoothAdapter == null)
            {
                Toast.MakeText(Activity, "Bluetooth is not available.", ToastLength.Long).Show();
                Activity.FinishAndRemoveTask();
            }

            writeListener = new WriteListener(this);
            handler = new ChatHandler(this);
        }

        public override void OnStart()
        {
            base.OnStart();
            if (!bluetoothAdapter.IsEnabled)
            {
                var enableIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                StartActivityForResult(enableIntent, REQUEST_ENABLE_BT);
            }
            else if (chatService == null)
            {
                SetupChat();
            }

            // Register for when the scan mode changes
            var filter = new IntentFilter(BluetoothAdapter.ActionScanModeChanged);
            Activity.RegisterReceiver(receiver, filter);
        }

        public override void OnResume()
        {
            base.OnResume();
            if (chatService != null)
            {
                if (chatService.GetState() == BluetoothChatService.STATE_NONE)
                {
                    chatService.Start();
                }
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) 
        {

            return inflater.Inflate(Resource.Layout.fragment_bluetooth_chat, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState) 
        {
            conversationView = view.FindViewById<ListView>(Resource.Id.@in);
            outDeviceName    = view.FindViewById<TextView>(Resource.Id.deviceselected);
            outEditText      = view.FindViewById<EditText>(Resource.Id.edit_text_out);
            sendButton       = view.FindViewById<Button>(Resource.Id.button_send);
            toolbarBottom    = view.FindViewById<Android.Widget.Toolbar>(Resource.Id.toolbar_bottom);

            toolbarBottom.InflateMenu(Resource.Menu.bluetooth_chat);

            OnCreatdLinerLayout(view);
            OnCreatedSchedule(view);
            OnCreatePower(view);
            OnCreateProgressbar(view);
            OnCreateTextProgress(view);

            outEditText.Visibility      = ViewStates.Invisible;
            sendButton.Visibility       = ViewStates.Invisible;
            conversationView.Visibility = ViewStates.Invisible;

            PairWithBlueToothDevice(false);
        }

        private void OnCreateTextProgress(View view)
        {
            Txtprogress1 = view.FindViewById<TextView>(Resource.Id.txtBattery_Status);
            Txtprogress2 = view.FindViewById<TextView>(Resource.Id.txtBattery_volt);
            Txtprogress3 = view.FindViewById<TextView>(Resource.Id.txtBattery_intensity);
            Txtprogress4 = view.FindViewById<TextView>(Resource.Id.txtLed_power);
            Txtprogress5 = view.FindViewById<TextView>(Resource.Id.txthumidity);
            Txtprogress6 = view.FindViewById<TextView>(Resource.Id.txtTemperature);

        }

        private void OnCreateProgressbar(View view)
        {
            progress1 = view.FindViewById<ProgressBar>(Resource.Id.progress1);
            progress2 = view.FindViewById<ProgressBar>(Resource.Id.progress2);
            progress3 = view.FindViewById<ProgressBar>(Resource.Id.progress3);
            progress4 = view.FindViewById<ProgressBar>(Resource.Id.progress4);
            progress5 = view.FindViewById<ProgressBar>(Resource.Id.progress5);
            progress6 = view.FindViewById<ProgressBar>(Resource.Id.progress6);
        }

        private void OnCreatdLinerLayout(View view)
        {
            linlyt1 = view.FindViewById<LinearLayout>(Resource.Id.LinLay1);
            linlyt2 = view.FindViewById<LinearLayout>(Resource.Id.LinLay2);
            linlyt3 = view.FindViewById<LinearLayout>(Resource.Id.LinLay3);
            linlyt4 = view.FindViewById<LinearLayout>(Resource.Id.LinLay4);
            linlyt5 = view.FindViewById<LinearLayout>(Resource.Id.LinLay5);
            linlyt6 = view.FindViewById<LinearLayout>(Resource.Id.LinLay6);

        }

        private void LinerLayoutVisibility(bool visibility)
        {
            if (visibility)
            {
                linlyt1.Visibility = ViewStates.Visible;
                linlyt2.Visibility = ViewStates.Visible;
                linlyt3.Visibility = ViewStates.Visible;
                linlyt4.Visibility = ViewStates.Visible;
                linlyt5.Visibility = ViewStates.Visible;
                linlyt6.Visibility = ViewStates.Visible;
            }
            else
            {
                linlyt1.Visibility = ViewStates.Invisible;
                linlyt2.Visibility = ViewStates.Invisible;
                linlyt3.Visibility = ViewStates.Invisible;
                linlyt4.Visibility = ViewStates.Invisible;
                linlyt5.Visibility = ViewStates.Invisible;
                linlyt6.Visibility = ViewStates.Invisible;
            }
        }

        private void OnCreatePower(View view)
        {
            seekbarPower  = view.FindViewById<CircularSeekBar>(Resource.Id.seekbar); 
            seekbarPower2 = view.FindViewById<CircularSeekBar>(Resource.Id.seekbar2); 
            seekbarPower3 = view.FindViewById<CircularSeekBar>(Resource.Id.seekbar3); 

            seekbarPower.SetOnCenterClickedListener(this);
            seekbarPower2.SetOnCenterClickedListener(this);
            seekbarPower3.SetOnCenterClickedListener(this);

            seekbarPower.SetOnCircularSeekBarChangeListener(this);
            seekbarPower2.SetOnCircularSeekBarChangeListener(this);
            seekbarPower3.SetOnCircularSeekBarChangeListener(this);

            seekbarPower.ProgressTextSize  = 30;
            seekbarPower2.ProgressTextSize = 30;
            seekbarPower3.ProgressTextSize = 30;
        }

        private void OnCreatedSchedule(View view)
        {
            //prueba ast poner un bucle for aqui porque se repite mucho 
            schudle1 = view.FindViewById<TextView>(Resource.Id.Schudle1);
            schudle2 = view.FindViewById<TextView>(Resource.Id.Schudle2);
            schudle3 = view.FindViewById<TextView>(Resource.Id.Schudle3);

            power1 = view.FindViewById<TextView>(Resource.Id.Power1);
            power2 = view.FindViewById<TextView>(Resource.Id.Power2);
            power3 = view.FindViewById<TextView>(Resource.Id.Power3);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults) 
        {
            var allGranted = grantResults.AllPermissionsGranted();
            if (requestCode == PermissionUtils.RC_LOCATION_PERMISSIONS)
            {
                if (requestingPermissionsSecure)
                {
                    PairWithBlueToothDevice(true);
                }
                if (requestingPermissionsInsecure)
                {
                    PairWithBlueToothDevice(false);
                }

                requestingPermissionsSecure = false;
                requestingPermissionsInsecure = false;
            }
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            switch (requestCode)
            {
                case REQUEST_CONNECT_DEVICE_SECURE:
                    if (Result.Ok == resultCode)
                    {
                        ConnectDevice(data, true);
                    }
                    break;
                case REQUEST_CONNECT_DEVICE_INSECURE:
                    if (Result.Ok == resultCode)
                    {
                        ConnectDevice(data, true);
                    }
                    break;
                case REQUEST_ENABLE_BT:
                    if (Result.Ok == resultCode)
                    {
                        Toast.MakeText(Activity, Resource.String.bt_not_enabled_leaving, ToastLength.Short).Show();
                        Activity.FinishAndRemoveTask();
                    }
                    break;
            }
        }

        public bool ItemSelected(IMenuItem item) 
        {
            switch (item.ItemId)
            {

                case Resource.Id.cleanup:
                    {
                        conversationArrayAdapter.Clear();
                        return true;
                    }

                case Resource.Id.refresh:
                    sendRefresh();
                    return true;
                case Resource.Id.edit_send:
                    if (outEditText.Visibility == ViewStates.Invisible)
                    {
                        outEditText.Visibility      = ViewStates.Visible;
                        sendButton.Visibility       = ViewStates.Visible;
                        conversationView.Visibility = ViewStates.Visible;
                        LinerLayoutVisibility(false);
                    }
                    else
                    {
                        outEditText.Visibility      = ViewStates.Invisible;
                        sendButton.Visibility       = ViewStates.Invisible;
                        conversationView.Visibility = ViewStates.Invisible;
                        LinerLayoutVisibility(true);

                    }
                    return true;
                case Resource.Id.insecure_connect_scan:                  
                    PairWithBlueToothDevice(false);
                    return true;
                case Resource.Id.secure_connect_scan:
                    PairWithBlueToothDevice(true);
                    return true;
                case Resource.Id.discoverable:
                    EnsureDiscoverable();
                    return true;
            }
            return false;
        }

        private void sendRefresh()
        {
            var msg = "Refresh";
            SendMessage(msg);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Activity.UnregisterReceiver(receiver);
            if (chatService != null)
            {
                chatService.Stop();
            }
        }

        public void PairWithBlueToothDevice(bool secure)
        {
            outDeviceName.Text = "No device connected";
            requestingPermissionsSecure = false;
            requestingPermissionsInsecure = false;

            // Bluetooth is automatically granted by Android. Location, OTOH,
            // is considered a "dangerous permission" and as such has to 
            // be explicitly granted by the user.
            if (!Activity.HasLocationPermissions())
            {
                requestingPermissionsSecure = secure;
                requestingPermissionsInsecure = !secure;
                this.RequestPermissionsForApp();
                return;
            }

            var intent = new Intent(Activity, typeof(DeviceListActivity)); //prueba ast hacemos llamadas aqui a la clase que visualiza activity_device??
            if (secure)
            {
                StartActivityForResult(intent, REQUEST_CONNECT_DEVICE_SECURE);
            }
            else
            {
                StartActivityForResult(intent, REQUEST_CONNECT_DEVICE_INSECURE);
            }
        }


        void SetupChat()
        {
            conversationArrayAdapter = new ArrayAdapter<string>(Activity, Resource.Layout.message);

            conversationView.Adapter = conversationArrayAdapter;
            
            outEditText.SetOnEditorActionListener(writeListener);
            sendButton.Click += (sender, e) =>
            {
                var textView = View.FindViewById<TextView>(Resource.Id.edit_text_out);
                var msg = textView.Text;
                SendMessage(msg);
            };
            toolbarBottom.MenuItemClick += (sender, e) => { ItemSelected(e.Item); };
            schudle1.Click += TimeSelectOnClick;
            schudle2.Click += TimeSelectOnClick;
            schudle3.Click += TimeSelectOnClick;

            chatService     = new BluetoothChatService(handler);
            outStringBuffer = new StringBuilder("");
        }

        void SendMessage(String message)
        {
            if (chatService.GetState() != BluetoothChatService.STATE_CONNECTED)
            {
                Toast.MakeText(Activity, Resource.String.not_connected, ToastLength.Long).Show();
                return;
            }

            if (message.Length > 0)
            {
                var bytes = Encoding.ASCII.GetBytes(message);
                chatService.Write(bytes);
                outStringBuffer.Clear();
                outEditText.Text = outStringBuffer.ToString();
            }
        }

        void TimeSelectOnClick(object sender, EventArgs eventArgs)
        {
            // Instantiate a TimePickerFragment (defined below) 
            string schudleName = ((TextView)sender).TransitionName;
            TimePickerFragment frag = TimePickerFragment.NewInstance(

                // Create and pass in a delegate that updates the Activity time display 
                // with the passed-in time value:
                delegate (DateTime time)
                {
                    ((TextView)sender).Text = "Time: " + time.ToShortTimeString();
                });

            // Launch the TimePicker dialog fragment (defined below):
            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }


        bool HasActionBar()
        {
            if (Activity == null)
            {
                return false;
            }
            if (Activity.ActionBar == null)
            {
                return false;
            }
            return true;
        }

        void SetStatus(int resId)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.SetSubtitle(resId);
            }
        }

        void SetStatus(string subTitle)
        {
            if (HasActionBar())
            {
                Activity.ActionBar.Subtitle = subTitle;
            }
        }

        void ConnectDevice(Intent data, bool secure)
        {
            var address = data.Extras.GetString(DeviceListActivity.EXTRA_DEVICE_ADDRESS);
            var device = bluetoothAdapter.GetRemoteDevice(address);
            chatService.Connect(device, secure);
        }

        public override void OnPrepareOptionsMenu(IMenu menu)
        {
            var menuItem = menu.FindItem(Resource.Id.discoverable);
            var refreshButton = menu.FindItem(Resource.Id.refresh);
            if (menuItem != null)
            {
                menuItem.SetEnabled(bluetoothAdapter.ScanMode == ScanMode.ConnectableDiscoverable);
            }

        }
/// <summary>
/// ///////////////////////////////////////////
/// </summary>
/// <param name="view"></param>
/// <param name="progress"></param>
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
            var identity = ((CircularSeekBar)p0).Id;
            if (identity == Resource.Id.seekbar) 
            {
                power1.Text = "Power1: " + p0.Progress.ToString("0.##\\%") + "\n";
                SendMessage(power1.Text);
            }

            if (identity == Resource.Id.seekbar2) 
            {
                power2.Text = "Power2: " + p0.Progress.ToString("0.##\\%") + "\n";
                SendMessage(power2.Text);
            }

            if (identity == Resource.Id.seekbar3)
            {
                power3.Text = "Power3: " + p0.Progress.ToString("0.##\\%") + "\n";
                SendMessage(power3.Text);
            }
        }

        /// <summary>
        /// Listen for return key being pressed.
        /// </summary>
        class WriteListener : Java.Lang.Object, TextView.IOnEditorActionListener
        {
            BluetoothChatFragment host;
            public WriteListener(BluetoothChatFragment frag)
            {
                host = frag;
            }
            public bool OnEditorAction(TextView v, [GeneratedEnum] ImeAction actionId, KeyEvent e)
            {
                if (actionId == ImeAction.ImeNull && e.Action == KeyEventActions.Up)
                {
                    host.SendMessage(v.Text);
                }
                return true;
            }
        }
        void EnsureDiscoverable()
        {
            if (bluetoothAdapter.ScanMode != ScanMode.ConnectableDiscoverable)
            {
                var discoverableIntent = new Intent(BluetoothAdapter.ActionRequestDiscoverable);
                discoverableIntent.PutExtra(BluetoothAdapter.ExtraDiscoverableDuration, 300);
                StartActivity(discoverableIntent);
            }
        }
    }
}
