#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Plugins\Injection\CircleReferences\Plugin02.cs) is part of CiviKey. 
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
using CK.Plugin;
using CK.Plugin.Config;
using NUnit.Framework;

namespace Injection.CircleRef
{
    public interface Service02 : IDynamicService
    {
        bool IsRunning { get; }
    }

    [Plugin( "{CD20AB53-6D77-41B8-BC8A-5D95519B1094}" )]
    public class Plugin02 : IPlugin, Service02
    {
        bool _running;

        [DynamicService( Requires=RunningRequirement.MustExistAndRun )]
        public IService<Service03> ServiceWrapped { get; set; }

        public bool Setup( IPluginSetupInfo info )
        {
            return _running = true;
        }

        public void Start()
        {
            Assert.That( ServiceWrapped != null );
            Assert.That( ServiceWrapped.Service.IsRunning );
        }

        public void Teardown()
        {
            
        }

        public void Stop()
        {
            
        }

        #region Service01 Members

        public bool IsRunning
        {
            get { return _running; }
        }

        #endregion
    }
}
