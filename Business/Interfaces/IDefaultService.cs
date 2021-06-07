using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IDefaultService<T>
    {
        public T addItemReturning(T itemDto);

        public void addItem(T itemDto);

        public T getItem(int id);

        public void editItem(T itemDto);

        public T editItemReturning(T itemDto);

        public void deleteItem(int id);

        public List<T> getAllItems();
    }
}
