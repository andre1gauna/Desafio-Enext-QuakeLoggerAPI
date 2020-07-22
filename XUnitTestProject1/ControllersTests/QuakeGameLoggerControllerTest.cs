using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuakeLogger.AutoMapper;
using QuakeLogger.Controllers;
using QuakeLogger.Data.Context;
using QuakeLogger.Data.Repositories;
using QuakeLogger.Models;
using QuakeLogger.Services;
using QuakeLogger.Tests.Fakes;
using QuakeLogger.ViewModels;
using Xunit;

namespace QuakeLogger.Tests.ControllersTests
{
    public class QuakeGameLoggerControllerTest
    {
        private readonly IMapper _mapper;
        private readonly ContextFake _contextFake;        
        private Parser _parser;
        private QuakeLoggerContext _context;
        private QuakePlayerRepo _repoP;
        private QuakeGameRepo _repoG;
        private QuakeKillMethodRepo _repoKM;        

        public QuakeGameLoggerControllerTest()
        {
            _contextFake = new ContextFake("QuakeGameLoggerControllerTest");
            _context = _contextFake.GetContext("ControllerTestingContext");
            _repoP = new QuakePlayerRepo(_context);
            _repoG = new QuakeGameRepo(_context);
            _repoKM = new QuakeKillMethodRepo(_context);
            _parser = new Parser(_repoG, _repoP, _repoKM);
            _parser.Reader(@"C:\Users\andre\source\repos\QuakeLoggerAPI\testRaw.txt");

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void GetById_Notfound()
        {
            var controller = new QuakeGameLoggerController(_repoG, _repoP, _repoKM, _mapper);
            var result = controller.GetById(int.MaxValue);


            Assert.IsType<ActionResult<GameViewModel>>(result);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void GetById_ShouldWork()
        {
            var controller = new QuakeGameLoggerController(_repoG, _repoP, _repoKM, _mapper);
            var result = controller.GetById(1);

            Assert.IsType<ActionResult<GameViewModel>>(result);
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<GameViewModel>(actionResult.Value);
        }
        
    }
}
