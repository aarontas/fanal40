using System;
using System.Text;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace com.xamarin.samples.bluetooth.bluetoothchat
{
    public partial class BluetoothChatFragment
    {
        /// <summary>
        /// Handles messages that come back from the ChatService.
        /// </summary>
        class ChatHandler : Handler
        {
            BluetoothChatFragment chatFrag;
            public ChatHandler(BluetoothChatFragment frag)
            {
                chatFrag = frag;

            }
            public override void HandleMessage(Message msg)
            {
                switch (msg.What)
                {
                    case Constants.MESSAGE_STATE_CHANGE:
                        switch (msg.What)
                        {
                            case BluetoothChatService.STATE_CONNECTED:
                                chatFrag.SetStatus(chatFrag.GetString(Resource.String.title_connected_to, chatFrag.connectedDeviceName));
                                chatFrag.conversationArrayAdapter.Clear();
                                break;
                            case BluetoothChatService.STATE_CONNECTING:
                                chatFrag.SetStatus(Resource.String.title_connecting);
                                break;
                            case BluetoothChatService.STATE_LISTEN:
                                chatFrag.SetStatus(Resource.String.not_connected);
                                break;
                            case BluetoothChatService.STATE_NONE:
                                chatFrag.SetStatus(Resource.String.not_connected);
                                break;
                        }
                        break;
                    case Constants.MESSAGE_WRITE:
                        var writeBuffer = (byte[])msg.Obj;
                        var writeMessage = Encoding.ASCII.GetString(writeBuffer);
                        chatFrag.conversationArrayAdapter.Add($"Me:  {writeMessage}");
                        break;
                    case Constants.MESSAGE_READ:
                        var readBuffer = (byte[])msg.Obj;
                        var readMessage = Encoding.ASCII.GetString(readBuffer, 0, msg.Arg1);
                        chatFrag.conversationArrayAdapter.Add($"{chatFrag.connectedDeviceName}: {readMessage}");
                        readInputMessage(readMessage);
                        break;
                    case Constants.MESSAGE_DEVICE_NAME:
                        chatFrag.connectedDeviceName = msg.Data.GetString(Constants.DEVICE_NAME);
                        if (chatFrag.Activity != null)
                        {
                            Toast.MakeText(chatFrag.Activity, $"Connected to {chatFrag.connectedDeviceName}.", ToastLength.Short).Show();
                            chatFrag.outDeviceName.Text = "Connected to: " + chatFrag.connectedDeviceName; 
                        }
                        break;
                    case Constants.MESSAGE_TOAST:
                        break;
                }
            }

            private void readInputMessage(string writeMessage)
            {
                ///////////////////////////
                ///    Power Setting   ////
                ///////////////////////////
                if (writeMessage.Contains("PW1:"))
                {
                    var PowerReceived = writeMessage.Substring(writeMessage.Length - 4);
                    chatFrag.seekbarPower.Progress = float.Parse(PowerReceived) / 100;
                    chatFrag.power1.Text = "Power1: " + chatFrag.seekbarPower.Progress.ToString("0.##\\%");
                }
                else if (writeMessage.Contains("PW2:"))
                {
                    var PowerReceived = writeMessage.Substring(writeMessage.Length - 4);
                    chatFrag.seekbarPower2.Progress = float.Parse(PowerReceived) / 100;
                    chatFrag.power2.Text = "Power2: " + chatFrag.seekbarPower2.Progress.ToString("0.##\\%");
                }
                else if (writeMessage.Contains("PW3:"))
                {
                    var PowerReceived = writeMessage.Substring(writeMessage.Length - 4);
                    chatFrag.seekbarPower3.Progress = float.Parse(PowerReceived) / 100;
                    chatFrag.power3.Text = "Power3: " + chatFrag.seekbarPower3.Progress.ToString("0.##\\%");
                }
                ///////////////////////////
                ///     Progress Bar   ////
                ///////////////////////////
                else if (writeMessage.Contains("PB1:"))
                {
                    var ProgressReceived = writeMessage.Substring(writeMessage.Length - 4);
                    var intProgress = int.Parse(ProgressReceived)/100;
                    chatFrag.Txtprogress1.Text = "Battery Status: " + intProgress.ToString() + " %  ";
                    chatFrag.progress1.Progress = intProgress;
                    if (intProgress < 33)
                    {
                        chatFrag.progress1.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else if (intProgress < 66)
                    {
                        chatFrag.progress1.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else
                    {
                        chatFrag.progress1.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                }
                else if (writeMessage.Contains("PB2:"))
                {
                    var ProgressReceived = writeMessage.Substring(writeMessage.Length - 4);
                    var intProgress = int.Parse(ProgressReceived)/100;
                    chatFrag.Txtprogress2.Text = "Battery Voltage: " + intProgress.ToString() + " V  ";
                    intProgress = int.Parse(ProgressReceived) / 15;

                    chatFrag.progress2.Progress = intProgress;
                    if (intProgress < 33)
                    {
                        chatFrag.progress2.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                        
                    }
                    else if (intProgress < 66)
                    {
                        chatFrag.progress2.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else
                    {
                        chatFrag.progress2.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                }
                else if (writeMessage.Contains("PB3:"))
                {
                    var ProgressReceived = writeMessage.Substring(writeMessage.Length - 4);
                    int intProgress = Int32.Parse(ProgressReceived);
                    chatFrag.Txtprogress3.Text = "Battery Intensity: " +intProgress.ToString() + " A ";
                    chatFrag.progress3.Progress = -1;
                    chatFrag.progress3.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                    
                    if (intProgress < 0)
                    {
                        chatFrag.progress3.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else if (intProgress < 2)
                    {
                        chatFrag.progress3.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else
                    {
                        chatFrag.progress3.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                }
                else if (writeMessage.Contains("PB4:"))
                {
                    var ProgressReceived = writeMessage.Substring(writeMessage.Length - 4);
                    var intProgress = int.Parse(ProgressReceived)/100;
                    chatFrag.Txtprogress4.Text = "Led Power: " + intProgress.ToString() + "0 W  ";
                    intProgress = int.Parse(ProgressReceived) / 20;

                    chatFrag.progress4.Progress = intProgress;
                    chatFrag.progress4.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);

                    if (intProgress < 33)
                    {
                        chatFrag.progress4.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else if (intProgress < 66)
                    {
                        chatFrag.progress4.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else
                    {
                        chatFrag.progress4.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                }
                else if (writeMessage.Contains("PB5:"))
                {
                    var ProgressReceived = writeMessage.Substring(writeMessage.Length - 4);
                    var intProgress = int.Parse(ProgressReceived)/100;
                    chatFrag.Txtprogress5.Text = "Board Humidity: " + intProgress.ToString() + " %  ";
                    chatFrag.progress5.Progress = intProgress;
                    if (intProgress < 33)
                    {
                        chatFrag.progress5.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                        
                    }
                    else if (intProgress < 66)
                    {
                        chatFrag.progress5.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else
                    {
                        chatFrag.progress5.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                }
                else if (writeMessage.Contains("PB6:"))
                {
                    var ProgressReceived = writeMessage.Substring(writeMessage.Length - 4);
                    var intProgress = int.Parse(ProgressReceived) / 100;
                    chatFrag.Txtprogress6.Text = "Board Temperature: " + intProgress.ToString() + "º  ";
                    intProgress = int.Parse(ProgressReceived) / 40;

                    chatFrag.progress6.Progress = intProgress;
                    if (intProgress < 33)
                    {
                        chatFrag.progress6.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Red, Android.Graphics.PorterDuff.Mode.Multiply);
                        
                    }
                    else if (intProgress < 66)
                    {
                        chatFrag.progress6.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Yellow, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                    else
                    {
                        chatFrag.progress6.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Green, Android.Graphics.PorterDuff.Mode.Multiply);
                    }
                }

            }
        }
    }
}
