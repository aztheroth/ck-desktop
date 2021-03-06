#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Config.Tests\TestBase.cs) is part of CiviKey. 
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
using System.IO;
using NUnit.Framework;
using System.Reflection;
using CK.Plugin.Config;
using CK.Storage;
using CK.Core;
using CK.SharedDic;
using System.Xml;
using CK.Plugin;

namespace PluginConfig
{

    public class TestBase
    {
        static string _testFolder;
        static string _appFolder;
        static string _pluginFolder;
        static string _configFilePath;

        static DirectoryInfo _testFolderDir;
        static DirectoryInfo _appFolderDir;

        public static string AppFolder
        {
            get
            {
                if( _appFolder == null ) InitalizePaths();
                return _appFolder;
            }
        }

        public static string TestFolder
        {
            get
            {
                if( _testFolder == null ) InitalizePaths();
                return _testFolder;
            }
        }

        public static string PluginFolder
        {
            get
            {
                if( _pluginFolder == null ) InitalizePaths();
                return _pluginFolder;
            }
        }

        public static string ConfigFilePath
        {
            get
            {
                if( _configFilePath == null ) InitalizePaths();
                return _configFilePath;
            }
        }

        public static DirectoryInfo AppFolderDir
        {
            get { return _appFolderDir ?? (_appFolderDir = new DirectoryInfo( AppFolder )); }
        }

        public static DirectoryInfo TestFolderDir
        {
            get { return _testFolderDir ?? (_testFolderDir = new DirectoryInfo( TestFolder )); }
        }

        public static void CleanupTestDir()
        {
            TestFolderDir.Refresh();
            if( TestFolderDir.Exists )
                TestFolderDir.Delete( true );
            TestFolderDir.Create();
        }

        public static void CopyPluginToTestDir( params FileInfo[] files )
        {
            if( _testFolder == null ) InitalizePaths();
            foreach( FileInfo f in files )
            {
                File.Copy( f.FullName, Path.Combine( _testFolder, f.Name ), true );
            }
        }

        public static void CopyPluginToTestDir( params string[] fileNames )
        {
            if( _testFolder == null ) InitalizePaths();
            foreach( string f in fileNames )
            {
                File.Copy( Path.Combine( _pluginFolder, f ), Path.Combine( _testFolder, f ), true );
            }
        }

        private static void InitalizePaths()
        {
            string p = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            // Code base is like "file:///C:/Documents and Settings/Olivier Spinelli/Mes documents/Dev/CK/Output/Debug/App/CVKTests.DLL"
            StringAssert.StartsWith( "file:///", p, "Code base must start with file:/// protocol." );

            p = p.Substring( 8 ).Replace( '/', System.IO.Path.DirectorySeparatorChar );

            // => Debug/
            p = Path.GetDirectoryName( p );
            _appFolder = p;

            // ==> Debug/SubTestDir
            _testFolder = Path.Combine( p, "SubTestDir" );
            if( Directory.Exists( _testFolder ) ) Directory.Delete( _testFolder, true );
            Directory.CreateDirectory( _testFolder );
            
            string p2 = p.ToString();
            _configFilePath = Path.Combine( p2, "Config" );

            // ==> Debug/Plugins
            _pluginFolder = Path.Combine( p, "Plugins" );
        }

        static public void ClearConfig()
        {
            if( Directory.Exists( ConfigFilePath ) )
            {                
                Directory.Delete( ConfigFilePath, true );
            }
            Directory.CreateDirectory( ConfigFilePath );
        }

        /// <summary>
        /// Builds a valid path to a xml file used in this unit test.
        /// </summary>
        /// <param name="name">The file name (will be part of the resulting file name).</param>
        /// <returns>A valid full path.</returns>
        static public string GetTestFilePath( string name )
        {
            return Path.Combine( TestBase.TestFolder, "CKTests.Configuration." + name + ".xml" );
        }

        static public void DumpFileToConsole( string path )
        {
            Console.WriteLine( File.ReadAllText( path ) );
        }

        static public Stream OpenFileResourceStream( string pathInFilesFolder )
        {
            string path = typeof( TestBase ).Namespace + ".Files." + pathInFilesFolder.Replace( '\\', '.' ).Replace( '/', '.' );
            return Assembly.GetExecutingAssembly().GetManifestResourceStream( path );
        }

        static public string GetFileResourceText( string pathInFilesFolder )
        {
            using( Stream s = OpenFileResourceStream( pathInFilesFolder ) )
            using( TextReader r = new StreamReader( s ) )
            {
                return r.ReadToEnd();
            }
        }


    }
}
