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

using Db4oUnit;

namespace Db4objects.Db4o.Tests.Common.TA.TA
{
    /// <exclude></exclude>
    public class TAArrayTestCase : TAItemTestCaseBase
    {
        private static readonly int[] Ints1 = {1, 2, 3};
        private static readonly int[] Ints2 = {4, 5, 6};

        private static readonly LinkedList[] List1 =
        {
            LinkedList.NewList
                (5),
            LinkedList.NewList(5)
        };

        private static readonly LinkedList[] List2 =
        {
            LinkedList.NewList
                (5),
            LinkedList.NewList(5)
        };

        public static void Main(string[] args)
        {
            new TAArrayTestCase().RunAll();
        }

        /// <exception cref="System.Exception"></exception>
        protected override object CreateItem()
        {
            var item = new TAArrayItem();
            item.value = Ints1;
            item.obj = Ints2;
            item.lists = List1;
            item.listsObject = List2;
            return item;
        }

        /// <exception cref="System.Exception"></exception>
        protected override void AssertItemValue(object obj)
        {
            var item = (TAArrayItem) obj;
            ArrayAssert.AreEqual(Ints1, item.Value());
            ArrayAssert.AreEqual(Ints2, (int[]) item.Object());
            ArrayAssert.AreEqual(List1, item.Lists());
            ArrayAssert.AreEqual(List2, (LinkedList[]) item.ListsObject());
        }
    }
}