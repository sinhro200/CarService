using System;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IRepository<T>
        where T : class
    {
        T Save(T item);

        void Update(T item);

        void Delete(T item);

        void SaveAll(IEnumerable<T> items);

        T FindById(int id);

        List<T> FindAll();

        List<T> FindBy(Func<T,bool> predicate);

        //List<T> FindIncludeBy(Func<T, bool> predicate, Func<T, bool> aggregate);

    }
}
