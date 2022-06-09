#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Cqrs.implementation.Infrastructure;
using Cqrs.implementation.Infrastructure.Ports;
using Cqrs.implementation.Stock.Movements;
using NUnit.Framework;

namespace Cqrs.implementation.Stock.Tests
{
    public class Tests
    {
        private IProductStockRepository _productStockRepository;
        private StockHandler _stockHandler;

        [SetUp]
        public void Setup()
        {
            _productStockRepository = new ProductStockRepository();
            _stockHandler = new StockHandler(_productStockRepository);
        }

        [Test]
        public void ShouldMakePurchaseOf20Units()
        {
            var stock = new ProductStock();
            var product = new Product("EAN00001", stock);

            var purchaseCmd = new PurchaseMovement(_stockHandler, product, 10, "Purchase N°1", DateTime.Now);
            purchaseCmd.Execute();

            var purchaseCmd2 = new PurchaseMovement(_stockHandler, product, 10, "Purchase N°2", DateTime.Now);
            purchaseCmd2.Execute();

            Assert.That(_productStockRepository.GetProductsInStock().Any());
            Assert.That(_productStockRepository.GetProductStockByProductEan("EAN00001")!.Quantity == 20);
        }

        [Test]
        public void ShouldMakeOrderOf30Units()
        {
            var stock = new ProductStock();
            var product = new Product("EAN00002", stock);
            var handler = new StockHandler(_productStockRepository);

            var purchaseCmd = new PurchaseMovement(handler, product, 10, "Purchase N°1", DateTime.Now);
            purchaseCmd.Execute();

            var purchaseCmd2 = new OrderMovement(handler, product, 30, "Order N°1", DateTime.Now);
            purchaseCmd2.Execute();

            Assert.That(_productStockRepository.GetProductStockByProductEan("EAN00002")!.Quantity == -20);
        }

        [Test]
        public void ShouldMakeInventoryMovement()
        {
            var stock = new ProductStock();
            var product = new Product("EAN00001", stock);

            var purchaseCmd = new InventoryMovement(_stockHandler, product, 10);
            purchaseCmd.Execute();

            Assert.That(_productStockRepository.GetProductStockByProductEan("EAN00001")!.Quantity == 10);
        }

        [Test]
        public void ShouldKnowStockAtGivenDate()
        {
            var stockDateNow = DateTime.UtcNow.Date;
            var stockDatePast = DateTime.UtcNow.Date.AddDays(-2);

            var stock = new ProductStock();
            var product = new Product("EAN00001", stock);

            var purchaseCmd = new PurchaseMovement(_stockHandler, product, 5, "Purchase N°1", stockDatePast);
            purchaseCmd.Execute();

            var purchaseCmd2 = new PurchaseMovement(_stockHandler, product, 10, "Purchase N°2", stockDateNow);
            purchaseCmd2.Execute();


            var query = new QueryProductStockByDate(_stockHandler, product, stockDatePast);
            Assert.That(query.Query() == 5);

        }


        [Test]
        public void ShouldKnowStockVariationsBetweenGivenDates()
        {
            var stockDateNow = DateTime.UtcNow.Date;
            var stockDatePast = DateTime.UtcNow.Date.AddDays(-2);

            var stock = new ProductStock();
            var product = new Product("EAN00003", stock);

            var purchaseCmd = new PurchaseMovement(_stockHandler, product, 5, "Purchase N°1", stockDatePast);
            purchaseCmd.Execute();

            var purchaseCmd2 = new PurchaseMovement(_stockHandler, product, 10, "Purchase N°2", stockDateNow);
            purchaseCmd2.Execute();

            var queryInterval1 = new QueryProductStocksInterval(_stockHandler, product, stockDatePast, stockDateNow);
            Assert.That(queryInterval1.Query() == 5);

            var orderCmd = new OrderMovement(_stockHandler, product, 30, "Order N°1", stockDateNow);
            orderCmd.Execute();

            var query = new QueryProductStocksInterval(_stockHandler, product, stockDatePast, stockDateNow);
            Assert.That(query.Query() == -25);
        }

        [Test]
        public void ShouldGetProductsInStock()
        {
            var stockDateNow = DateTime.UtcNow.Date;
            var stockDatePast = DateTime.UtcNow.Date.AddDays(-2);

            var stock = new ProductStock();
            var product = new Product("EAN00001", stock);

            var purchaseCmd = new PurchaseMovement(_stockHandler, product, 5, "Purchase N°1", stockDatePast);
            purchaseCmd.Execute();

            var purchaseCmd2 = new PurchaseMovement(_stockHandler, product, 10, "Purchase N°2", stockDateNow);
            purchaseCmd2.Execute();


            var queryStock = new QueryCurrentProductsInStock(_stockHandler);
            Assert.That(queryStock.Query().Any);

            var orderCmd = new OrderMovement(_stockHandler, product, 100, "Order N°1", stockDateNow);
            orderCmd.Execute();

            var queryStock2 = new QueryCurrentProductsInStock(_stockHandler);
            Assert.That(!queryStock2.Query().Any());
        }

        [Test]
        public void ShouldExecuteSeveralStockMovement()
        {
            var stockDateNow = DateTime.UtcNow.Date;

            var stockProduct1 = new ProductStock();
            var stockProduct2 = new ProductStock();

            var product = new Product("EAN00001", stockProduct1);
            var product2 = new Product("EAN00002", stockProduct2);

            var purchaseCmd = new PurchaseMovement(_stockHandler, product, 5, "Purchase N°1", stockDateNow);
            purchaseCmd.Execute();

            var purchaseCmd2 = new PurchaseMovement(_stockHandler, product2, 5, "Purchase N°2", stockDateNow);
            purchaseCmd2.Execute();

            var orderCommand = new PurchaseMovement(_stockHandler, product, 1, "Order N°1", stockDateNow);
            orderCommand.Execute();

            var batch = new BatchedStockMovement(new List<AStockMovement> { purchaseCmd, purchaseCmd2, orderCommand });
            batch.Execute();

            Assert.That(_productStockRepository.GetProductsInStock().Contains(product) &&
                        _productStockRepository.GetProductsInStock().Contains(product2));
        }


        [Test]
        public void ShouldThrowInvalidOperationExceptionOnWrongInventoryDate()
        {
            var stockDate = DateTime.UtcNow.Date.AddDays(-20);
          
            var stockProduct = new ProductStock();
            var product = new Product("EAN00001", stockProduct);
            
            var purchase1 = new PurchaseMovement(_stockHandler, product, 5, "Purchase N°1", stockDate);
            purchase1.Execute();

            var inventoryMovement = new InventoryMovement(_stockHandler, product, 1);
            inventoryMovement.Execute();

            var purchase2 = new PurchaseMovement(_stockHandler, product, 5, "Purchase N°2", stockDate);
            Assert.Throws<InvalidOperationException>(() => purchase2.Execute());

        }

        [Test]
        public void ShouldThrowArgumentExceptionExceptionIfInventoryMovementQuantityIsLowerThanZero()
        {
            var stockProduct = new ProductStock();
            var product = new Product("EAN00001", stockProduct);

            Assert.Throws<ArgumentException>(() => new InventoryMovement(_stockHandler, product, -1));

        }
    }
}