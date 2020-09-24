using AutoMapper;
using KodoomOstad.DataAccessLayer.Contracts;
using KodoomOstad.Entities.Models;
using KodoomOstad.Services.Services;
using KodoomOstad.WebApi.Models.Answers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace KodoomOstad.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class AnswersController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Answer> _answerRepository;
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Professor> _professorRepository;
        private readonly IRepository<PollQuestion> _pollQuestionRepository;
        private readonly IJwtService _jwtService;

        public AnswersController(IMapper mapper, IRepository<Answer> answerRepository, UserManager<User> userManager, IRepository<Professor> professorRepository, IRepository<PollQuestion> pollQuestionRepository, IJwtService jwtService)
        {
            _mapper = mapper;
            _answerRepository = answerRepository;
            _userManager = userManager;
            _professorRepository = professorRepository;
            _pollQuestionRepository = pollQuestionRepository;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var answers = await _answerRepository.TableNoTracking.ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<AnswersOutputDto>>(answers);

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var answer = await _answerRepository.GetByIdAsync(cancellationToken, id);

            if (answer == null)
                return NotFound();

            var dto = _mapper.Map<AnswersOutputDto>(answer);

            return Ok(dto);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(AnswersInputDto dto, CancellationToken cancellationToken)
        {
            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isUserHimSelfRequesting = userWhoSentRequest.Id == dto.UserId;

            if (!isUserHimSelfRequesting)
                return BadRequest("You can't create answer for others.");


            var user = await _userManager.FindByIdAsync(dto.UserId.ToString());

            var professor = await _professorRepository.GetByIdAsync(cancellationToken, dto.ProfessorId);

            var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, dto.PollQuestionId);


            var notFoundMessages = new List<string>();

            if (user == null)
                notFoundMessages.Add("User not found.");

            if (professor == null)
                notFoundMessages.Add("Professor not found.");

            if (pollQuestion == null)
                notFoundMessages.Add("PollQuestion not found.");

            if (notFoundMessages.Any())
                return NotFound(notFoundMessages);

            if (dto.Score < pollQuestion.MinScore || dto.Score > pollQuestion.MaxScore)
                return BadRequest(
                    $"Score must be something between {pollQuestion.MinScore} and {pollQuestion.MaxScore}");


            var answer = _mapper.Map<Answer>(dto);

            await _answerRepository.AddAsync(answer, cancellationToken);

            var createdAnswer = _mapper.Map<AnswersOutputDto>(answer);

            professor.AverageRate = (int)professor.Answers.Average(x => x.Score);

            await _professorRepository.UpdateAsync(professor, cancellationToken);

            return Created($"api/v1/Answers/{createdAnswer.Id}", createdAnswer);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, AnswersUpdateInputDto dto, CancellationToken cancellationToken)
        {
            var answer = await _answerRepository.GetByIdAsync(cancellationToken, id);

            if (answer == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isUserHimSelfRequesting = userWhoSentRequest.Id == answer.UserId;

            if (!isUserHimSelfRequesting)
                return BadRequest("You can't update others' answers.");

            if (dto.Score != answer.Score)
            {
                var pollQuestion = await _pollQuestionRepository.GetByIdAsync(cancellationToken, answer.PollQuestionId);
                if (dto.Score < pollQuestion.MinScore || dto.Score > pollQuestion.MaxScore)
                    return BadRequest(
                        $"Score must be something between {pollQuestion.MinScore} and {pollQuestion.MaxScore}");
            }


            _mapper.Map(dto, answer);

            await _answerRepository.UpdateAsync(answer, cancellationToken);


            var professor = _professorRepository.GetById(answer.ProfessorId);

            professor.AverageRate = (int)professor.Answers.Average(x => x.Score);

            await _professorRepository.UpdateAsync(professor, cancellationToken);

            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var answer = await _answerRepository.GetByIdAsync(cancellationToken, id);

            if (answer == null)
                return NotFound();

            var userWhoSentRequestId = _jwtService.GetIdFromToken(Request.Headers["Authorization"]);
            var userWhoSentRequest = await _userManager.FindByIdAsync(userWhoSentRequestId);

            var isAdmin = await _userManager.IsInRoleAsync(userWhoSentRequest, "Admin");

            var isUserHimSelfRequesting = userWhoSentRequest.Id == answer.UserId;

            if (!isAdmin && !isUserHimSelfRequesting)
                return Forbid();

            await _answerRepository.DeleteAsync(answer, cancellationToken);


            var professor = _professorRepository.GetById(answer.ProfessorId);

            professor.AverageRate = (int)professor.Answers.Average(x => x.Score);

            await _professorRepository.UpdateAsync(professor, cancellationToken);

            return NoContent();
        }
    }
}
