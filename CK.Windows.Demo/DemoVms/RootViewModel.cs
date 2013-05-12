﻿#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\RootViewModel.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;
using CK.Windows.App;
using System.Threading;
using System.Windows.Threading;

namespace CK.Windows.Demo
{
    internal class RootViewModel : ConfigPage
    {
        SubViewModel _subvm;

        public RootViewModel(AppViewModel app, ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Root view";

            var action = new ConfigItemAction( this.ConfigManager, new SimpleCommand( () => MessageBox.Show( "Pow!" ) ) );
            action.ImagePath = "Forward.png";
            action.DisplayName = "Show a popup";
            this.Items.Add( action );

            // Using extension methods.
            this.AddAction( "CiviKey window, not Activable.", () => new DemoVms.NonActivableCiviKeyWindow().Show() );
            this.AddAction( "CiviKey window, not Activable (secondary thread).", SecondThread );
            this.AddAction( "CiviKey window, Activable with TextBox.", () => new DemoVms.ActivableCiviKeyWindowWithText().Show() );
            this.AddAction( "CiviKey Text Buffer.", () => new DemoVms.CiviKeyTextBuffering().Show() );
            this.AddAction( "Standard WPF window with TextBox.", () => new DemoVms.StandardWindow().Show() );

            this.AddAction( "Simple MessageBox.", () => MessageBox.Show( "Another Pow!" ) );
            this.AddAction( "Dialog from ModalViewModel", ShowCustomMessageBox );
            this.AddLink( _subvm ?? ( _subvm = new SubViewModel( app, configManager ) ) );
        }


        class WPFThread
        {
            public readonly Dispatcher Dispatcher;
            readonly object _lock;

            public WPFThread()
            {
                _lock = new object();
                Thread t = new Thread( StartDispatcher );
                t.SetApartmentState( ApartmentState.STA );
                lock( _lock )
                {
                    t.Start();
                    Monitor.Wait( _lock );
                }
                Dispatcher = Dispatcher.FromThread( t );
            }

            void StartDispatcher()
            {
                // This creates the Dispatcher and pushes the job.
                Dispatcher.CurrentDispatcher.BeginInvoke( (System.Action)DispatcherStarted, null );
                // Initializes a SynchronizationContext (for tasks ot other components that would require one). 
                SynchronizationContext.SetSynchronizationContext( new DispatcherSynchronizationContext( Dispatcher.CurrentDispatcher ) ); 
                Dispatcher.Run();
            }

            void DispatcherStarted()
            {
                lock( _lock )
                {
                    Monitor.Pulse( _lock );
                }
            }
        }

        WPFThread _secondThread;
        
        void SecondThread()
        {
            if( _secondThread == null ) _secondThread = new WPFThread();
            _secondThread.Dispatcher.BeginInvoke( (System.Action)(() => new DemoVms.NonActivableCiviKeyWindow().Show()), null );
        }

        /// <summary>
        /// Shows how to use the WPF CustomMsgBox
        /// </summary>
        internal void ShowCustomMessageBox()
        {
            ModalViewModel modalDataContext = new ModalViewModel( "Mise à jour disponible", String.Format( "Testing TextWrapping to make sure that the window {0}won't end up extremely wide.", Environment.NewLine ), true, "Remember my choice test", CustomMsgBoxIcon.Question, 1 );
            modalDataContext.Buttons.Add( new ModalButton( modalDataContext, "OK", null, ModalResult.Ok ) );
            modalDataContext.Buttons.Add( new ModalButton( modalDataContext, "Cancel", () => Console.Out.WriteLine( "Testing Cancel" ), ModalResult.Cancel ) );
            CustomMsgBox b = new CustomMsgBox( ref modalDataContext );
            b.ShowDialog();
            Console.Out.WriteLine( String.Format( "result : {0}", modalDataContext.ModalResult + " \r\n checkbox checked : " + modalDataContext.IsCheckboxChecked ) );
        }
    }
}
