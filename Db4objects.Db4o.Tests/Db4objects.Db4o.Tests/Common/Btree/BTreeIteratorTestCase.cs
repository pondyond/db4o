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

using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4oUnit;
using Db4oUnit.Data;

namespace Db4objects.Db4o.Tests.Common.Btree
{
    /// <exclude></exclude>
    public class BTreeIteratorTestCase : BTreeTestCaseBase
    {
        public virtual void TestEmpty()
        {
            var iterator = _btree.Iterator(Trans());
            Assert.IsNotNull(iterator);
            Assert.IsFalse(iterator.MoveNext());
        }

        public virtual void TestOneKey()
        {
            _btree.Add(Trans(), 1);
            var iterator = _btree.Iterator(Trans());
            Assert.IsTrue(iterator.MoveNext());
            Assert.AreEqual(1, iterator.Current);
            Assert.IsFalse(iterator.MoveNext());
        }

        public virtual void TestManyKeys()
        {
            for (var keyCount = 50; keyCount < 70; keyCount++)
            {
                _btree = NewBTree();
                var keys = RandomPositiveIntegersWithoutDuplicates(keyCount);
                var keyIterator = keys.GetEnumerator();
                while (keyIterator.MoveNext())
                {
                    var currentKey = (int) keyIterator.Current;
                    _btree.Add(Trans(), currentKey);
                }
                Iterator4Assert.SameContent(keys.GetEnumerator(), _btree.Iterator(Trans()));
            }
        }

        private IEnumerable RandomPositiveIntegersWithoutDuplicates(int keyCount)
        {
            var generator = Generators.Take(keyCount, Streams.RandomIntegers());
            var res = new Collection4();
            var i = generator.GetEnumerator();
            while (i.MoveNext())
            {
                var currentInteger = (int) i.Current;
                if (currentInteger < 0)
                {
                    currentInteger = -currentInteger;
                }
                if (!res.Contains(currentInteger))
                {
                    res.Add(currentInteger);
                }
            }
            return res;
        }
    }
}