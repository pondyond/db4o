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
using Db4objects.Db4o.Activation;

namespace Db4objects.Db4o.Tests.Common.TA.Hierarchy
{
    internal class UnitOfWork : ActivatableImpl
    {
        internal DateTime _finished;
        internal string _name;
        internal DateTime _started;

        public UnitOfWork(string name, DateTime started, DateTime finished)
        {
            _name = name;
            _started = started;
            _finished = finished;
        }

        public virtual string GetName()
        {
            // TA BEGIN
            Activate(ActivationPurpose.Read);
            // TA END
            return _name;
        }

        public virtual long TimeSpent()
        {
            // TA BEGIN
            Activate(ActivationPurpose.Read);
            // TA END
            return _finished.Ticks - _started.Ticks;
        }
    }
}