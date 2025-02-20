using log4net;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ceitcon_Designer.Utilities
{
    public class SigalrHelper
    {
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region [SignalR Variables Declearation]
        private static IHubProxy HubProxy { get; set; }
        private static string ServerURI = "http://{0}:9896";
        private static HubConnection Connection { get; set; }
        #endregion

        #region SignalR Function Events"

        public static void DisconnectSignalR()
        {
            if (Connection != null)
            {
                try
                {
                    Connection.Stop();
                }
                catch (Exception)
                {


                }
            }
        }
        public static async void ConnectAsync()
        {

            logger.Info("Connect Async Called");
            if (Connection != null)
            {
                logger.Info("Connect Async Called : Connection was not NULL: State" + Connection.State.ToString());

                if (Connection.State == ConnectionState.Disconnected)
                {

                    try
                    {
                        Connection.Closed -= Connection_Closed;
                        Connection.StateChanged -= Connection_StateChanged;
                        Connection.ConnectionSlow -= Connection_ConnectionSlow;
                        Connection.Error -= Connection_Error;
                    }
                    catch (Exception)
                    {
                    }
                    Connection.Dispose();

                    logger.Info("Connect Async Called : State was Disconnected");
                    ServerURI = string.Format(ServerURI, ConfigurationManager.AppSettings["SignalRURL"]);
                    Connection = new HubConnection(ServerURI);

                    Connection.Closed += Connection_Closed;
                    HubProxy = Connection.CreateHubProxy("chatHub");
                    Connection.StateChanged += Connection_StateChanged;
                    Connection.ConnectionSlow += Connection_ConnectionSlow;



                    try
                    {
                        await Connection.Start();
                        logger.Info("Connection Stated Successfully with " + ServerURI);

                        //await HubProxy.Invoke("PlayerSignIn",
                        //    new object[] { PlayerConfiguration.configPlayerFaceID.ToString(), PlayerConfiguration.configPlayerFaceName, PlayerConfiguration.playerfaceLocation, PlayerConfiguration.ipAddress, PlayerConfiguration.hostName, DateTime.Now });


                    }
                    catch (Exception ex)
                    {
                        logger.Info(" Exception: SignalR Connect Sync " + ex.Message);
                        logger.Debug("SignalR Connect Sync:", ex);

                    }
                }
            }
            else
            {


                logger.Info("Connect Async Called : When Connection State was NULL");
                ServerURI = string.Format(ServerURI, ConfigurationManager.AppSettings["SignalRURL"]);
                Connection = new HubConnection(ServerURI);
                // this.Connection.TransportConnectTimeout = TimeSpan.FromDays(1);

                Connection.Closed += Connection_Closed;

                Connection.StateChanged += Connection_StateChanged;
                Connection.ConnectionSlow += Connection_ConnectionSlow;
                // Connection.Error += Connection_Error;

                HubProxy = Connection.CreateHubProxy("chatHub");

                // HubProxy.On("RegisteredPlayer", RegisteredPlayer);


                try
                {
                    await Connection.Start();

                }
                catch (Exception ex)
                {
                    logger.Info(" Exception: SignalR Connect Sync " + ex.Message);
                    logger.Debug("SignalR Connect Sync:", ex);

                }

            }

        }



        private static void Connection_Error(Exception obj)
        {
            logger.Debug("Connection_Error", obj);


            logger.Info("ConnectAsync Called from Connection_Error with Connection State:" + Connection.State.ToString());

        }

        private static void Connection_ConnectionSlow()
        {
            logger.Info("Connection is getting Slow");
        }
        static void Connection_StateChanged(StateChange obj)
        {
            objState = obj;

            logger.Info("Connection_StateChanged:" + " Connection.State New:" + objState.NewState.ToString() + " Connection State Old:" + objState.OldState.ToString());

        }

        static StateChange objState;

        static void Connection_Closed()
        {
            logger.Info("Connection Closed Event Called:" + Connection.State.ToString());


        }

        #endregion

        public static void SendMessage(string Name, string topic, string message)
        {

            try
            {
                if (objState != null && objState.NewState == ConnectionState.Connected)
                    HubProxy.Invoke("SendRTMessage", new object[] { Name, topic, message });
                else
                {
                    ConnectAsync();
                    HubProxy.Invoke("SendRTMessage", new object[] { Name, topic, message });
                }
            }
            catch (Exception ex)
            { }


        }
        public static void SendCDPFile(string PlayerName, string IPAddress, string ComputerName, byte[] cdpFile)
        {

            try
            {
                if (objState != null && objState.NewState == ConnectionState.Connected)
                {
                    
                    HubProxy.Invoke("SendCDPFile", new object[] { PlayerName.ToLower(), IPAddress, ComputerName.ToLower(), cdpFile });
                    logger.Info("Called successfully SendCDPFile 1");
                }
                else
                {
                    ConnectAsync();
                  
                    HubProxy.Invoke("SendCDPFile", new object[] { PlayerName.ToLower(), IPAddress, ComputerName.ToLower(), cdpFile });
                    logger.Info("Called successfully SendCDPFile 2");
                }
            }
            catch (Exception ex)
            {
                logger.Error("SendCDPFile:", ex);
            }


        }

        public static void _SendRegistrateCommand(bool bRegistered, string PlayerName, string IPaddress, string HostName)
        {

            try
            {
                if (objState != null)
                {
                    if (objState.NewState == ConnectionState.Connected)
                    {
                        if (HubProxy != null)
                        {
                            //RegisterPlayer(string PlayerName, string IPAddress, string ComputerName)
                            if (bRegistered)
                            {
                                HubProxy.Invoke("RegisterPlayer", new object[] { PlayerName.ToLower(), IPaddress, HostName.ToLower() });
                            }
                            else
                            {
                                HubProxy.Invoke("UnRegisterPlayer", new object[] { PlayerName.ToLower(), IPaddress, HostName.ToLower() });
                            }

                        }
                    }
                    else
                    {
                        ConnectAsync();
                        if (bRegistered)
                        {
                            HubProxy.Invoke("RegisterPlayer", new object[] { PlayerName.ToLower(), IPaddress, HostName.ToLower() });
                        }
                        else
                        {
                            HubProxy.Invoke("UnRegisterPlayer", new object[] { PlayerName.ToLower(), IPaddress, HostName.ToLower() });
                        }
                    }
                    //else
                    //{
                    //    MessageBox.Show("Not connected");
                    //}
                }
                else
                {
                    MessageBox.Show("Not connected");
                }
            }
            catch (Exception)
            {


            }
        }
    }
}
