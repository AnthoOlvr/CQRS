using FnacDarty.JobInterview.Stock.UnitTest.Common;
using FnacDarty.JobInterview.StockService;
using FnacDarty.JobInterview.StockService.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestClass]
    public class StockTest
    {
        private Mock<FlowHandler> _flowHandlerMock;
        private ServiceProvider serviceProvider;
        private List<ProductStock> stock;
        private List<AStockOperation> flows;
        
        [TestInitialize]
        public void InitTest()
        {
            serviceProvider = Factory.ConfigureServices(x =>
                x.RegisterAsMock<IFlowHandler>()
            );
            _flowHandlerMock = new Mock<FlowHandler>(() => new FlowHandler());
            
            stock = new List<ProductStock>();
            stock.Add(new ProductStock
            {
                Product = new Product { EAN = "EAN00001" },
                InventoryDate = new DateTime(),
                quantity = 10
            });
            stock.Add(new ProductStock
            {
                Product = new Product { EAN = "EAN00002" },
                InventoryDate = new DateTime(),
                quantity = 10
            });
            stock.Add(new ProductStock
            {
                Product = new Product { EAN = "EAN00003" },
                InventoryDate = new DateTime(),
                quantity = 10
            });

            flows = new List<AStockOperation>();
            flows.Add(new AStockOperation
            {
                Label = "Achat N°1",
                FlowDate = new DateTime(2020, 1, 1),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00001" }, InventoryDate = new DateTime(), quantity = 10 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Achat N°2",
                FlowDate = new DateTime(2020, 1, 1),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00002" }, InventoryDate = new DateTime(), quantity = 10 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Achat N°3",
                FlowDate = new DateTime(2020, 1, 1),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00003" }, InventoryDate = new DateTime(), quantity = 10 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Cmd N°1",
                FlowDate = new DateTime(2020, 1, 2),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00001" }, InventoryDate = new DateTime(), quantity = -3 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Cmd N°1",
                FlowDate = new DateTime(2020, 1, 2),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00002" }, InventoryDate = new DateTime(), quantity = -3 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Cmd N°1",
                FlowDate = new DateTime(2020, 1, 2),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00003" }, InventoryDate = new DateTime(), quantity = -3 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Cmd N°2",
                FlowDate = new DateTime(2020, 1, 3),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00001" }, InventoryDate = new DateTime(), quantity = -1 }
            });
            flows.Add(new AStockOperation
            {
                Label = "Cmd N°2",
                FlowDate = new DateTime(2020, 1, 3),
                ProductMovement = new ProductStock { Product = new Product { EAN = "EAN00002" }, InventoryDate = new DateTime(), quantity = -10 }
            });

            _flowHandlerMock.Object.Flows = flows;
            _flowHandlerMock.Object.ProductStocks = stock;
        }

        private void Verify()
        {
            _flowHandlerMock.Verify();
            _flowHandlerMock.VerifyAll();
            _flowHandlerMock.VerifyNoOtherCalls();

            serviceProvider.Verify();
        }

        [TestMethod]
        public void ShouldAddQuantityWithoutErrors()
        {
            var input = new List<ProductStock>();
            _flowHandlerMock.Object.AddQuantity(input);
        }

        [TestMethod]
        public void ShouldDeleteQuantityWithoutErrors()
        {
            var input = new List<ProductStock>();

            _flowHandlerMock.Object.RemoveQuantity(input);
        }

        [TestMethod]
        public void ShouldAddFlowsWithoutErrors()
        {
            var input = new List<AStockOperation>();
            _flowHandlerMock.Object.AddMovements(input);
        }

        [TestMethod]
        public void ShouldReturnStockAtGivenDateWithoutErrors()
        {
            var expectedResult = 7;

            var inputEAN = "EAN00001";
            var inputDate = new DateTime(2020, 1, 2);

            var result = _flowHandlerMock.Object.GetProductStockFromDate(inputEAN, inputDate);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ShouldReturnStockVariationWithoutErrors()
        {
            var startDate = new DateTime(2020, 1, 1);
            var endDate = new DateTime(2020, 1, 4);
            var product = new Product { EAN = "EAN00003" };

            int expectedResult = -3; 

            int result =  _flowHandlerMock.Object.GetStockVariation(product, startDate, endDate);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ShouldReturnActualStockForAProductWithoutErrors()
        {
            var input = "EAN00002";
            int expectedResult = 10;

            int result = _flowHandlerMock.Object.GetActualProductStock(input);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void ShouldReturnActualStockWithoutErrors()
        {
            var expectedResult = stock.Select(x => x.Product);
            bool sameList = true;

            List<Product> result = _flowHandlerMock.Object.GetProductsInStock();

            foreach (var value in result)
            {
                if (!expectedResult.Contains(value)) 
                {
                    sameList = false;
                }
            }

            Assert.AreEqual(true, sameList); ;
        }

        [TestMethod]
        public void ShouldReturnStockTotalQuantityWithoutErrors()
        {
            int expectedResult = 30;

            int result = _flowHandlerMock.Object.GetTotalProductsInStock();

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void InventoryCheckShouldChangeValueWithoutErrors()
        {
            Product product = new Product { EAN = "EAN00002" };
            int quantity = 1;

            int expectedResult = 1;

            _flowHandlerMock.Object.InventoryCheck(product, quantity);
            var result = _flowHandlerMock.Object.ProductStocks.Where(x => x.Product == product).Select(y => y.quantity);

            if (result.Count() == 1)
            {
                Assert.AreEqual(expectedResult, result.First());
            }
            else
            {
                Assert.Fail();
            }
        }
    }
}
