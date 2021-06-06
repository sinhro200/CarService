using Business.Interfaces;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class DefaultService<Dto,Model,P> : IDefaultService<Dto> 
        where Dto : class
        where Model : class
    {
        private readonly IRepository<Model> repository;
        protected readonly ILogger<P> logger;
        protected readonly Func<Dto, Model> dtoToModelConverter;
        protected readonly Func<Model, Dto> modelToDtoConverter;

        public DefaultService(
            IRepository<Model> repository, 
            ILogger<P> logger, 
            Func<Dto,Model> dtoToModelConverter,
            Func<Model, Dto> modelToDtoConverter
            )
        {
            this.repository = repository;
            this.logger = logger;
            this.dtoToModelConverter = dtoToModelConverter;
            this.modelToDtoConverter = modelToDtoConverter;
        }
        public Dto addItem(Dto itemDto)
        {
            //logger.LogInformation($"Invoking repository.save on item {itemDto.ToString()}");
            Model result = repository.Save(dtoToModelConverter.Invoke(itemDto));
            return modelToDtoConverter.Invoke(result);
        }

        public Dto getItem(int id)
        {
            Model model = repository.FindById(id);
            return model == null ? null : modelToDtoConverter.Invoke(model);
        }

        public Dto deleteItem(int id)
        {
            if (repository.FindById(id) != null)
            {
                Model res = repository.Delete(id);
                return modelToDtoConverter.Invoke(res);
            }

            return null;
        }

        public Dto editItem(Dto itemDto)
        {
            Model model = dtoToModelConverter.Invoke(itemDto);
            if (repository.Contains(model))
            {
                Model res = repository.Update(model);
                return modelToDtoConverter.Invoke(res);
            }

            return null;
        }

        public List<Dto> getAllItems()
        {
            return repository.FindAll().ConvertAll<Dto>(modelToDtoConverter.Invoke);
        }


    }
}
