using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net.Sockets;

namespace producer
{
    class Program
    {
        private static IPEndPoint consumerIPEngPoint;
        private static UdpClient udpclient = new UdpClient();
        static void Main(string[] args)
        { 
            var consumerIp = ConfigurationManager.AppSettings.Get("consumerIP");
            var consumerPort = int.Parse(ConfigurationManager.AppSettings.Get("consumerPort"));
            consumerIPEngPoint = new IPEndPoint(IPAddress.Parse(consumerIp), consumerPort);
            Console.WriteLine($"consumer: { consumerIPEngPoint}");
            FilterInfoCollection videoDivece = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            VideoCaptureDevice videoSource = new VideoCaptureDevice(videoDivece[0].MonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();

            Console.ReadLine();

        }

        private static void VideoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            var bmp = new Bitmap(eventArgs.Frame, 1025, 667);
            try
            {
                using (var ms = new MemoryStream())
                {
                    bmp.Save(ms, ImageFormat.Jpeg);
                    var bytes = ms.ToArray();
                    udpclient.Send(bytes, bytes.Length, consumerIPEngPoint);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
