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

using System.ComponentModel;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
    public class DelegateFieldTestCase : AbstractDb4oTestCase, IOptOutSilverlight
    {
        protected override void Store()
        {
            var item = new Item();
            item.changed += delegate { };
            Store(new Holder(item));
        }

        public void Test()
        {
            Assert.IsNull(RetrieveOnlyInstance<Holder>().item.changed);
        }

        public class Item
        {
            public PropertyChangedEventHandler changed;
        }

        public class Holder
        {
            public Item item;

            public Holder(Item item)
            {
                this.item = item;
            }
        }
    }
}