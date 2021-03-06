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
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Data;
using Sharpen;

namespace Db4objects.Drs.Tests
{
	public class DateReplicationTestCase : DrsTestCase
	{
		public virtual void Test()
		{
			ItemDates item1 = new ItemDates(new DateTime(0), new DateTime());
			ItemDates item2 = new ItemDates(new DateTime(10000), new DateTime(Runtime.CurrentTimeMillis
				() - 10000));
			A().Provider().StoreNew(item1);
			A().Provider().StoreNew(item2);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
			IObjectSet found = B().Provider().GetStoredObjects(typeof(ItemDates));
			Iterator4Assert.SameContent(new object[] { item2, item1 }, ReplicationTestPlatform
				.Adapt(found.GetEnumerator()));
		}
	}
}
