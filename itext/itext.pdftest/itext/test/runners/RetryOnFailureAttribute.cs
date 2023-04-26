/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2023 Apryse Group NV
    Authors: Apryse Software.

    This program is offered under a commercial and under the AGPL license.
    For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

    AGPL licensing:
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Commands;

namespace iText.Test.Runners
{
    /// <summary>
    ///  This class is used for flaky test retry after failure. 
    /// Current implementation: we use specified retryCount = 3
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class RetryOnFailureAttribute : Attribute, IWrapTestMethod, IWrapSetUpTearDown
    {
        private const int retryCount = 3;
        public static int failedAttempts = 0;

        public ActionTargets Targets
        {
            get { return ActionTargets.Suite | ActionTargets.Test; }
        }


        public TestCommand Wrap(TestCommand command)
        {
            return new CustomRetryCommand(command, retryCount);
        }


        public class CustomRetryCommand : DelegatingTestCommand
        {
            private int _retryCount;


            public CustomRetryCommand(TestCommand innerCommand, int retryCount)
                : base(innerCommand)
            {
                _retryCount = retryCount;
            }


            public override TestResult Execute(TestExecutionContext context)
            {
                int count = _retryCount;

                while (count-- > 0)
                {
                    context.CurrentResult = this.innerCommand.Execute(context);
                    var results = context.CurrentResult.ResultState;

                    if (results.Equals(ResultState.Error)
                        || results.Equals(ResultState.Failure)
                        || results.Equals(ResultState.SetUpError)
                        || results.Equals(ResultState.SetUpFailure)
                        || results.Equals(ResultState.TearDownError)
                        || results.Equals(ResultState.ChildFailure)
                        || results.Equals(ResultState.Cancelled))
                    {
                        Console.WriteLine("Test Failed on attempt #" + (failedAttempts + 1));
                        failedAttempts++;
                    }
                    else
                    {
                        Console.WriteLine("Test Passed on attempt #" + (failedAttempts + 1));
                        break;
                    }
                }

                return context.CurrentResult;
            }
        }

    }
}
