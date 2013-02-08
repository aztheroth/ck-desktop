﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Hosting
{
    enum ServiceRunningRequirementReason
    {
        /// <summary>
        /// Initialized by ServiceData constructor.
        /// </summary>
        Config,

        /// <summary>
        /// Sets by PluginData.CheckReferencesWhenMustExist method.
        /// </summary>
        FromMustExistReference,

        /// <summary>
        /// Sets by ServiceData.GetMustExistService method.
        /// </summary>
        FromGeneralization,

        /// <summary>
        /// Sets by ServiceData.RetrieveOrUpdateTheCommonServiceReferences.
        /// </summary>
        FromServiceConfigToCommonPluginReferences,

        /// <summary>
        /// Sets by ServiceData.RetrieveOrUpdateTheCommonServiceReferences.
        /// </summary>
        FromServiceToCommonPluginReferences,

        /// <summary>
        /// 
        /// </summary>
        FromMustExistSpecialization,
        FromMustExistGeneralization
    }
}
