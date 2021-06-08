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
        protected readonly IRepository<Model> repository;
        protected readonly ILogger<P> logger;
        protected readonly Func<Dto, Model> dtoToModelConverter;
        protected readonly Func<Model, Dto> modelToDtoConverter;
        protected readonly Func<Model, int> idGetter;

        public DefaultService(
            IRepository<Model> repository, 
            ILogger<P> logger, 
            Func<Dto,Model> dtoToModelConverter,
            Func<Model, Dto> modelToDtoConverter,
            Func<Model, int> idGetter
            )
        {
            this.repository = repository;
            this.logger = logger;
            this.dtoToModelConverter = dtoToModelConverter;
            this.modelToDtoConverter = modelToDtoConverter;
            this.idGetter = idGetter;
        }
        public void addItem(Dto itemDto)
        {
            //logger.LogInformation($"Invoking repository.save on item {itemDto.ToString()}");
            repository.Save(dtoToModelConverter.Invoke(itemDto));
        }

        public Dto addItemReturning(Dto itemDto)
        {
            //logger.LogInformation($"Invoking repository.save on item {itemDto.ToString()}");
            logger.LogDebug("Converting DTO: " + itemDto + "; to Model. ");
            Model m = dtoToModelConverter.Invoke(itemDto);
            logger.LogDebug("Converted to model: " + m+ ". Then saving in repo...");
            Model notFilled = repository.Save(m);
            logger.LogDebug("Saved in repo. Got not filled:" + notFilled);
            Model withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(notFilled));
            logger.LogDebug("Got with includes from repo:" + withIncludes + ". Returning...");
            return withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
        }

        public Dto getItem(int id)
        {
            Model withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == id);
            return withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
        }

        public void deleteItem(int id)
        {
            if (repository.FindByIdWithoutIncludes(id) != null)
            {
                repository.Delete(id);
            }
        }

        public void editItem(Dto itemDto)
        {
            Model model = dtoToModelConverter.Invoke(itemDto);
            if (repository.Contains(model))
            {
                Model res = repository.Update(model);
            }
        }

        public Dto editItemReturning(Dto itemDto)
        {
            Model model = dtoToModelConverter.Invoke(itemDto);
            if (repository.Contains(model))
            {
                Model withoutIncludes = repository.Update(model);
                Model withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(withoutIncludes));
                return withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
            }

            return null;
        }

        public List<Dto> getAllItems()
        {
            return repository.FindAll().ConvertAll<Dto>(modelToDtoConverter.Invoke);
        }


    }
}
