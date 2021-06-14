using Business.DTO;
using Business.Interfaces;
using ClosedXML.Excel;
using Core;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Business.Services
{
    public class OrderService : DefaultService<OrderDto, Order, OrderService>, IOrderService
    {

        private readonly IRepository<Car> carRepository;
        private readonly IRepository<Service> serviceRepository;
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Model> modelRepository;
        private readonly XMLService xMLService;

        public OrderService(ILogger<OrderService> logger, UnitOfWork repos, XMLService xMLService)
            : base(repos.Orders, logger,
                  dto =>
                  {
                  Order o = new Order
                  {
                      Id = dto.Id,
                      CarId = dto.Car.Id,
                      OpenDateTime = dto.CreateDateTime,
                      OrderServices = new List<Core.Entities.OrderService>(),
                      //OrderServices = dto.Services.ConvertAll(s =>
                      //{
                      //    Service serv = repos.Services.SingleOrNull(serv => serv.Id == s.ServiceId);
                      //    Core.Entities.OrderService os = new Core.Entities.OrderService
                      //    {
                      //        //проблема в том, что dto.Id ещё нету
                      //        OrderId = dto.Id,
                      //        ServiceId = s.ServiceId,
                      //        Price = serv == null ? -1 : serv.DefaultPrice,
                      //        OrderServiceStatusId = s.Status == null ? 1 : s.Status.Id == 0 ? 1 : s.Status.Id

                      //    };
                      //    return os;
                      //}),
                      Services = dto.Services.ConvertAll(s =>
                          {

                              Service service = repos.Services.FindByIdWithoutIncludes(s.ServiceId);
                              //Service service = repos.Services.SingleOrNull(serv => serv.Id == s.ServiceId);
                              //if(service == null)
                              //{
                              //    throw new System.NullReferenceException("Service is null while get from repo while mapping from dto");
                              //}
                              return service;
                          }
                      )
                  
                      };
                      return o;
                  },
                  order =>
                  {
                      return new OrderDto {
                          Id = order.Id,
                          CreateDateTime = order.OpenDateTime,
                          ClosedDateTime = order.CloseDateTime,
                          IsClosed = order.IsClosed,
                          Services = order.OrderServices.ConvertAll(s => {
                              return new FullServiceDto { 
                                  ServiceId = s.ServiceId,
                                  Price = s.Price,
                                  Title = s.Service.Title,
                                  Status = new FullServiceStatusDto{
                                      Id = s.OrderServiceStatus.Id,
                                      Title = s.OrderServiceStatus.Title
                                  },
                                  Mechanic = s.Mechanic == null ? null : new MechanicDto
                                  {
                                      Id = s.Mechanic.Id,
                                      Name = s.Mechanic.Name
                                  }
                              };
                          }),
                          Car = new CarDto { 
                              Id = order.Car.Id, 
                              Model = new ModelDto { 
                                  Id = order.Car.Model.Id,
                                  BrandDto = new BrandDto { 
                                      Id = order.Car.Model.Brand.Id,
                                      Title = order.Car.Model.Brand.Title
                                  },
                                  Title = order.Car.Model.Title
                              }, 
                              Owner = new UserDto {
                                  Id = order.Car.OwnerId,
                                  Name = order.Car.Owner.Name
                              }
                          }
                      };
                  },
                  brand => brand.Id
                  )
        {
            this.carRepository = repos.Cars;
            this.serviceRepository = repos.Services;
            this.userRepository = repos.Users;
            this.modelRepository = repos.Models;
            this.xMLService = xMLService;
        }

        public new OrderDto addItemReturning(OrderDto dto)
        {
            if (dto.Id <= 0)
            {
                dto.CreateDateTime = System.DateTime.Now;
                logger.LogDebug("Converting DTO: " + dto + "; to Model. ");
                Order order = dtoToModelConverter.Invoke(dto);
                logger.LogDebug("Converted to model: " + order + ". Then saving in repo...");
                Order savedNotFilled = repository.Save(order);
                logger.LogDebug("Saved in repo. Got not filled:" + savedNotFilled);
                foreach (Core.Entities.OrderService os in order.OrderServices)
                {
                    Service relatedService = os.Service;
                    os.Price = relatedService == null ? -1 : relatedService.DefaultPrice;
                    FullServiceStatusDto status = dto.Services.Find(s => s.ServiceId == relatedService.Id).Status;
                    os.OrderServiceStatusId = status == null ? 1 : status.Id == 0 ? 1 : status.Id;
                }
                repository.Update(order);

                Order withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(order));
                logger.LogDebug("Got with includes from repo:" + withIncludes + ". Returning...");
                OrderDto res = withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
                return res;
                //List<Core.Entities.OrderService> orderServices = new List<Core.Entities.OrderService>();
                //logger.LogDebug("Creating OrderServices...");
                //foreach (FullServiceDto s in dto.Services)
                //{
                //    //Service serv = serviceRepository.SingleOrNull(serv => serv.Id == s.ServiceId);
                //    Service serv = serviceRepository.FindByIdWithoutIncludes(s.ServiceId);
                //    Core.Entities.OrderService os = new Core.Entities.OrderService
                //    {
                //        OrderId = savedNotFilled.Id,
                //        ServiceId = serv.Id,
                //        Price = serv == null ? -1 : serv.DefaultPrice,
                //        OrderServiceStatusId = s.Status == null ? 1 : s.Status.Id == 0 ? 1 : s.Status.Id
                //    };
                //    orderServices.Add(os);
                //};
                //savedNotFilled.OrderServices = orderServices;
                //logger.LogDebug("Updating Order with created OrderServices...");
                //savedNotFilled = repository.Update(savedNotFilled);
                //logger.LogDebug("Updated successfully");
                //Order withIncludes = repository.SingleOrNull(m => idGetter.Invoke(m) == idGetter.Invoke(savedNotFilled));
                //logger.LogDebug("Got with includes from repo:" + withIncludes + ". Returning...");
                //OrderDto res = withIncludes == null ? null : modelToDtoConverter.Invoke(withIncludes);
                //return res;
            }
            return null;
        }

        public new void addItem(OrderDto dto)
        {
            if (dto.Id <= 0)
            {
                dto.CreateDateTime = System.DateTime.Now;
                logger.LogDebug("Converting DTO: " + dto + "; to Model. ");
                Order order = dtoToModelConverter.Invoke(dto);
                logger.LogDebug("Converted to model: " + order + ". Then saving in repo...");
                Order savedNotFilled = repository.Save(order);
                logger.LogDebug("Saved in repo. Got not filled:" + savedNotFilled);
                foreach (Core.Entities.OrderService os in order.OrderServices)
                {
                    Service relatedService = os.Service;
                    os.Price = relatedService == null ? -1 : relatedService.DefaultPrice;
                    FullServiceStatusDto status = dto.Services.Find(s => s.ServiceId == relatedService.Id).Status;
                    os.OrderServiceStatusId = status == null ? 1 : status.Id == 0 ? 1 : status.Id;
                }
                repository.Update(order);
            }
        }

        public List<OrderDto> AllOrdersForCar(int carId)
        {
            if (carRepository.SingleOrNull(c=>c.Id==carId) != null)
                return repository.FindBy(o => o.CarId == carId).ConvertAll(modelToDtoConverter.Invoke);
            return new List<OrderDto>();
        }

        public List<FullServiceDto> ServicesForOrder(int orderId)
        {
            Order o = repository.SingleOrNull(o => o.Id == orderId);
            if (o == null)
                return new List<FullServiceDto>();

            return o.OrderServices.ConvertAll(os =>
            {
                User mechanicUser = userRepository.SingleOrNull(u => u.Id == os.MechanicId);
                MechanicDto mech = mechanicUser==null ? null : 
                    new MechanicDto { Id = mechanicUser.Id, Name = mechanicUser.Name };
                return new FullServiceDto
                {
                    ServiceId = os.ServiceId,
                    Price = os.Price,
                    Title = os.Service.Title,
                    Mechanic = mech,
                    Status = new FullServiceStatusDto { Id = os.OrderServiceStatus.Id, Title = os.OrderServiceStatus.Title },
                };
            });
        }

        public void TryCloseOrder(int orderId)
        {
            Order order = repository.SingleOrNull(o => o.Id == orderId);
            if (order == null)
                return;
            foreach( Core.Entities.OrderService os in order.OrderServices)
            {
                if (os.OrderServiceStatusId < 3)
                    return;
            }
            order.IsClosed = true;
            order.CloseDateTime = System.DateTime.Now;
            repository.Update(order);
        }


        private System.Func<Order, bool> IsOrderOkPredicate( OrderFilterDto orderFilterDto)
        {
            return (Order ord) =>
            {
                bool isOk = true;

                if (isOk && orderFilterDto.FinishedStatus != null)
                    isOk = orderFilterDto.FinishedStatus == 1 ||
                           (orderFilterDto.FinishedStatus == 2 && ord.IsClosed) ||
                           (orderFilterDto.FinishedStatus == 3 && !ord.IsClosed);

                if (isOk && orderFilterDto.CarModelIds != null && orderFilterDto.CarModelIds.Count > 0)
                    isOk = orderFilterDto.CarModelIds.Contains(ord.Car.ModelId);
                if (isOk && orderFilterDto.OwnerIds != null && orderFilterDto.OwnerIds.Count > 0)
                    isOk = orderFilterDto.OwnerIds.Contains(ord.Car.OwnerId);

                if (isOk && orderFilterDto.CreationDateTimeMax != null)
                    isOk = ord.OpenDateTime.CompareTo(orderFilterDto.CreationDateTimeMax) < 0;
                if (isOk && orderFilterDto.CreationDateTimeMin != null)
                    isOk = ord.OpenDateTime.CompareTo(orderFilterDto.CreationDateTimeMin) > 0;

                if (isOk && orderFilterDto.ClosedDateTimeMax != null)
                    isOk = ord.OpenDateTime.CompareTo(orderFilterDto.ClosedDateTimeMax) < 0;
                if (isOk && orderFilterDto.ClosedDateTimeMin != null)
                    isOk = ord.OpenDateTime.CompareTo(orderFilterDto.ClosedDateTimeMin) > 0;


                return isOk;
            };   
        }

        public List<OrderDto> OrdersWithFilter(OrderFilterDto orderFilterDto)
        {
            List<OrderDto> result = new List<OrderDto>();
            List<Order> foundUsingFilters = repository.FindBy(IsOrderOkPredicate(orderFilterDto));
            return foundUsingFilters.ConvertAll(modelToDtoConverter.Invoke);
        }

        /**
         * orderCode:
         * 1 - order by name
         * 2 - order by price
         * 3 - order by creationDate
         * 4 - order by closingDate
         * another - default
         */
        public List<OrderDto> OrdersWithFilterOrdeing(OrderFilterDto orderFilterDto, int orderCode, bool isAsc)
        {
            List<OrderDto> result = new List<OrderDto>();
            OrderRepository ordRep = repository as OrderRepository;
            List<Order> foundUsingFilters;
            switch (orderCode)
            {
                case 1:
                    foundUsingFilters = ordRep.FindByWithOrderingOwner(
                        IsOrderOkPredicate(orderFilterDto), isAsc);
                    break;
                case 2:
                    foundUsingFilters = ordRep.FindByWithOrderingSumPrice(
                        IsOrderOkPredicate(orderFilterDto), isAsc);
                    break;
                case 3:
                    foundUsingFilters = ordRep.FindByWithOrderingCreationDate(
                        IsOrderOkPredicate(orderFilterDto), isAsc);
                    break;
                case 4:
                    foundUsingFilters = ordRep.FindByWithOrderingClosingDate(
                        IsOrderOkPredicate(orderFilterDto), isAsc);
                    break;
                    break;
                default:
                    foundUsingFilters = ordRep.FindBy(IsOrderOkPredicate(orderFilterDto));
                    break;
            };
            return foundUsingFilters.ConvertAll(modelToDtoConverter.Invoke);
        }

        /*
        public MemoryStream ToXml(List<OrderDto> orders, OrderFilterDto orderFilters)
        {
            List<User> filterUsers;
            if (orderFilters.OwnerIds == null || orderFilters.OwnerIds.Count == 0)
                filterUsers = null;
            else
                filterUsers= userRepository.FindBy(u => orderFilters.OwnerIds.Contains(u.Id));

            List<Model> filterCars;
            if (orderFilters.CarModelIds == null || orderFilters.CarModelIds.Count == 0)
                filterCars = null;
            else
                filterCars = modelRepository.FindBy(m=>orderFilters.CarModelIds.Contains(m.Id));

            DateTime? filterCreationDateTimeMin = orderFilters.CreationDateTimeMin;
            DateTime? filterCreationDateTimeMax = orderFilters.CreationDateTimeMax;
            DateTime? filterFinishedDateTimeMin = orderFilters.ClosedDateTimeMin;
            DateTime? filterFinishedDateTimeMax = orderFilters.ClosedDateTimeMax;

            String finishedStatus;
            switch (orderFilters.FinishedStatus)
            {
                case 1:
                    finishedStatus = "Все заказы";
                    break;
                case 2:
                    finishedStatus = "Только закрытые";
                    break;
                case 3:
                    finishedStatus = "Только открытые";
                    break;
                default:
                    finishedStatus = "Любой";
                    break;
            }

            var workbook = new XLWorkbook();
            var worksheetMain = workbook.Worksheets.Add("Список заказов");
            var worksheetBrief = workbook.Worksheets.Add("Сводная статистика");
            var worksheetDetailed = workbook.Worksheets.Add("Подробная статистика");
          

            int row = 1;
            int rowForDetailed = 3;

            worksheetMain.Cell("D" + row).Value = "Применённые фильтры:";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.DarkGray;
            worksheetMain.Cell("D" + row).Style.Font.FontColor = XLColor.White;
            row++;

            //Дата создания
            worksheetMain.Cell("A" + row).Value = "Дата создания:";
            row++;
            worksheetMain.Cell("C" + row).Value = "От :";
            worksheetMain.Cell("D" + row).Value = 
                filterCreationDateTimeMin.HasValue ? filterCreationDateTimeMin.Value : "Любая";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;
            worksheetMain.Cell("C" + row).Value = "По:";
            worksheetMain.Cell("D" + row).Value =
                filterCreationDateTimeMax.HasValue ? filterCreationDateTimeMax.Value : "Любая";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;

            //Дата закрытия
            worksheetMain.Cell("A" + row).Value = "Дата закрытия:";
            row++;
            worksheetMain.Cell("C" + row).Value = "От :";
            worksheetMain.Cell("D" + row).Value =
                filterFinishedDateTimeMin.HasValue ? filterFinishedDateTimeMin.Value : "Любая";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;
            worksheetMain.Cell("C" + row).Value = "По:";
            worksheetMain.Cell("D" + row).Value =
                filterFinishedDateTimeMax.HasValue ? filterFinishedDateTimeMax.Value : "Любая";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;

            //Владельцы
            worksheetMain.Cell("A" + row).Value = "Владельцы:";
            row++;
            if (filterUsers == null)
            {
                worksheetMain.Cell("C" + row).Value = "Все";
                worksheetMain.Cell("C" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            }
            else
            {
                foreach (User user in filterUsers)
                {
                    worksheetMain.Cell("C" + row).Value = user.Name;
                    worksheetMain.Cell("C" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
                    row++;
                }
            }
            row++;

            //Машины
            worksheetMain.Cell("A" + row).Value = "Машины:";
            row++;
            if (filterCars == null)
            {
                worksheetMain.Cell("C" + row).Value = "Любые";
                worksheetMain.Cell("C" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            }
            else
            {
                foreach(Model model in filterCars)
                {
                    worksheetMain.Cell("C" + row).Value = model.Brand.Title + " " + model.Title;
                    row++;
                }
            }
            row++;

            //Условие завершения
            worksheetMain.Cell("A" + row).Value = "Условие завершения:";
            row++;
            worksheetMain.Cell("C" + row).Value = finishedStatus;
            worksheetMain.Cell("C" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;

            var filtersUnderline = worksheetMain.Range("A"+row+":F"+row);
            filtersUnderline.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            row +=2;

            worksheetMain.Cell("D" + row).Value = "Всего заказов после фильтрации:";
            row++;
            worksheetMain.Cell("D" + row).Value = orders.Count;
            worksheetMain.Cell("D" + row).Style.Font.Bold = true;
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightGreen;
            row++;

            worksheetMain.Cell("D" + row).Value = "Выручка:";
            row++;
            IXLCell profitCell = worksheetMain.Cell("D" + row);
            profitCell.Style.Font.Bold = true;
            profitCell.Style.Fill.BackgroundColor = XLColor.LightGreen;

            var profitAndCountUnderline = worksheetMain.Range("A" + row + ":F" + row);
            profitAndCountUnderline.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            row +=2;
            worksheetMain.Cell("D" + row).Value = "Заказы после применения фильтров:";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightBlue;
            worksheetMain.Cell("E" + row).Style.Fill.BackgroundColor = XLColor.LightBlue;
            row ++;
            //создадим заголовки у столбцов
            worksheetMain.Cell("A" + row).Value = "Id";
            worksheetMain.Cell("A" + row).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetMain.Cell("B" + row).Value = "Заказчик";
            worksheetMain.Cell("B" + row).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetMain.Cell("C" + row).Value = "Машина";
            worksheetMain.Cell("C" + row).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetMain.Cell("D" + row).Value = "Дата создания заказа";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetMain.Cell("E" + row).Value = "Даза закрытия заказа";
            worksheetMain.Cell("E" + row).Style.Fill.BackgroundColor = XLColor.Gray;
            worksheetMain.Cell("F" + row).Value = "Сумма";
            worksheetMain.Cell("F" + row).Style.Fill.BackgroundColor = XLColor.Gray;
            row++;

            var table = worksheetMain.Range("A"+row+":F" + (row+orders.Count));
            table.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            table.Style.Border.BottomBorder = XLBorderStyleValues.Thin;


            double globalSum = 0;
            foreach(OrderDto od in orders)
            {

                worksheetMain.Cell("A" + row).Value = od.Id;
                worksheetMain.Cell("B" + row).Value = od.Car.Owner.Name;
                worksheetMain.Cell("C" + row).Value = od.Car.Model.BrandDto.Title + " " + od.Car.Model.Title;
                worksheetMain.Cell("D" + row).Value = od.CreateDateTime.ToString("dd/MM/yyyy H:mm");
                DateTime? closeDT = od.ClosedDateTime;
                
                if (closeDT != null)
                    worksheetMain.Cell("E" + row).Value = closeDT.Value.ToString("dd/MM/yyyy H:mm");
                else
                    worksheetMain.Cell("E" + row).Value = "";

                //detailed region
                worksheetDetailed.Cell("A" + rowForDetailed).Value = od.Id;
                worksheetDetailed.Cell("B" + rowForDetailed).Value = od.Car.Owner.Name;
                worksheetDetailed.Cell("C" + rowForDetailed).Value = od.Car.Model.BrandDto.Title + " " + od.Car.Model.Title;
                worksheetDetailed.Cell("D" + rowForDetailed).Value = od.CreateDateTime.ToString("dd/MM/yyyy H:mm");
                if (closeDT != null)
                    worksheetDetailed.Cell("E" + rowForDetailed).Value = closeDT.Value.ToString("dd/MM/yyyy H:mm");
                else
                    worksheetDetailed.Cell("E" + rowForDetailed).Value = "";
                //end detailed region

                double sum = 0;
                rowForDetailed++;
                foreach (FullServiceDto s in od.Services)
                {
                    sum += s.Price;
                    //worksheetDetailed.Cell("B" + rowForDetailed).Value = "Услуга";
                    worksheetDetailed.Cell("C" + rowForDetailed).Value = s.Title;
                    worksheetDetailed.Cell("D" + rowForDetailed).Value = s.Status.Title;
                    worksheetDetailed.Cell("E" + rowForDetailed).Value = s.Mechanic == null ? "" : s.Mechanic.Name;
                    worksheetDetailed.Cell("F" + rowForDetailed).Value = s.Price;
                    rowForDetailed++;
                }
                worksheetMain.Cell("F" + row).Value = sum;

                worksheetDetailed.Cell("E" + rowForDetailed).Value = "Сумма:";
                worksheetDetailed.Cell("E" + rowForDetailed).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                worksheetDetailed.Cell("E" + rowForDetailed).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                worksheetDetailed.Cell("F" + rowForDetailed).Value = sum;
                worksheetDetailed.Cell("F" + rowForDetailed).Style.Fill.BackgroundColor = XLColor.AppleGreen;

                globalSum += sum;
                row++;
                rowForDetailed+=2;
            }
            worksheetMain.Cell("E" + row).Value = "Выручка:";
            worksheetMain.Cell("E" + row).Style.Fill.BackgroundColor = XLColor.Green;
            worksheetMain.Cell("E" + row).Style.Font.FontColor = XLColor.White;
            //worksheet.Cell("F" + row).Value = ;
            //worksheetMain.Cell("F" + row).FormulaA1 = $"=СУММ(F2:F{(row - 1)})";
            worksheetMain.Cell("F" + row).Value = globalSum;
            worksheetMain.Cell("F" + row).Style.Fill.BackgroundColor = XLColor.Green;
            worksheetMain.Cell("F" + row).Style.Font.FontColor = XLColor.White;

            profitCell.Value = globalSum;

            worksheetDetailed.Cell("E" + rowForDetailed).Style.Fill.BackgroundColor = XLColor.Green;
            worksheetDetailed.Cell("E" + rowForDetailed).Value = "Выручка:";
            worksheetDetailed.Cell("F" + rowForDetailed).Style.Fill.BackgroundColor = XLColor.Green;
            worksheetDetailed.Cell("F" + rowForDetailed).Value = globalSum;


            worksheetMain.Column("C").Width = 14;
            worksheetMain.Column("D").Width = 20;
            worksheetMain.Column("E").Width = 20;

            worksheetDetailed.Column("C").Width = 14;
            worksheetDetailed.Column("D").Width = 20;
            worksheetDetailed.Column("E").Width = 20;
            //пример изменения стиля ячейки
            //worksheet.Cell("B" + 2).Style.Fill.BackgroundColor = XLColor.Red;

            // пример создания сетки в диапазоне
            //var rngTable = worksheet.Range("A1:G" + 10);
            //rngTable.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            //rngTable.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            //worksheet.Columns().AdjustToContents(); //ширина столбца по содержимому
            OrderFilterStatisticDto stat = StatisticOrdersWithFilter(orderFilters);
            int briefRow = 1;
            worksheetBrief.Cell("C" + briefRow).Value = "Сводная статистика";
            briefRow++;
            worksheetBrief.Cell("A" + briefRow).Value = "Количество заказов:";
            briefRow++;
            worksheetBrief.Cell("B" + briefRow).Value = stat.OrderCount;
            worksheetBrief.Cell("B" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
            briefRow++;
            worksheetBrief.Cell("A" + briefRow).Value = "Суммарная выручка:";
            briefRow++;
            worksheetBrief.Cell("B" + briefRow).Value = stat.Profit;
            worksheetBrief.Cell("B"+ briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
            briefRow+=2;


            worksheetBrief.Cell("A" + briefRow).Value = "Выручка по механикам:";
            briefRow++;
            foreach (KeyValuePair<int, double> e in stat.MechanicIdToProfitMap)
            {
                worksheetBrief.Cell("B"+briefRow).Value = stat.MechanicIdToMechanicMap[e.Key].Name;
                worksheetBrief.Cell("C" + briefRow).Value = e.Value;
                worksheetBrief.Cell("C" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                briefRow++;
            }
            briefRow++;

            worksheetBrief.Cell("A"+briefRow).Value = "Выручка по клиентам:";
            briefRow++;
            foreach (KeyValuePair<int, double> e in stat.UserIdToProfitMap)
            {
                worksheetBrief.Cell("B" + briefRow).Value = stat.UserIdToUserMap[e.Key].Name;
                worksheetBrief.Cell("C" + briefRow).Value = e.Value;
                worksheetBrief.Cell("C" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                briefRow++;
            }
            briefRow++;

            worksheetBrief.Cell("A" + briefRow).Value = "Частота услуг:";
            briefRow++;
            foreach (KeyValuePair<int, int> e in stat.ServiceIdToCountMap)
            {
                worksheetBrief.Cell("A" + briefRow).Value = stat.ServiceIdToServiceMap[e.Key].Title;
                worksheetBrief.Cell("C" + briefRow).Value = e.Value;
                worksheetBrief.Cell("C" + briefRow).Style.Fill.BackgroundColor = XLColor.LightGreen;
                briefRow++;
            }
            briefRow++;

            worksheetBrief.Column("A").Width = 20;
            worksheetBrief.Column("B").Width = 20;



            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream);

            return stream;
            
        }
        */
        public MemoryStream ToXml(List<OrderDto> orders, OrderFilterDto orderFilters)
        {
            return xMLService.GenerateXml(orders, orderFilters, StatisticOrdersWithFilter(orderFilters));
        }
        public OrderFilterStatisticDto StatisticOrdersWithFilter(OrderFilterDto orderFilterDto)
        {
            
            List<Order> foundUsingFilters = repository.FindBy(IsOrderOkPredicate(orderFilterDto));

            Dictionary<int, MechanicDto> mechanicsById = new Dictionary<int, MechanicDto>();
            Dictionary<int, UserDto> usersById = new Dictionary<int, UserDto>();
            Dictionary<int, ServiceDto> servicesById = new Dictionary<int, ServiceDto>();

            Dictionary<int, double> mechIdToProfit = new Dictionary<int, double>();
            Dictionary<int, double> userIdToProfit = new Dictionary<int, double>();
            Dictionary<int, int> servicesIdToCount = new Dictionary<int, int>();

            List<OrderDto> afterFiltersDto = foundUsingFilters.ConvertAll(modelToDtoConverter.Invoke);

            double globalProfit = 0;
            foreach (OrderDto ord in afterFiltersDto)
            {
                double orderProfit = 0.0;
                foreach(FullServiceDto os in ord.Services)
                {
                    if (os.Status.Id < 2)
                        continue;
                    if(os.Mechanic == null)
                    {
                        logger.LogCritical("OrderService has status 2[finished] but no mechanic, " + os );
                        continue;
                    }

                    if (!mechIdToProfit.ContainsKey(os.Mechanic.Id))
                    {
                        mechIdToProfit[os.Mechanic.Id] = 0;
                        mechanicsById[os.Mechanic.Id] = os.Mechanic;
                    }
                    mechIdToProfit[os.Mechanic.Id] += os.Price;

                    

                    if (!servicesIdToCount.ContainsKey(os.ServiceId))
                    {
                        servicesIdToCount[os.ServiceId] = 0;
                        servicesById[os.ServiceId] = os;
                    }
                    servicesIdToCount[os.ServiceId] += 1;

                    

                    orderProfit += os.Price;
                }
                if (!userIdToProfit.ContainsKey(ord.Car.Owner.Id))
                {
                    userIdToProfit[ord.Car.Owner.Id] = 0;
                    usersById[ord.Car.Owner.Id] = ord.Car.Owner;
                }
                userIdToProfit[ord.Car.Owner.Id] += orderProfit;

                

                globalProfit += orderProfit;
            }

            //Dictionary<MechanicDto, double> mechToProfit = new Dictionary<MechanicDto, double>();
            //Dictionary<UserDto, double> userToProfit = new Dictionary<UserDto, double>();
            //Dictionary<ServiceDto, int> servicesToCount = new Dictionary<ServiceDto, int>();

            //foreach(KeyValuePair<int,double> p in mechIdToProfit)
            //    mechToProfit[mechanicsById[p.Key]] = p.Value;
            //foreach (KeyValuePair<int, double> p in userIdToProfit)
            //    userToProfit[usersById[p.Key]] = p.Value;
            //foreach (KeyValuePair<int, int> p in servicesIdToCount)
            //    servicesToCount[servicesById[p.Key]] = p.Value;


            OrderFilterStatisticDto result = new OrderFilterStatisticDto {
                Profit = globalProfit,
                OrderCount = afterFiltersDto.Count,
                MechanicIdToProfitMap = mechIdToProfit,//.OrderByDescending(pair => pair.Value)
                    //.ToDictionary(pair => pair.Key, pair => pair.Value),
                UserIdToProfitMap = userIdToProfit,//.OrderByDescending(pair => pair.Value)
                    //.ToDictionary(pair => pair.Key, pair => pair.Value),
                ServiceIdToCountMap = servicesIdToCount,//.OrderByDescending(pair => pair.Value)
                    //.ToDictionary(pair => pair.Key, pair => pair.Value),
                MechanicIdToMechanicMap = mechanicsById,
                ServiceIdToServiceMap = servicesById,
                UserIdToUserMap = usersById
            };

            return result;
        }
    }
}
