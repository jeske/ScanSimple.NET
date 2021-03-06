﻿// ScanSimple
// Copyright (C) 2017, by David W. Jeske, all Rights Reserved

using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

namespace ScanWorker
{


    // helpful docs on .net remoting
    //
    // http://www.codeguru.com/csharp/csharp/cs_syntax/remoting/article.php/c9251/NET-Remoting-Using-a-New-IPC-Channel.htm
    // https://stackoverflow.com/questions/735822/working-with-singletons-in-net-remoting
    //


    public class Program
    {
        public static string APP_NAME = "ScanSimple";
        public static string SERVICE_NAME = "ScanWorker";
        static ObjRef wellKnown;

        static void SetupRPCListener(MainForm mainForm) {

            // connect to the scanner controller
            var serverProvider = new BinaryServerFormatterSinkProvider();
            var clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            var props = new Hashtable();
            props["name"] = "ipc";            
            props["portName"] = APP_NAME;

            IpcChannel serverChannel = new IpcChannel(props, clientProvider, serverProvider);            
            
            ChannelServices.RegisterChannel(serverChannel);
            RemotingConfiguration.ApplicationName = APP_NAME;
            
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(ScanWorker), SERVICE_NAME, WellKnownObjectMode.Singleton);

            ScanWorker myScanWorker = new ScanWorker(mainForm);
            wellKnown = RemotingServices.Marshal(myScanWorker, SERVICE_NAME, typeof(ScanWorker));
            Console.WriteLine("object published at: " + wellKnown.URI);

            // Parse the channel's URI.
            string[] urls = serverChannel.GetUrlsForUri(SERVICE_NAME);
            if (urls != null && urls.Length > 0) {
                string objectUrl = urls[0];
                string objectUri;
                string channelUri = serverChannel.Parse(objectUrl, out objectUri);
                Console.WriteLine("The URL is {0}.", String.Join(" : ", urls));
                Console.WriteLine("The object URI is {0}.", objectUri);
                Console.WriteLine("The channel URI is {0}.", channelUri);
            }

            // Show the URIs associated with the channel.
            System.Runtime.Remoting.Channels.ChannelDataStore channelData =
                (System.Runtime.Remoting.Channels.ChannelDataStore)
                serverChannel.ChannelData;
            foreach (string uri in channelData.ChannelUris) {
                Console.WriteLine("The channel URI is {0}.", uri);
            }


        }

        [STAThread]
        static void Main(string[] args) {
            // must be first
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
            }

            Console.WriteLine("Main ThreadId: " + Thread.CurrentThread.ManagedThreadId);
            log4net.Config.BasicConfigurator.Configure();

            MainForm mainForm = new MainForm();
            SetupRPCListener(mainForm);            
            Application.Run(mainForm);
        }
    }
}
