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

using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Internal.References
{
    /// <exclude></exclude>
    public class TransactionalReferenceSystem : TransactionalReferenceSystemBase, IReferenceSystem
    {
        public override void Commit()
        {
            TraverseNewReferences(new _IVisitor4_16(this));
            CreateNewReferences();
        }

        public override void AddExistingReference(ObjectReference @ref)
        {
            _committedReferences.AddExistingReference(@ref);
        }

        public override void AddNewReference(ObjectReference @ref)
        {
            _newReferences.AddNewReference(@ref);
        }

        public override void RemoveReference(ObjectReference @ref)
        {
            _newReferences.RemoveReference(@ref);
            _committedReferences.RemoveReference(@ref);
        }

        public override void Rollback()
        {
            CreateNewReferences();
        }

        public virtual void Discarded()
        {
        }

        private sealed class _IVisitor4_16 : IVisitor4
        {
            private readonly TransactionalReferenceSystem _enclosing;

            public _IVisitor4_16(TransactionalReferenceSystem _enclosing)
            {
                this._enclosing = _enclosing;
            }

            public void Visit(object obj)
            {
                var oref = (ObjectReference) obj;
                if (oref.GetObject() != null)
                {
                    _enclosing._committedReferences.AddExistingReference(oref);
                }
            }
        }
    }
}