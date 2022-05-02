using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MGCap.Domain.ViewModels.Common
{
    public abstract class EntityParentViewModel<TChild> : IEntityParentViewModel<TChild>
        where TChild : IEntityViewModel
    {
        public int ID { get; set; }

        public IList<TChild> Children1 { get; set; }
    }

    public abstract class EntityParentViewModel<T1, T2> : IEntityParentViewModel<T1, T2>
        where T1 : IEntityViewModel
        where T2 : IEntityViewModel
    {
        public int ID { get; set; }

        public IList<T1> Children1 { get; set; }

        public IList<T2> Children2 { get; set; }
    }

    public abstract class EntityParentViewModel<T1, T2, T3> : IEntityParentViewModel<T1, T2, T3>
        where T1 : IEntityViewModel
        where T2 : IEntityViewModel
        where T3 : IEntityViewModel
    {
        public int ID { get; set; }

        public IList<T1> Children1 { get; set; }

        public IList<T2> Children2 { get; set; }

        public IList<T3> Children3 { get; set; }
    }

    public abstract class EntityParentViewModel<T1, T2, T3, T4> : IEntityParentViewModel<T1, T2, T3, T4>
        where T1 : IEntityViewModel
        where T2 : IEntityViewModel
        where T3 : IEntityViewModel
        where T4 : IEntityViewModel
    {
        public int ID { get; set; }

        public IList<T1> Children1 { get; set; }

        public IList<T2> Children2 { get; set; }

        public IList<T3> Children3 { get; set; }

        public IList<T4> Children4 { get; set; }
    }

    #region Interfaces

    public interface IEntityParentViewModel<TChild> : IEntityViewModel 
        where TChild : IEntityViewModel
    {
        [IgnoreDataMember]
        IList<TChild> Children1 { get; set; }
    }

    public interface IEntityParentViewModel<T1, T2> : IEntityParentViewModel<T1> 
        where T1 : IEntityViewModel 
        where T2 : IEntityViewModel
    {
        [IgnoreDataMember]
        IList<T2> Children2 { get; set; }
    }

    public interface IEntityParentViewModel<T1, T2, T3> : IEntityParentViewModel<T1, T2> 
        where T1 : IEntityViewModel 
        where T2 : IEntityViewModel 
        where T3 : IEntityViewModel
    {
        [IgnoreDataMember]
        IList<T3> Children3 { get; set; }
    }

    public interface IEntityParentViewModel<T1, T2, T3, T4> : IEntityParentViewModel<T1, T2, T3>
        where T1 : IEntityViewModel
        where T2 : IEntityViewModel
        where T3 : IEntityViewModel
        where T4 : IEntityViewModel
    {
        [IgnoreDataMember]
        IList<T4> Children4 { get; set; }
    }

    #endregion
}
