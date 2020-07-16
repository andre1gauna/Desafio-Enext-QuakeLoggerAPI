using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuakeLogger.API.ViewModels;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Models;
using QuakeLogger.ViewModels;

namespace QuakeLogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuakeGameLoggerController : ControllerBase
    {
        private readonly IQuakeGameRepo _repoG;
        private readonly IQuakePlayerRepo _repoP;
        private readonly IMapper _mapper;
        public QuakeGameLoggerController(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP, IMapper mapper)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
            _mapper = mapper;
        }

        //api/QuakeGameLogger/{id}       
        [HttpGet("{id}")]
        public ActionResult<GameViewModel<PlayerViewModel>> GetById(int id)
        {                           
            var result = _mapper.Map<GameViewModel<PlayerViewModel>>(_repoG.FindById(id));

            if (result is null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public ActionResult<List<PlayerViewModel>> GetPlayers()
        {
            var result = _mapper.Map<List<PlayerViewModel>>(_repoP.GetAll());

            if (result is null)
                return NotFound();

            return Ok(result);
        }


    }
}
