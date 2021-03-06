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
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4oUnit;
using Sharpen;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
    public class Collection4TestCase : ITestCase
    {
        public static void Main(string[] args)
        {
            new ConsoleTestRunner(typeof (Collection4TestCase)).Run();
        }

        public virtual void TestRemoveAll()
        {
            string[] originalElements = {"foo", "bar", "baz"};
            var c = NewCollection(originalElements);
            c.RemoveAll(NewCollection(new string[0]));
            AssertCollection(originalElements, c);
            c.RemoveAll(NewCollection(new[] {"baz", "bar", "zeng"}));
            AssertCollection(new[] {"foo"}, c);
            c.RemoveAll(NewCollection(originalElements));
            AssertCollection(new string[0], c);
        }

        public virtual void TestContains()
        {
            var a = new object();
            var c = new Collection4();
            c.Add(new object());
            Assert.IsFalse(c.Contains(a));
            c.Add(a);
            Assert.IsTrue(c.Contains(a));
            c.Remove(a);
            Assert.IsFalse(c.Contains(a));
        }

        public virtual void TestContainsAll()
        {
            var a = new Item(42);
            var b = new Item(a.id + 1);
            var c = new Item(b.id + 1);
            var a_ = new Item(a.id);
            var needle = new Collection4();
            var haystack = new Collection4();
            haystack.Add(a);
            needle.Add(a);
            needle.Add(b);
            Assert.IsFalse(haystack.ContainsAll(needle));
            needle.Remove(b);
            Assert.IsTrue(haystack.ContainsAll(needle));
            needle.Add(b);
            haystack.Add(b);
            Assert.IsTrue(haystack.ContainsAll(needle));
            needle.Add(a_);
            Assert.IsTrue(haystack.ContainsAll(needle));
            needle.Add(c);
            Assert.IsFalse(haystack.ContainsAll(needle));
            needle.Clear();
            Assert.IsTrue(haystack.ContainsAll(needle));
            haystack.Clear();
            Assert.IsTrue(haystack.ContainsAll(needle));
        }

        public virtual void TestReplace()
        {
            var c = new Collection4();
            c.Replace("one", "two");
            c.Add("one");
            c.Add("two");
            c.Add("three");
            c.Replace("two", "two.half");
            AssertCollection(new[] {"one", "two.half", "three"}, c);
            c.Replace("two.half", "one");
            c.Replace("one", "half");
            AssertCollection(new[] {"half", "one", "three"}, c);
        }

        public virtual void TestNulls()
        {
            var c = new Collection4();
            c.Add("one");
            AssertNotContainsNull(c);
            c.Add(null);
            AssertContainsNull(c);
            AssertCollection(new[] {"one", null}, c);
            c.Prepend(null);
            AssertCollection(new[] {null, "one", null}, c);
            c.Prepend("zero");
            c.Add("two");
            AssertCollection(new[] {"zero", null, "one", null, "two"}, c);
            AssertContainsNull(c);
            c.Remove(null);
            AssertCollection(new[] {"zero", "one", null, "two"}, c);
            c.Remove(null);
            AssertNotContainsNull(c);
            AssertCollection(new[] {"zero", "one", "two"}, c);
            c.Remove(null);
            AssertCollection(new[] {"zero", "one", "two"}, c);
        }

        public virtual void TestGetByIndex()
        {
            var c = new Collection4();
            c.Add("one");
            c.Add("two");
            Assert.AreEqual("one", c.Get(0));
            Assert.AreEqual("two", c.Get(1));
            AssertIllegalIndex(c, -1);
            AssertIllegalIndex(c, 2);
        }

        public virtual void TestIndexOf()
        {
            var c = new Collection4();
            Assert.AreEqual(-1, c.IndexOf("notInCollection"));
            c.Add("one");
            Assert.AreEqual(-1, c.IndexOf("notInCollection"));
            Assert.AreEqual(0, c.IndexOf("one"));
            c.Add("two");
            c.Add("three");
            Assert.AreEqual(0, c.IndexOf("one"));
            Assert.AreEqual(1, c.IndexOf("two"));
            Assert.AreEqual(2, c.IndexOf("three"));
            Assert.AreEqual(-1, c.IndexOf("notInCollection"));
        }

        private void AssertIllegalIndex(Collection4 c, int index)
        {
            Assert.Expect(typeof (ArgumentException), new _ICodeBlock_170(c, index));
        }

        public virtual void TestPrepend()
        {
            var c = new Collection4();
            c.Prepend("foo");
            AssertCollection(new[] {"foo"}, c);
            c.Add("bar");
            AssertCollection(new[] {"foo", "bar"}, c);
            c.Prepend("baz");
            AssertCollection(new[] {"baz", "foo", "bar"}, c);
            c.Prepend("gazonk");
            AssertCollection(new[] {"gazonk", "baz", "foo", "bar"}, c);
        }

        public virtual void TestCopyConstructor()
        {
            string[] expected = {"1", "2", "3"};
            var c = NewCollection(expected);
            AssertCollection(expected, new Collection4(c));
        }

        public virtual void TestInvalidIteratorException()
        {
            var c = NewCollection(new[] {"1", "2"});
            var i = c.GetEnumerator();
            Assert.IsTrue(i.MoveNext());
            c.Add("3");
            Assert.Expect(typeof (InvalidIteratorException), new _ICodeBlock_200(i));
        }

        public virtual void TestRemove()
        {
            var c = NewCollection(new[] {"1", "2", "3", "4"});
            c.Remove("3");
            AssertCollection(new[] {"1", "2", "4"}, c);
            c.Remove("4");
            AssertCollection(new[] {"1", "2"}, c);
            c.Add("5");
            AssertCollection(new[] {"1", "2", "5"}, c);
            c.Remove("1");
            AssertCollection(new[] {"2", "5"}, c);
            c.Remove("2");
            c.Remove("5");
            AssertCollection(new string[] {}, c);
            c.Add("6");
            AssertCollection(new[] {"6"}, c);
        }

        private void AssertCollection(string[] expected, Collection4 c)
        {
            Assert.AreEqual(expected.Length, c.Size());
            Iterator4Assert.AreEqual(expected, c.GetEnumerator());
        }

        private void AssertContainsNull(Collection4 c)
        {
            Assert.IsTrue(c.Contains(null));
            Assert.IsNull(c.Get(null));
            var size = c.Size();
            c.Ensure(null);
            Assert.AreEqual(size, c.Size());
        }

        private void AssertNotContainsNull(Collection4 c)
        {
            Assert.IsFalse(c.Contains(null));
            Assert.IsNull(c.Get(null));
            var size = c.Size();
            c.Ensure(null);
            Assert.AreEqual(size + 1, c.Size());
            c.Remove(null);
            Assert.AreEqual(size, c.Size());
        }

        public virtual void TestIterator()
        {
            string[] expected = {"1", "2", "3"};
            var c = NewCollection(expected);
            Iterator4Assert.AreEqual(expected, c.GetEnumerator());
        }

        private Collection4 NewCollection(string[] expected)
        {
            var c = new Collection4();
            c.AddAll(expected);
            return c;
        }

        public virtual void TestToString()
        {
            var c = new Collection4();
            Assert.AreEqual("[]", c.ToString());
            c.Add("foo");
            Assert.AreEqual("[foo]", c.ToString());
            c.Add("bar");
            Assert.AreEqual("[foo, bar]", c.ToString());
        }

        private class Item
        {
            public readonly int id;

            public Item(int id)
            {
                this.id = id;
            }

            public override int GetHashCode()
            {
                var prime = 31;
                var result = 1;
                result = prime*result + id;
                return result;
            }

            public override bool Equals(object obj)
            {
                if (this == obj)
                {
                    return true;
                }
                if (obj == null)
                {
                    return false;
                }
                if (GetType() != obj.GetType())
                {
                    return false;
                }
                var other = (Item) obj;
                if (id != other.id)
                {
                    return false;
                }
                return true;
            }
        }

        private sealed class _ICodeBlock_170 : ICodeBlock
        {
            private readonly Collection4 c;
            private readonly int index;

            public _ICodeBlock_170(Collection4 c, int index)
            {
                this.c = c;
                this.index = index;
            }

            /// <exception cref="System.Exception"></exception>
            public void Run()
            {
                c.Get(index);
            }
        }

        private sealed class _ICodeBlock_200 : ICodeBlock
        {
            private readonly IEnumerator i;

            public _ICodeBlock_200(IEnumerator i)
            {
                this.i = i;
            }

            /// <exception cref="System.Exception"></exception>
            public void Run()
            {
                Runtime.Out.WriteLine(i.Current);
            }
        }
    }
}