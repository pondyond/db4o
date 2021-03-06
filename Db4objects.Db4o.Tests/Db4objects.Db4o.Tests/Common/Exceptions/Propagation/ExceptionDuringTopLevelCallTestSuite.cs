/* This file is part of the db4o object database http://www.db4o.com

Copyright (C) 2004 - 2011  Versant Corporation http://www.versant.com

db4o is free software; you can redistribute it and/or modify it under
the terms of version 3 of the GNU General Public License as published
by the Free Software Foundation.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program.  If not, see http://www.gnu.org/licenses/. */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Config;
using Db4objects.Db4o.Internal.Ids;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Fixtures;

namespace Db4objects.Db4o.Tests.Common.Exceptions.Propagation
{
    public class ExceptionDuringTopLevelCallTestSuite : FixtureBasedTestSuite, IDb4oTestCase
        , IOptOutNetworkingCS, IOptOutIdSystem
    {
        private static readonly FixtureVariable ExceptionBehaviourFixture = FixtureVariable.NewInstance
            ("exc");

        private static readonly FixtureVariable OperationFixture = FixtureVariable.NewInstance("op"
            );

        public override IFixtureProvider[] FixtureProviders()
        {
            return new IFixtureProvider[]
            {
                new Db4oFixtureProvider(), new SimpleFixtureProvider
                    (ExceptionBehaviourFixture, new object[]
                    {
                        new OutOfMemoryErrorPropagationFixture
                            (),
                        new OneTimeDb4oExceptionPropagationFixture(), new OneTimeRuntimeExceptionPropagationFixture
                            (),
                        new RecurringDb4oExceptionPropagationFixture(), new RecurringRuntimeExceptionPropagationFixture
                            (),
                        new RecoverableExceptionPropagationFixture()
                    }),
                new SimpleFixtureProvider(OperationFixture
                    , new object[]
                    {
                        new _TopLevelOperation_123("commit"), new _TopLevelOperation_127
                            ("store"),
                        new _TopLevelOperation_131("activate"), new _TopLevelOperation_145("peek"
                            ),
                        new _TopLevelOperation_149("qbe"), new _TopLevelOperation_153("query")
                    })
            };
        }

        public override Type[] TestUnits()
        {
            return new[]
            {
                typeof (ExceptionDuringTopLevelCallTestUnit
                    )
            };
        }

        public class ExceptionDuringTopLevelCallTestUnit : AbstractDb4oTestCase
        {
            private ExceptionSimulatingIdSystem _idSystem;
            private IIdSystemFactory _idSystemFactory;
            private ExceptionSimulatingStorage _storage;
            private object _unactivated;

            /// <exception cref="System.Exception"></exception>
            protected override void Configure(IConfiguration config)
            {
                if (Platform4.NeedsLockFileThread())
                {
                    config.LockDatabaseFile(false);
                }
                var propagationFixture = CurrentExceptionPropagationFixture
                    ();
                IExceptionFactory exceptionFactory = new _IExceptionFactory_37(propagationFixture
                    );
                _storage = new ExceptionSimulatingStorage(config.Storage, exceptionFactory);
                config.Storage = _storage;
                _idSystemFactory = new _IIdSystemFactory_61(this, exceptionFactory);
                ConfigureIdSystem(config);
            }

            private void ConfigureIdSystem(IConfiguration config)
            {
                Db4oLegacyConfigurationBridge.AsIdSystemConfiguration(config).UseCustomSystem(_idSystemFactory
                    );
            }

            /// <exception cref="System.Exception"></exception>
            protected override void Db4oSetupAfterStore()
            {
                Store(new Item
                    ());
            }

            public virtual void TestExceptionDuringTopLevelCall()
            {
                _unactivated = ((Item
                    ) RetrieveOnlyInstance(typeof (Item
                        )));
                Db().Deactivate(_unactivated);
                _storage.TriggerException(true);
                _idSystem.TriggerException(true);
                var context = new DatabaseContext(Db(), _unactivated);
                CurrentExceptionPropagationFixture().AssertExecute(context, CurrentOperationFixture
                    ());
                if (context.StorageIsClosed())
                {
                    AssertIsNotLocked(FileSession().FileName());
                }
            }

            private void AssertIsNotLocked(string fileName)
            {
                var embeddedConfiguration = Db4oEmbedded.NewConfiguration();
                embeddedConfiguration.IdSystem.UseCustomSystem(_idSystemFactory);
                IObjectContainer oc = Db4oEmbedded.OpenFile(embeddedConfiguration, fileName);
                oc.Close();
            }

            private IExceptionPropagationFixture CurrentExceptionPropagationFixture()
            {
                return ((IExceptionPropagationFixture) ExceptionBehaviourFixture.Value);
            }

            private TopLevelOperation CurrentOperationFixture()
            {
                return ((TopLevelOperation) OperationFixture.Value);
            }

            /// <exception cref="System.Exception"></exception>
            protected override void Db4oTearDownBeforeClean()
            {
                _storage.TriggerException(false);
                _idSystem.TriggerException(false);
            }

            public class Item
            {
                public string _name;
            }

            private sealed class _IExceptionFactory_37 : IExceptionFactory
            {
                private readonly IExceptionPropagationFixture propagationFixture;
                private bool _alreadyCalled;

                public _IExceptionFactory_37(IExceptionPropagationFixture propagationFixture)
                {
                    this.propagationFixture = propagationFixture;
                    _alreadyCalled = false;
                }

                public void ThrowException()
                {
                    try
                    {
                        if (!_alreadyCalled)
                        {
                            propagationFixture.ThrowInitialException();
                        }
                        else
                        {
                            propagationFixture.ThrowShutdownException();
                        }
                    }
                    finally
                    {
                        _alreadyCalled = true;
                    }
                }

                public void ThrowOnClose()
                {
                    propagationFixture.ThrowCloseException();
                }
            }

            private sealed class _IIdSystemFactory_61 : IIdSystemFactory
            {
                private readonly ExceptionDuringTopLevelCallTestUnit _enclosing;
                private readonly IExceptionFactory exceptionFactory;

                public _IIdSystemFactory_61(ExceptionDuringTopLevelCallTestUnit _enclosing, IExceptionFactory
                    exceptionFactory)
                {
                    this._enclosing = _enclosing;
                    this.exceptionFactory = exceptionFactory;
                }

                public IIdSystem NewInstance(LocalObjectContainer container)
                {
                    _enclosing._idSystem = new ExceptionSimulatingIdSystem(container, exceptionFactory
                        );
                    return _enclosing._idSystem;
                }
            }
        }

        private sealed class _TopLevelOperation_123 : TopLevelOperation
        {
            public _TopLevelOperation_123(string baseArg1) : base(baseArg1)
            {
            }

            public override void Apply(DatabaseContext context)
            {
                context._db.Commit();
            }
        }

        private sealed class _TopLevelOperation_127 : TopLevelOperation
        {
            public _TopLevelOperation_127(string baseArg1) : base(baseArg1)
            {
            }

            public override void Apply(DatabaseContext context)
            {
                context._db.Store(new Item());
            }
        }

        private sealed class _TopLevelOperation_131 : TopLevelOperation
        {
            public _TopLevelOperation_131(string baseArg1) : base(baseArg1)
            {
            }

            public override void Apply(DatabaseContext context)
            {
                context._db.Activate(context._unactivated, int.MaxValue);
            }
        }

        private sealed class _TopLevelOperation_145 : TopLevelOperation
        {
            public _TopLevelOperation_145(string baseArg1) : base(baseArg1)
            {
            }

            // - no deactivate test, since it doesn't trigger I/O activity
            // - no getByID test, not refactored to asTopLevelCall, since it has custom, more relaxed exception handling -> InvalidSlotExceptionTestCase
            // FIXME doesn't trigger initial exception - deletes are processed in finally block
            //					new TopLevelOperation("delete") {
            //						@Override
            //						public void apply(DatabaseContext context) {
            //							context._db.delete(context._unactivated);
            //						}
            //					},
            public override void Apply(DatabaseContext context)
            {
                context._db.Ext().PeekPersisted(context._unactivated, 1, true);
            }
        }

        private sealed class _TopLevelOperation_149 : TopLevelOperation
        {
            public _TopLevelOperation_149(string baseArg1) : base(baseArg1)
            {
            }

            public override void Apply(DatabaseContext context)
            {
                context._db.QueryByExample(new Item());
            }
        }

        private sealed class _TopLevelOperation_153 : TopLevelOperation
        {
            public _TopLevelOperation_153(string baseArg1) : base(baseArg1)
            {
            }

            public override void Apply(DatabaseContext context)
            {
                var result = context._db.Query().Execute();
                if (result.HasNext())
                {
                    result.Next();
                }
            }
        }
    }
}