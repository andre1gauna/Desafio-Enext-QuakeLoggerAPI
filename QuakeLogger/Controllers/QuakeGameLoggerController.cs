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
        private readonly IQuakeGameRepo _repo;
        private readonly IMapper _mapper;
        public QuakeGameLoggerController(IQuakeGameRepo repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        //api/QuakeGameLogger/{id}       
        [HttpGet("{id}")]
        public ActionResult<GameViewModel<PlayerViewModel>> GetById(int id)
        {                           
            var result = _mapper.Map<GameViewModel<PlayerViewModel>>(_repo.FindById(id));

            if (result is null)
                return NotFound();

            return Ok(result);
        }
             

    }
}
