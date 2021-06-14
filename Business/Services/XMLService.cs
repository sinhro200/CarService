using Business.DTO;
using ClosedXML.Excel;
using Core.Entities;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class XMLService
    {
        private readonly IRepository<User> userRepository;
        private readonly IRepository<Model> modelRepository;

        public XMLService(Core.UnitOfWork unitOfWork)
        {
            this.userRepository = unitOfWork.Users;
            this.modelRepository = unitOfWork.Models;
        }
        public MemoryStream GenerateXml(
            List<OrderDto> orders, 
            OrderFilterDto orderFilters,
            OrderFilterStatisticDto orderFilterStatistic)
        {
            List<User> filterUsers;
            if (orderFilters.OwnerIds == null || orderFilters.OwnerIds.Count == 0)
                filterUsers = null;
            else
                filterUsers = userRepository.FindBy(u => orderFilters.OwnerIds.Contains(u.Id));

            List<Model> filterCars;
            if (orderFilters.CarModelIds == null || orderFilters.CarModelIds.Count == 0)
                filterCars = null;
            else
                filterCars = modelRepository.FindBy(m => orderFilters.CarModelIds.Contains(m.Id));

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
                foreach (Model model in filterCars)
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

            var filtersUnderline = worksheetMain.Range("A" + row + ":F" + row);
            filtersUnderline.Style.Border.BottomBorder = XLBorderStyleValues.Thin;

            row += 2;

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

            row += 2;
            worksheetMain.Cell("D" + row).Value = "Заказы после применения фильтров:";
            worksheetMain.Cell("D" + row).Style.Fill.BackgroundColor = XLColor.LightBlue;
            worksheetMain.Cell("E" + row).Style.Fill.BackgroundColor = XLColor.LightBlue;
            row++;
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

            var table = worksheetMain.Range("A" + row + ":F" + (row + orders.Count));
            table.Style.Border.RightBorder = XLBorderStyleValues.Thin;
            table.Style.Border.BottomBorder = XLBorderStyleValues.Thin;


            double globalSum = 0;
            foreach (OrderDto od in orders)
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
                rowForDetailed += 2;
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
            int briefRow = 1;
            worksheetBrief.Cell("C" + briefRow).Value = "Сводная статистика";
            briefRow++;
            worksheetBrief.Cell("A" + briefRow).Value = "Количество заказов:";
            briefRow++;
            worksheetBrief.Cell("B" + briefRow).Value = orderFilterStatistic.OrderCount;
            worksheetBrief.Cell("B" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
            briefRow++;
            worksheetBrief.Cell("A" + briefRow).Value = "Суммарная выручка:";
            briefRow++;
            worksheetBrief.Cell("B" + briefRow).Value = orderFilterStatistic.Profit;
            worksheetBrief.Cell("B" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
            briefRow += 2;


            worksheetBrief.Cell("A" + briefRow).Value = "Выручка по механикам:";
            briefRow++;
            foreach (KeyValuePair<int, double> e in orderFilterStatistic.MechanicIdToProfitMap)
            {
                worksheetBrief.Cell("B" + briefRow).Value = orderFilterStatistic.MechanicIdToMechanicMap[e.Key].Name;
                worksheetBrief.Cell("C" + briefRow).Value = e.Value;
                worksheetBrief.Cell("C" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                briefRow++;
            }
            briefRow++;

            worksheetBrief.Cell("A" + briefRow).Value = "Выручка по клиентам:";
            briefRow++;
            foreach (KeyValuePair<int, double> e in orderFilterStatistic.UserIdToProfitMap)
            {
                worksheetBrief.Cell("B" + briefRow).Value = orderFilterStatistic.UserIdToUserMap[e.Key].Name;
                worksheetBrief.Cell("C" + briefRow).Value = e.Value;
                worksheetBrief.Cell("C" + briefRow).Style.Fill.BackgroundColor = XLColor.AppleGreen;
                briefRow++;
            }
            briefRow++;

            worksheetBrief.Cell("A" + briefRow).Value = "Частота услуг:";
            briefRow++;
            foreach (KeyValuePair<int, int> e in orderFilterStatistic.ServiceIdToCountMap)
            {
                worksheetBrief.Cell("A" + briefRow).Value = orderFilterStatistic.ServiceIdToServiceMap[e.Key].Title;
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
    }
}
