﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace FMCG.BLL
{
    public static class FMCGHubClient
    {
        #region Field
        private static HubConnection _hubCon;
        private static IHubProxy _fmcgHub;
        #endregion

        #region Property
        public static HubConnection Hubcon
        {
            get
            {
                if (_hubCon == null) HubConnect();
                return _hubCon;
            }
            set
            {
                _hubCon = value;
            }
        }
        
        public static IHubProxy FMCGHub
        {
            get
            {
                if (_fmcgHub == null) HubConnect();
                if (Hubcon.State != ConnectionState.Connected) HubConnect();
                return _fmcgHub;
            }
            set
            {
                _fmcgHub = value;
            }
        }
        #endregion
        
        #region Method
        public static void HubConnect()
        {
            //            _hubCon = new HubConnection("http://110.4.40.46/fmcgsl/SignalR");
            _hubCon = new HubConnection("http://localhost:55487/SignalR");
            _fmcgHub = _hubCon.CreateHubProxy("FMCGHub");
            _hubCon.Start().Wait();
        }

        public static void HubDisconnect()
        {
            _hubCon.Stop();
        }
        #endregion
    }
}
