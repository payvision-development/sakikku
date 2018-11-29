// -----------------------------------------------------------------------
// <copyright file="AtomicBoolTests.cs" company="Payvision">
//     Payvision Copyright © 2018
// </copyright>
// -----------------------------------------------------------------------

namespace Payvision.Health.Service.Tests.Threading
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Diagnostics.Health.Threading;

    using Xunit;

    public class AtomicBoolTests
    {
        [Fact]
        public void CompareExchange_Twice_SecondFalse()
        {
            var subject = new AtomicBool();
            Assert.True(subject.CompareExchange(true, false));
            Assert.False(subject.CompareExchange(true, false));
        }

        [Fact]
        public void Exchange_Twice_SecondFalse()
        {
            var subject = new AtomicBool();
            Assert.True(subject.Exchange(true));
            Assert.False(subject.Exchange(true));
        }

        [Fact]
        public void CompareExchange_Concurrent()
        {
            int accessedTimes = 0;
            var gate = new object();
            var evt = new ManualResetEvent(false);
            AtomicBool subject = false;
            Task[] tasks = Enumerable.Repeat(
                                             Task.Run(
                                                      () =>
                                                      {
                                                          lock (gate)
                                                          {
                                                              evt.WaitOne();
                                                          }

                                                          if (subject.CompareExchange(true, false))
                                                          {
                                                              Interlocked.Increment(ref accessedTimes);
                                                          }
                                                      }),
                                             10)
                .ToArray();

            evt.Set();

            Assert.True(Task.WaitAll(tasks, TimeSpan.FromSeconds(10)));
            Assert.Equal(1, accessedTimes);
        }

        [Fact]
        public void Exchange_Concurrent()
        {
            int accessedTimes = 0;
            var gate = new object();
            var evt = new ManualResetEvent(false);
            AtomicBool subject = false;
            Task[] tasks = Enumerable.Repeat(
                                             Task.Run(
                                                      () =>
                                                      {
                                                          lock (gate)
                                                          {
                                                              evt.WaitOne();
                                                          }

                                                          if (subject.Exchange(true))
                                                          {
                                                              Interlocked.Increment(ref accessedTimes);
                                                          }
                                                      }),
                                             10)
                .ToArray();

            evt.Set();

            Assert.True(Task.WaitAll(tasks, TimeSpan.FromSeconds(10)));
            Assert.Equal(1, accessedTimes);
        }
    }
}