using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        T Save(T item);

        T Update(T item);

        T Delete(int id);

        T Delete(T item);

        bool Contains(T item);

        void SaveAll(IEnumerable<T> items);

        T FindByIdWithoutIncludes(int id);

        public T SingleOrNull(Func<T, bool> predicate);
        

        List<T> FindAll();

        List<T> FindBy(Func<T,bool> predicate);

        //List<T> FindIncludeBy(Func<T, bool> predicate, Func<T, bool> aggregate);

    }
}
