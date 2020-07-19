using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using QuakeLogger.API.ViewModels;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.ViewModels;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult<GameViewModel> GetById(int id)
        {
            var result = _mapper.Map<GameViewModel>(_repoG.FindById(id));
            result.Players = _mapper.Map<List<PlayerViewModel>>(_repoP.FindByGameId(id).Select(p => p.Player));
            result.Players.RemoveAll(x => x.Name == "<world>");
            

            if (result is null)
                return NotFound();


            return Ok(result);
        }           


    }
}
