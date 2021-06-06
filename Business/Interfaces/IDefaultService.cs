using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Interfaces
{
    public interface IDefaultService<T>
    {
        public T addItem(T itemDto);

        public T getItem(int id);

        public T editItem(T itemDto);

        public T deleteItem(int id);

        public List<T> getAllItems();
    }
}
