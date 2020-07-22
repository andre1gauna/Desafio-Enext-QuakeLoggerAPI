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
        private readonly IQuakeKillMethodRepo _repoKM;
        private readonly IMapper _mapper;
        public QuakeGameLoggerController(IQuakeGameRepo repositoryG, IQuakePlayerRepo repositoryP, IQuakeKillMethodRepo repositoryKM, IMapper mapper)
        {
            _repoG = repositoryG;
            _repoP = repositoryP;
            _repoKM = repositoryKM;
            _mapper = mapper;
        }

        //api/QuakeGameLogger/{id}       
        [HttpGet("{id}")]
        public ActionResult<GameViewModel> GetById(int id)
        {
            if(_repoG.GetAll().Count<id)
            {                
               return NotFound();
            }

            var result = _mapper.Map<GameViewModel>(_repoG.FindById(id));
            result.Players = _mapper.Map<List<PlayerViewModel>>(_repoP.FindByGameId(id).Select(p => p.Player));
            result.KillMethods = _mapper.Map<List<KillMethodViewModel>>(_repoKM.GetAll().Where(i => i.GameId==result.Id).ToList());
            result.Players.RemoveAll(x => x.Name == "<world>");         

            return Ok(result);
        }           


    }
}
