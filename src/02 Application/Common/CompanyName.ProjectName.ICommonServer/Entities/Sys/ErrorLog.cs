﻿/**************************************************************************
 * 作者：X
 * 日期：2017.01.18
 * 描述：
 * 修改记录：
 * ***********************************************************************/

using CompanyName.ProjectName.Core;
using CompanyName.ProjectName.Enum;
using System;

namespace CompanyName.ProjectName.ICommonServer
{
    public class ErrorLog : Entity, IEntity
    {
        /// <summary>
        /// 来源
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string LogLevel { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string StackTrace { get; set; }

        public DateTime CreatorTime { get; set; }
    }
}