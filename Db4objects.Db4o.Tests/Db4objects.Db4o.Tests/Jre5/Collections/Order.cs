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

#if !SILVERLIGHT
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Collections;
using Db4objects.Db4o.Tests.Common.TA;

namespace Db4objects.Db4o.Tests.Jre5.Collections
{
    public class Order : ActivatableImpl
    {
        private readonly ArrayList4<OrderItem> _items;

        public Order()
        {
            _items = new ArrayList4<OrderItem>();
        }

        public virtual void AddItem(OrderItem item)
        {
            Activate(ActivationPurpose.Read);
            _items.Add(item);
        }

        public virtual OrderItem Item(int i)
        {
            Activate(ActivationPurpose.Read);
            return _items[i];
        }

        public virtual int Size()
        {
            Activate(ActivationPurpose.Read);
            return _items.Count;
        }
    }
}

#endif // !SILVERLIGHT