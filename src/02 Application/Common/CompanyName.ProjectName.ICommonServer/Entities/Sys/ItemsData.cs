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
    /// <summary>
    /// 数据字典
    /// </summary>
    public class ItemsData : Entity, IEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否生效
        /// </summary>
        public bool? IsEnabled { get; set; }

        /// <summary>
        /// 排序  asc
        /// </summary>
        public int? SortCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public long ParentId { get; set; }

        public DateTime? CreatorTime { get; set; }

        public long? CreatorUserId { get; set; }
    }
}