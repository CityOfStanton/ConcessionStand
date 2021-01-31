﻿/*
 * Copyright 2021
 * City of Stanton
 * Stanton, Kentucky
 * www.stantonky.gov
 * github.com/CityOfStanton
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using KioskLibrary.Actions;
using static CommonTestLibrary.TestUtils;
using System.Collections.Generic;
using Moq;
using Windows.Web.Http;
using System.Threading.Tasks;
using KioskLibrary.Helpers;
using System.Linq;

namespace KioskLibrary.Spec.Actions
{
    [TestClass]
    public class WebsiteActionSpec
    {
        public static IEnumerable<object[]> GetConstructorTestData()
        {
            yield return new object[] {
                CreateRandomString(), 
                CreateRandomNumber(), 
                CreateRandomString(), 
                Convert.ToBoolean(CreateRandomNumber(0, 1)),
                CreateRandomNumber(),
                (double?)CreateRandomNumber(),
                CreateRandomNumber()
            };
            yield return new object[] {
                null,
                null,
                null,
                Convert.ToBoolean(CreateRandomNumber(0, 1)),
                null,
                null,
                null
            };
        }

        [DataTestMethod]
        [DynamicData(nameof(GetConstructorTestData), DynamicDataSourceType.Method)]
        public void ConstructorTest(string name, int? duration, string path, bool autoScroll, int? scrollDuration, double? scrollInterval, int? scrollResetDelay)
        {
            var action = new WebsiteAction(name, duration, path, autoScroll, scrollDuration, scrollInterval, scrollResetDelay);

            Assert.AreEqual(name, action.Name);
            Assert.AreEqual(duration, action.Duration);
            Assert.AreEqual(path, action.Path);
            Assert.AreEqual(autoScroll, action.AutoScroll);
            Assert.AreEqual(scrollDuration, action.ScrollDuration);
            Assert.AreEqual(scrollInterval, action.ScrollInterval);
            Assert.AreEqual(scrollResetDelay, action.ScrollResetDelay);
        }

        [DataTestMethod]
        [DataRow(true, 0, null)]
        [DataRow(false, 1, "ERROR, ERROR, ERROR")]
        public async Task ValidateFailedAsyncTest(bool validationResult, int errorCount, string errorMessage)
        {
            var randomName= CreateRandomString();
            var randomPath = $"http://{CreateRandomString()}";

            Mock<IHttpHelper> mockHttpClient = new Mock<IHttpHelper>();
            mockHttpClient
                .Setup(x => x.ValidateURI(It.Is<string>(p => p == randomPath), It.Is<HttpStatusCode>(h => h == HttpStatusCode.Ok)))
                .Returns(Task.FromResult((validationResult, errorMessage)));

            var action = new WebsiteAction(
                randomName,
                CreateRandomNumber(),
                randomPath,
                false,
                CreateRandomNumber(),
                (double)CreateRandomNumber(),
                CreateRandomNumber(),
                mockHttpClient.Object);

            var (IsValid, Name, Errors) = await action.ValidateAsync();

            Assert.AreEqual(validationResult, IsValid, $"The result is {validationResult}.");
            Assert.AreEqual(randomName, Name, "The name is correct.");
            Assert.AreEqual(errorCount, Errors.Count, $"The error count is {errorCount}");
            Assert.AreEqual(errorMessage, Errors.FirstOrDefault(), $"The error message is {errorMessage}");
        }
    }
}