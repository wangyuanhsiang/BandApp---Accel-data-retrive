using Microsoft.Band;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band.Sensors;
using System.IO.IsolatedStorage;
using Windows.Storage.Pickers;
using Windows.Storage;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BandApp___Accel_data_retrive
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IBandInfo[] pairedBands;
        IBandClient bandClient;
        string accelData;
        public MainPage()
        {
            this.InitializeComponent();
            OnLoaded();
        }

        private async void OnLoaded()
        {
            pairedBands = await BandClientManager.Instance.GetBandsAsync();
            bandClient = await BandClientManager.Instance.ConnectAsync(pairedBands[0]);

            bandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            await bandClient.SensorManager.Accelerometer.StartReadingsAsync();


        }

        private async void Accelerometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            IBandAccelerometerReading accel = e.SensorReading;
            accelData = string.Format("X = {0:G4}\nY = {1:G4}\nZ = {2:G4}", accel.AccelerationX, accel.AccelerationY, accel.AccelerationZ);
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                listView.Items.Add(accelData);
                using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    isoStore.CreateDirectory("Accelerometer");
                    IsolatedStorageFileStream fileStream = isoStore.OpenFile("Accelerometer\\myFile.txt", FileMode.Append, FileAccess.Write);
                    using (StreamWriter writeFile = new StreamWriter(fileStream))
                    {
                        writeFile.WriteLine(accelData.ToString());
                        writeFile.Dispose();
                    }
                }

                //using (IsolatedStorageFile Loisf = IsolatedStorageFile.GetUserStoreForApplication())
                //{
                //    if (Loisf.FileExists("myData.store"))
                //    {
                //        using (IsolatedStorageFileStream fstream = Loisf.OpenFile("myData.store", FileMode.Open))
                //        {
                //            IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                //            IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("Accelerometer\\myFile.txt", FileMode.Open, FileAccess.Read);
                //            fileStream.Position = 0;
                //            using (StreamReader reader = new StreamReader(fileStream))
                //            {
                //                tbResult.Text += "\n" + reader.ReadToEnd();
                //            }
                //        }
                //    }

                //}


            }).AsTask();

        }

        private void bStart_Click(object sender, RoutedEventArgs e)
        {
            bandClient.SensorManager.Accelerometer.StartReadingsAsync();
            //using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            //{
            //    using (IsolatedStorageFileStream fstream = isf.CreateFile("myData.store"))
            //    {
            //        StreamWriter writer = new StreamWriter(fstream);
            //        writer.WriteLine(tbTest.Text);
            //        writer.Dispose();
                   
                        
                       
                

            //    }
            //}

        }

        private async void bStop_Click(object sender, RoutedEventArgs e)
        {
           await bandClient.SensorManager.Accelerometer.StopReadingsAsync();
            using (IsolatedStorageFile Loisf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (Loisf.FileExists("myData.store"))
                {
                    using (IsolatedStorageFileStream fstream = Loisf.OpenFile("myData.store", FileMode.Open))
                    {
                        IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
                        IsolatedStorageFileStream fileStream = myIsolatedStorage.OpenFile("Accelerometer\\myFile.txt", FileMode.Open, FileAccess.Read);
                        fileStream.Position = 0;
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            tbResult.Text += "\n" + reader.ReadToEnd();
                        }
                    }
                }

            }
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("file style", new string[] { ".txt" });
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = "accelemeterData.txt";
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, tbResult.Text);
            }
        }
    }
}
