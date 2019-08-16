﻿using System;
using Hangfire.States;
using Hangfire.Storage;

namespace dotNET.Hangfire.Job.Code
{
    /// <summary>
    /// 已完成的job设置过期，防止数据无限增长
    /// </summary>
    public class SucceededStateExpireHandler : IStateHandler
    {
        public TimeSpan JobExpirationTimeout;
        public SucceededStateExpireHandler(int jobExpirationTimeout)
        {
            JobExpirationTimeout = TimeSpan.FromMinutes(jobExpirationTimeout);
        }

        public string StateName => SucceededState.StateName;

        public void Apply(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {
            context.JobExpirationTimeout = JobExpirationTimeout;
        }

        public void Unapply(ApplyStateContext context, IWriteOnlyTransaction transaction)
        {

        }
    }
}
