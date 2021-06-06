﻿using Business.DTO;
using Business.Interfaces;
using Core;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Business.Services
{
    public class BrandService : DefaultService<BrandDto, Brand, BrandService>, IBrandService
    {

        public BrandService(ILogger<BrandService> logger, UnitOfWork repos)
            : base(repos.Brands, logger,
                  dto =>
                  {
                      return new Brand { Id = dto.Id, Title = dto.Title };
                  },
                  model =>
                  {
                      return new BrandDto { Id = model.Id, Title = model.Title};
                  }
                  )
        { }
    }
}
