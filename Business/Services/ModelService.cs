using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public class ModelService : DefaultService<ModelDto, Model, ModelService>, IModelService
    {

        public ModelService(ILogger<ModelService> logger, UnitOfWork repos, IBrandService brandService)
            : base(repos.Models, logger,
                  dto =>
                  {
                      return new Model { Id = dto.Id, Title = dto.Title, BrandId = dto.BrandDto.Id };
                  },
                  model =>
                  {
                      return new ModelDto { 
                          Id = model.Id, Title = model.Title,
                          BrandDto = new BrandDto { Id = model.Brand.Id, Title = model.Brand.Title },
                          //BrandDto = brandService.getItem(model.BrandId)
                      };
                  },
                  model => model.Id
                  )
        { }
        //public new ModelDto addItem(ModelDto itemDto)
        //{
        //    //logger.LogInformation($"Invoking repository.save on item {itemDto.ToString()}");
        //    Model modelToSave = dtoToModelConverter.Invoke(itemDto);
        //    Model result = repository.Save(modelToSave);
        //    Model withIncludes = repository.SingleOrNull(m => m.Id == result.Id);
        //    return modelToDtoConverter.Invoke(withIncludes);
        //}
    }
}
